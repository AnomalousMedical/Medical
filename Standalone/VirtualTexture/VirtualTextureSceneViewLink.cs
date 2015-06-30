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

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.sceneViewController = standaloneController.SceneViewController;
            this.sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            this.standaloneController = standaloneController;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;

            //OgreInterface.Instance.MaterialParser.addMaterialBuilder(this);
        }

        public void Dispose()
        {
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            IDisposableUtil.DisposeIfNotNull(virtualTexture);
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            virtualTexture = new VirtualTextureManager(window);
            window.RenderingStarted += window_RenderingStarted; //This leaks when disposed, need to track windows and remove these events
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
        }

        void window_RenderingStarted(SceneViewWindow window, bool currentCameraRender)
        {
            virtualTexture.update(window, currentCameraRender);
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            //Dumb but easy way to detect virtual textures, iterate over everything in the material manager
            foreach(Material material in MaterialManager.getInstance().Iterator)
            {
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

        public override MaterialPtr buildMaterial(MaterialDescription description)
        {
            MaterialPtr material = MaterialManager.getInstance().create(description.Name, description.Group, false, null);
            var mainTech = material.Value.createTechnique();
            switch(description.ShaderName)
            {
                case "NormalMapSpecularMapGlossMap":
                    createNormalMapSpecularMapGlossMap(mainTech, description);
                    break;
            }
            if(description.HasTextures)
            {
                var feedbackTechnique = material.Value.createTechnique();
                VirtualTextureManager.createOrRetrieveIndirectionTexture(description.TextureSet, new IntSize2(2048, 2048)).setupFeedbackBufferTechnique(feedbackTechnique);
            }
            return material;
        }

        public override void destroyMaterial(MaterialPtr materialPtr)
        {
            materialPtr.Dispose();
        }

        private void createNormalMapSpecularMapGlossMap(Technique technique, MaterialDescription description)
        {
            var pass = technique.createPass();

            pass.setSpecular(description.SpecularColor);
            pass.setDiffuse(description.DiffuseColor);
            pass.setEmissive(description.EmissiveColor);

            setVertexProgram(pass, description);

            pass.setFragmentProgram("NormalMapSpecularMapGlossMapFP");
            using(var gpuParams = pass.getFragmentProgramParameters())
            {
                gpuParams.Value.setNamedConstant("glossyStart", description.GlossyStart);
                gpuParams.Value.setNamedConstant("glossyRange", description.GlossyRange);
            }

            pass.createTextureUnitState(VirtualTextureManager.getPhysicalTexture("NormalMap").TextureName);
            pass.createTextureUnitState(VirtualTextureManager.getPhysicalTexture("Diffuse").TextureName);
            pass.createTextureUnitState(VirtualTextureManager.getPhysicalTexture("Specular").TextureName);
            pass.createTextureUnitState(VirtualTextureManager.createOrRetrieveIndirectionTexture(description.TextureSet, new IntSize2(2048, 2048)).TextureName); //Slow key
        }

        private static void setVertexProgram(Pass pass, MaterialDescription description)
        {
            StringBuilder programName = new StringBuilder("UnifiedVP");
            if(description.NumHardwareBones > 0)
            {
                programName.AppendFormat("HardwareSkin{0}BonePerVertex", description.NumHardwareBones);
            }
            if(description.NumHardwarePoses > 0)
            {
                programName.AppendFormat("{0}Pose", description.NumHardwarePoses);
            }
            if(description.Parity)
            {
                programName.AppendFormat("Parity");
            }
            pass.setVertexProgram(programName.ToString());
        }
    }
}
