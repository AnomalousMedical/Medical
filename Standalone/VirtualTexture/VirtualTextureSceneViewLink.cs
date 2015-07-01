using Anomalous.GuiFramework.Cameras;
using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class VirtualTextureSceneViewLink : MaterialBuilder, IDisposable
    {
        private VirtualTextureManager virtualTexture;
        private SceneViewController sceneViewController;
        private StandaloneController standaloneController;

        private PhysicalTexture normalTexture;
        private PhysicalTexture diffuseTexture;
        private PhysicalTexture specularTexture;
        private PhysicalTexture opacityTexture;
        private List<Material> createdMaterials = new List<Material>(); //This is only for detection

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.sceneViewController = standaloneController.SceneViewController;
            this.sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            this.standaloneController = standaloneController;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;

            virtualTexture = new VirtualTextureManager();

            diffuseTexture = virtualTexture.createPhysicalTexture("Diffuse");
            normalTexture = virtualTexture.createPhysicalTexture("NormalMap");
            specularTexture = virtualTexture.createPhysicalTexture("Specular");
            opacityTexture = virtualTexture.createPhysicalTexture("Opacity");

            //Debug texture colors
            normalTexture.color(Color.Blue);
            diffuseTexture.color(Color.Red);
            specularTexture.color(Color.Green);
            opacityTexture.color(Color.HotPink);

            OgreInterface.Instance.MaterialParser.addMaterialBuilder(this);
        }

        public void Dispose()
        {
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            IDisposableUtil.DisposeIfNotNull(virtualTexture);
        }

        void sceneViewController_WindowCreated(SceneViewWindow window) //Only works for the first window
        {
            standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
            window.CameraCreated += window_CameraCreated;
            window.CameraDestroyed += window_CameraDestroyed;
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
        }

        void window_CameraCreated(SceneViewWindow window)
        {
            int width = window.RenderWidth / 10;
            if (width < 10)
            {
                width = 10;
            }
            int height = window.RenderHeight / 10;
            if (height < 10)
            {
                height = 10;
            }

            VirtualTextureManager.createFeedbackBufferCamera(window.Camera, new IntSize2(width, height));
        }

        void window_CameraDestroyed(SceneViewWindow window)
        {
            VirtualTextureManager.destroyFeedbackBufferCamera();
        }

        void MedicalController_OnLoopUpdate(Engine.Platform.Clock time)
        {
            virtualTexture.update();
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            //Dumb but easy way to detect virtual textures, iterate over everything in the material manager
            foreach(Material material in MaterialManager.getInstance().Iterator.Where(m => !createdMaterials.Contains(m)))
            {
                if(material.Name == "SkinMaterial_HWSkin")
                {
                    Logging.Log.Debug("");
                }

                int numTechniques = material.getNumTechniques();
                Technique technique = material.getTechnique((ushort)(numTechniques - 1));
                if (technique != null && technique.getSchemeName() == FeedbackBuffer.Scheme)
                {
                    String name = material.getName();
                    if (name.EndsWith("Alpha"))
                    {
                        name = name.Substring(0, name.Length - 5);
                    }
                    if (name.Contains("_HWSkin"))
                    {
                        name = name.Substring(0, name.LastIndexOf('_'));
                    }
                    int techniqueCount = material.getNumTechniques();
                    if(techniqueCount > 0)
                    {
                        Technique feedbackTechnique;
                        for (int i = techniqueCount; i > 0; --i )
                        {
                            feedbackTechnique = material.getTechnique((ushort)(i - 1)); //The last one is the most likely feedbackTechnique
                            if(feedbackTechnique.SchemeName == FeedbackBuffer.Scheme)
                            {
                                virtualTexture.processMaterialAdded(name, material.getTechnique(0), feedbackTechnique);
                            }
                        }
                    }
                }
            }
        }

        public VirtualTextureManager VirtualTextureManager
        {
            get
            {
                return virtualTexture;
            }
        }

        public override string Name
        {
            get
            {
                return "VirtualTexture";
            }
        }

        public override void buildMaterial(MaterialDescription description, MaterialRepository repo)
        {
            constructMaterial(description, repo, false);
            if(description.CreateAlphaMaterial)
            {
                constructMaterial(description, repo, true);
            }
        }

        public override void destroyMaterial(MaterialPtr materialPtr)
        {
            createdMaterials.Remove(materialPtr.Value);
            MaterialManager.getInstance().remove(materialPtr.Value.Name);
            materialPtr.Dispose();
        }

        private void constructMaterial(MaterialDescription description, MaterialRepository repo, bool alpha)
        {
            String name = description.Name;
            if(description.CreateAlphaMaterial && alpha) //Is this an automatic alpha material?
            {
                name += "Alpha";
            }
            MaterialPtr material = MaterialManager.getInstance().create(name, description.Group, false, null);
            IndirectionTexture indirectionTex = null;
            switch (description.ShaderName)
            {
                case "NormalMapSpecularMapGlossMap":
                    indirectionTex = createNormalMapSpecularMapGlossMap(material.Value.getTechnique(0), description, alpha, true);
                    if(alpha)
                    {
                        //Create no depth check technique
                        Technique technique = material.Value.createTechnique();
                        technique.setLodIndex(1);
                        technique.createPass();
                        createNormalMapSpecularMapGlossMap(technique, description, alpha, false);
                    }
                    break;
            }
            if (indirectionTex != null)
            {
                indirectionTex.setupFeedbackBufferTechnique(material.Value, determineVertexShaderName("", description.NumHardwareBones, description.NumHardwarePoses, false));
            }
            material.Value.compile();
            material.Value.load();

            createdMaterials.Add(material.Value);
            repo.addMaterial(material, description);
        }

        private IndirectionTexture createNormalMapSpecularMapGlossMap(Technique technique, MaterialDescription description, bool alpha, bool depthCheck)
        {
            var pass = technique.getPass(0); //Make sure technique has one pass already defined
            if(alpha && depthCheck)
            {
                //Setup depth check pass
                pass.setColorWriteEnabled(false);
                pass.setDepthBias(-1.0f);
                pass.setSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);

                pass.setVertexProgram(determineVertexShaderName("DepthCheckVP", description.NumHardwareBones, description.NumHardwarePoses, description.Parity));
                pass.setFragmentProgram("HiddenFP");

                pass = technique.createPass(); //Get another pass
            }

            pass.setSpecular(description.SpecularColor);
            pass.setDiffuse(description.DiffuseColor);
            pass.setEmissive(description.EmissiveColor);

            if(alpha)
            {
                pass.setSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);
                pass.setDepthFunction(CompareFunction.CMPF_LESS_EQUAL);
            }

            pass.setVertexProgram(determineVertexShaderName("UnifiedVP", description.NumHardwareBones, description.NumHardwarePoses, description.Parity));

            pass.setFragmentProgram(determineFragmentShaderName("NormalMapSpecularMapGlossMapFP", alpha));
            using (var gpuParams = pass.getFragmentProgramParameters())
            {
                virtualTexture.setupVirtualTextureFragmentParams(gpuParams);
                gpuParams.Value.setNamedConstant("glossyStart", description.GlossyStart);
                gpuParams.Value.setNamedConstant("glossyRange", description.GlossyRange);
            }

            var texUnit = pass.createTextureUnitState(normalTexture.TextureName);
            pass.createTextureUnitState(diffuseTexture.TextureName);
            pass.createTextureUnitState(specularTexture.TextureName);
            IndirectionTexture indirectionTexture;
            if (virtualTexture.createOrRetrieveIndirectionTexture(description.TextureSet, new IntSize2(2048, 2048), out indirectionTexture)) //Slow key
            {
                indirectionTexture.addOriginalTexture("NormalMap", description.NormalMap + ".png");
                indirectionTexture.addOriginalTexture("Diffuse", description.DiffuseMap + ".png");
                indirectionTexture.addOriginalTexture("Specular", description.SpecularMap + ".png");
            }
            setupIndirectionTexture(pass, indirectionTexture);

            return indirectionTexture;
        }

        private static String determineVertexShaderName(String baseName, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            StringBuilder programName = new StringBuilder(baseName);
            if (numHardwareBones > 0)
            {
                programName.AppendFormat("HardwareSkin{0}BonePerVertex", numHardwareBones);
            }
            if (numHardwarePoses > 0)
            {
                programName.AppendFormat("{0}Pose", numHardwarePoses);
            }
            if (parity)
            {
                programName.AppendFormat("Parity");
            }
            return programName.ToString();
        }

        private static String determineFragmentShaderName(String baseName, bool alpha)
        {
            if(alpha)
            {
                baseName += "Alpha";
            }
            return baseName;
        }

        private static void setupIndirectionTexture(Pass pass, IndirectionTexture indirectionTexture)
        {
            var indirectionTextureUnit = pass.createTextureUnitState(indirectionTexture.TextureName);
            indirectionTextureUnit.setFilteringOptions(FilterOptions.Point, FilterOptions.Point, FilterOptions.None);
        }
    }
}
