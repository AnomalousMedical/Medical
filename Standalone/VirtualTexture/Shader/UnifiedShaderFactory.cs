using Engine.Resources;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class UnifiedShaderFactory : IDisposable
    {
        private const String ShaderBaseName = "Medical.VirtualTexture.Shader.D3D11.";

        private Dictionary<String, Action<String, int, int, bool>> shaderBuilderFuncs = new Dictionary<string, Action<String, int, int, bool>>();
        private Dictionary<String, HighLevelGpuProgramSharedPtr> createdVertexPrograms = new Dictionary<string, HighLevelGpuProgramSharedPtr>();
        private ResourceGroup shaderResourceGroup;

        public UnifiedShaderFactory(ResourceManager liveResourceManager)
        {
            var rendererResources = liveResourceManager.getSubsystemResource("Ogre");
            shaderResourceGroup = rendererResources.addResourceGroup("UnifiedShaderFactory");
            shaderResourceGroup.addResource(GetType().AssemblyQualifiedName, "EmbeddedResource", true);
            
            liveResourceManager.initializeResources();

            shaderBuilderFuncs.Add("UnifiedVP", setupUnifiedVP);
        }

        public void Dispose()
        {
            foreach(var program in createdVertexPrograms.Values)
            {
                program.Dispose();
            }
        }

        public String createVertexProgram(String baseName, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            String shaderName = DetermineVertexShaderName(baseName, numHardwareBones, numHardwarePoses, parity);
            if(!createdVertexPrograms.ContainsKey(shaderName))
            {
                Action<String, int, int, bool> buildFunc;
                if(shaderBuilderFuncs.TryGetValue(baseName, out buildFunc))
                {
                    buildFunc(shaderName, numHardwareBones, numHardwarePoses, parity);
                }
                else
                {
                    Logging.Log.Error("Cannot build shader '{0}' no setup function defined.");
                }
            }
            return shaderName;
        }

        private void setupUnifiedVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdVertexPrograms.Add(name, program);

            program.Value.SourceFile = ShaderBaseName + "UnifiedVS.hlsl";
            if (numHardwareBones > 0 || numHardwarePoses > 0)
            {
                program.Value.setParam("entry_point", "mainVPHardwareSkin");
            }
            else
            {
                program.Value.setParam("entry_point", "mainVP");
            }

            program.Value.setParam("target", "vs_4_0");
            program.Value.setParam("column_major_matrices", "false");
            program.Value.SkeletalAnimationIncluded = numHardwareBones > 0;
            program.Value.NumberOfPoses = (ushort)numHardwarePoses;
            program.Value.setParam("preprocessor_defines", DeterminePreprocessorDefines(numHardwareBones, numHardwarePoses, parity));
            program.Value.load();

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                if (numHardwareBones > 0 || numHardwarePoses > 0)
                {
                    defaultParams.Value.setNamedAutoConstant("worldEyePosition", AutoConstantType.ACT_CAMERA_POSITION);
                    defaultParams.Value.setNamedAutoConstant("lightAttenuation", AutoConstantType.ACT_LIGHT_ATTENUATION, 0);
                    defaultParams.Value.setNamedAutoConstant("worldLightPosition", AutoConstantType.ACT_LIGHT_POSITION, 0);

                    defaultParams.Value.setNamedAutoConstant("worldMatrix3x4Array", AutoConstantType.ACT_WORLD_MATRIX_ARRAY_3x4);
                    defaultParams.Value.setNamedAutoConstant("viewProjectionMatrix", AutoConstantType.ACT_VIEWPROJ_MATRIX);

                    if (numHardwarePoses > 0)
                    {
                        defaultParams.Value.setNamedAutoConstant("poseAnimAmount", AutoConstantType.ACT_ANIMATION_PARAMETRIC);
                    }
                }
                else
                {
                    defaultParams.Value.setNamedAutoConstant("worldViewProj", AutoConstantType.ACT_WORLDVIEWPROJ_MATRIX);
                    defaultParams.Value.setNamedAutoConstant("eyePosition", AutoConstantType.ACT_CAMERA_POSITION_OBJECT_SPACE);
                    defaultParams.Value.setNamedAutoConstant("lightAttenuation", AutoConstantType.ACT_LIGHT_ATTENUATION, 0);
                    defaultParams.Value.setNamedAutoConstant("lightPosition", AutoConstantType.ACT_LIGHT_POSITION_OBJECT_SPACE, 0);
                }
            }
        }

        public static String DetermineVertexShaderName(String baseName, int numHardwareBones, int numHardwarePoses, bool parity)
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

        public static String DetermineFragmentShaderName(String baseName, bool alpha)
        {
            if (alpha)
            {
                baseName += "Alpha";
            }
            return baseName;
        }

        private static String DeterminePreprocessorDefines(int numHardwareBones, int numHardwarePoses, bool parity)
        {
            StringBuilder definesBuilder = new StringBuilder();
            if (parity)
            {
                definesBuilder.AppendFormat("PARITY;");
            }
            if(numHardwareBones > 0)
            {
                definesBuilder.AppendFormat("BONES_PER_VERTEX={0};", numHardwareBones);
            }
            if(numHardwarePoses > 0)
            {
                definesBuilder.AppendFormat("POSE_COUNT={0};", numHardwareBones);
            }
            return definesBuilder.ToString();
        }
    }
}
