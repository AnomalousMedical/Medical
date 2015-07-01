using Engine;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class UnifiedMaterialBuilder : MaterialBuilder
    {
        private List<Material> createdMaterials = new List<Material>(); //This is only for detection
        private VirtualTextureManager virtualTextureManager;
        private String textureFormat = ".png";

        private PhysicalTexture normalTexture;
        private PhysicalTexture diffuseTexture;
        private PhysicalTexture specularTexture;
        private PhysicalTexture opacityTexture;

        public UnifiedMaterialBuilder(VirtualTextureManager virtualTextureManager)
        {
            this.virtualTextureManager = virtualTextureManager;

            diffuseTexture = virtualTextureManager.createPhysicalTexture("Diffuse");
            normalTexture = virtualTextureManager.createPhysicalTexture("NormalMap");
            specularTexture = virtualTextureManager.createPhysicalTexture("Specular");
            opacityTexture = virtualTextureManager.createPhysicalTexture("Opacity");

            //Debug texture colors
            normalTexture.color(Color.Blue);
            diffuseTexture.color(Color.Red);
            specularTexture.color(Color.Green);
            opacityTexture.color(Color.HotPink);
        }

        public bool isCreator(Material material)
        {
            return createdMaterials.Contains(material);
        }

        public override void buildMaterial(MaterialDescription description, MaterialRepository repo)
        {
            constructMaterial(description, repo, false);
            if (description.CreateAlphaMaterial)
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

        public override string Name
        {
            get
            {
                return "VirtualTexture";
            }
        }

        private void constructMaterial(MaterialDescription description, MaterialRepository repo, bool alpha)
        {
            String name = description.Name;
            if (description.CreateAlphaMaterial && alpha) //Is this an automatic alpha material?
            {
                name += "Alpha";
            }
            MaterialPtr material = MaterialManager.getInstance().create(name, description.Group, false, null);
            IndirectionTexture indirectionTex = null;
            switch (description.ShaderName)
            {
                case "NormalMapSpecularMapGlossMap":
                    indirectionTex = createNormalMapSpecularMapGlossMap(material.Value.getTechnique(0), description, alpha, true);
                    if (alpha)
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
            //Create depth check pass if needed
            var pass = createDepthPass(technique, description, alpha, depthCheck);

            //Setup this pass
            setupCommonPassAttributes(description, alpha, pass);

            //Material specific, setup shaders
            pass.setVertexProgram(determineVertexShaderName("UnifiedVP", description.NumHardwareBones, description.NumHardwarePoses, description.Parity));

            pass.setFragmentProgram(determineFragmentShaderName("NormalMapSpecularMapGlossMapFP", alpha));
            using (var gpuParams = pass.getFragmentProgramParameters())
            {
                virtualTextureManager.setupVirtualTextureFragmentParams(gpuParams);
                gpuParams.Value.setNamedConstant("glossyStart", description.GlossyStart);
                gpuParams.Value.setNamedConstant("glossyRange", description.GlossyRange);
            }

            //Setup textures
            return setupNormalDiffuseSpecularTextures(description, pass);
        }

        private IndirectionTexture setupNormalDiffuseSpecularTextures(MaterialDescription description, Pass pass)
        {
            var texUnit = pass.createTextureUnitState(normalTexture.TextureName);
            pass.createTextureUnitState(diffuseTexture.TextureName);
            pass.createTextureUnitState(specularTexture.TextureName);
            IndirectionTexture indirectionTexture;
            if (virtualTextureManager.createOrRetrieveIndirectionTexture(description.TextureSet, getTextureSize(), out indirectionTexture)) //Slow key
            {
                indirectionTexture.addOriginalTexture("NormalMap", description.NormalMap + textureFormat);
                indirectionTexture.addOriginalTexture("Diffuse", description.DiffuseMap + textureFormat);
                indirectionTexture.addOriginalTexture("Specular", description.SpecularMap + textureFormat);
            }
            setupIndirectionTexture(pass, indirectionTexture);
            return indirectionTexture;
        }

        private static IntSize2 getTextureSize()
        {
            return new IntSize2(2048, 2048); //Hardcoded size for now.
        }

        private static void setupCommonPassAttributes(MaterialDescription description, bool alpha, Pass pass)
        {
            pass.setSpecular(description.SpecularColor);
            pass.setDiffuse(description.DiffuseColor);
            pass.setEmissive(description.EmissiveColor);

            if (alpha)
            {
                pass.setSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);
                pass.setDepthFunction(CompareFunction.CMPF_LESS_EQUAL);
            }
        }

        private static Pass createDepthPass(Technique technique, MaterialDescription description, bool alpha, bool depthCheck)
        {
            var pass = technique.getPass(0); //Make sure technique has one pass already defined
            if (alpha && depthCheck)
            {
                //Setup depth check pass
                pass.setColorWriteEnabled(false);
                pass.setDepthBias(-1.0f);
                pass.setSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);

                pass.setVertexProgram(determineVertexShaderName("DepthCheckVP", description.NumHardwareBones, description.NumHardwarePoses, description.Parity));
                pass.setFragmentProgram("HiddenFP");

                pass = technique.createPass(); //Get another pass
            }
            return pass;
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
            if (alpha)
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
