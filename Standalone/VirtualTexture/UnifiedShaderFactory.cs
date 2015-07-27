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
        private const String ShaderPathBase = "Medical.VirtualTexture";
        private const String UnifiedShaderBase = ShaderPathBase + ".Unified.D3D11.";
        private const String EyeShaderBase = ShaderPathBase + ".Eye.D3D11.";

        private Dictionary<String, Action<String, int, int, bool>> vertexBuilderFuncs = new Dictionary<string, Action<String, int, int, bool>>();
        private Dictionary<String, Action<String, bool>> fragmentBuilderFuncs = new Dictionary<string, Action<String, bool>>();
        private Dictionary<String, HighLevelGpuProgramSharedPtr> createdPrograms = new Dictionary<string, HighLevelGpuProgramSharedPtr>();
        private ResourceGroup shaderResourceGroup;

        public UnifiedShaderFactory(ResourceManager liveResourceManager)
        {
            var rendererResources = liveResourceManager.getSubsystemResource("Ogre");
            shaderResourceGroup = rendererResources.addResourceGroup("UnifiedShaderFactory");
            shaderResourceGroup.addResource(GetType().AssemblyQualifiedName, "EmbeddedResource", true);
            
            liveResourceManager.initializeResources();

            vertexBuilderFuncs.Add("UnifiedVP", setupUnifiedVP);
            vertexBuilderFuncs.Add("DepthCheckVP", setupDepthCheckVP);
            vertexBuilderFuncs.Add("NoTexturesVP", setupNoTexturesVP);
            vertexBuilderFuncs.Add("FeedbackBufferVP", setupFeedbackBufferVP);
            vertexBuilderFuncs.Add("HiddenVP", setupHiddenVP);
            vertexBuilderFuncs.Add("EyeOuterVP", setupEyeOuterVP);

            fragmentBuilderFuncs.Add("FeedbackBufferFP", createFeedbackBufferFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularFP", createNormalMapSpecularFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularHighlightFP", createNormalMapSpecularHighlightFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularMapFP", createNormalMapSpecularMapFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularOpacityMapFP", createNormalMapSpecularOpacityMapFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularMapGlossMapFP", createNormalMapSpecularMapGlossMapFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularMapOpacityMapFP", createNormalMapSpecularMapOpacityMapFP);
            fragmentBuilderFuncs.Add("NormalMapSpecularMapOpacityMapGlossMapFP", createNormalMapSpecularMapOpacityMapGlossMapFP);
            fragmentBuilderFuncs.Add("HiddenFP", createHiddenFP);
            fragmentBuilderFuncs.Add("NoTexturesColoredFP", createNoTexturesColoredFP);
            fragmentBuilderFuncs.Add("EyeOuterFP", createEyeOuterFP);
        }

        public void Dispose()
        {
            foreach(var program in createdPrograms.Values)
            {
                program.Dispose();
            }
        }

        #region Vertex Programs

        public String createVertexProgram(String baseName, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            String shaderName = DetermineVertexShaderName(baseName, numHardwareBones, numHardwarePoses, parity);
            if(!createdPrograms.ContainsKey(shaderName))
            {
                Action<String, int, int, bool> buildFunc;
                if(vertexBuilderFuncs.TryGetValue(baseName, out buildFunc))
                {
                    buildFunc(shaderName, numHardwareBones, numHardwarePoses, parity);
                }
                else
                {
                    Logging.Log.Error("Cannot build vertex shader '{0}' no setup function defined.", shaderName);
                }
            }
            return shaderName;
        }

        private void setupUnifiedVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedVS.hlsl";
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
            program.Value.setParam("preprocessor_defines", DetermineVertexPreprocessorDefines(numHardwareBones, numHardwarePoses, parity));
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

        private void setupNoTexturesVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedVS.hlsl";
            if (numHardwareBones > 0 || numHardwarePoses > 0)
            {
                program.Value.setParam("entry_point", "NoTexturesVPHardwareSkin");
            }
            else
            {
                program.Value.setParam("entry_point", "NoTexturesVP");
            }

            program.Value.setParam("target", "vs_4_0");
            program.Value.setParam("column_major_matrices", "false");
            program.Value.SkeletalAnimationIncluded = numHardwareBones > 0;
            program.Value.NumberOfPoses = (ushort)numHardwarePoses;
            program.Value.setParam("preprocessor_defines", DetermineVertexPreprocessorDefines(numHardwareBones, numHardwarePoses, parity));
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

        private void setupDepthCheckVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            parity = false; //Does not do parity

            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "DepthCheck.hlsl";
            if (numHardwareBones > 0 || numHardwarePoses > 0)
            {
                program.Value.setParam("entry_point", "depthCheckSkinPose");
            }
            else
            {
                program.Value.setParam("entry_point", "depthCheckVP");
            }

            program.Value.setParam("target", "vs_4_0");
            program.Value.setParam("column_major_matrices", "false");
            program.Value.SkeletalAnimationIncluded = numHardwareBones > 0;
            program.Value.NumberOfPoses = (ushort)numHardwarePoses;
            program.Value.setParam("preprocessor_defines", DetermineVertexPreprocessorDefines(numHardwareBones, numHardwarePoses, parity));
            program.Value.load();

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                if (numHardwareBones > 0 || numHardwarePoses > 0)
                {
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
                }
            }
        }

        private void setupHiddenVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            parity = false; //Does not do parity

            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "Hidden.hlsl";
            program.Value.setParam("entry_point", "hiddenVP");
            program.Value.setParam("target", "vs_4_0");
            program.Value.setParam("column_major_matrices", "false");
            program.Value.SkeletalAnimationIncluded = numHardwareBones > 0;
            program.Value.NumberOfPoses = (ushort)numHardwarePoses;
            program.Value.setParam("preprocessor_defines", DetermineVertexPreprocessorDefines(numHardwareBones, numHardwarePoses, parity));
            program.Value.load();
        }

        private void setupFeedbackBufferVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            parity = false; //Does not do parity

            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "FeedbackBuffer.hlsl";
            if (numHardwareBones > 0 || numHardwarePoses > 0)
            {
                program.Value.setParam("entry_point", "FeedbackBufferVPHardwareSkinPose");
            }
            else
            {
                program.Value.setParam("entry_point", "FeedbackBufferVP");
            }

            program.Value.setParam("target", "vs_4_0");
            program.Value.setParam("column_major_matrices", "false");
            program.Value.SkeletalAnimationIncluded = numHardwareBones > 0;
            program.Value.NumberOfPoses = (ushort)numHardwarePoses;
            program.Value.setParam("preprocessor_defines", DetermineVertexPreprocessorDefines(numHardwareBones, numHardwarePoses, parity));
            program.Value.load();

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                if (numHardwareBones > 0 || numHardwarePoses > 0)
                {
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
                }
            }
        }

        private void setupEyeOuterVP(String name, int numHardwareBones, int numHardwarePoses, bool parity)
        {
            numHardwareBones = 0;
            numHardwarePoses = 0;
            parity = false;

            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_VERTEX_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = EyeShaderBase + "Eye.hlsl";
            program.Value.setParam("entry_point", "mainVP");

            program.Value.setParam("target", "vs_4_0");
            program.Value.load();

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("worldViewProj", AutoConstantType.ACT_WORLDVIEWPROJ_MATRIX);
                defaultParams.Value.setNamedAutoConstant("eyePosition", AutoConstantType.ACT_CAMERA_POSITION_OBJECT_SPACE);
                defaultParams.Value.setNamedAutoConstant("lightAttenuation", AutoConstantType.ACT_LIGHT_ATTENUATION, 0);
                defaultParams.Value.setNamedAutoConstant("lightPosition", AutoConstantType.ACT_LIGHT_POSITION_OBJECT_SPACE, 0);
            }
        }

        #endregion Vertex Programs

        #region Fragment Programs

        public String createFragmentProgram(String baseName, bool alpha)
        {
            String shaderName = DetermineFragmentShaderName(baseName, alpha);
            if (!createdPrograms.ContainsKey(shaderName))
            {
                Action<String, bool> buildFunc;
                if (fragmentBuilderFuncs.TryGetValue(baseName, out buildFunc))
                {
                    buildFunc(shaderName, alpha);
                }
                else
                {
                    Logging.Log.Error("Cannot build fragment shader '{0}' no setup function defined.", shaderName);
                }
            }
            return shaderName;
        }

        public void createNormalMapSpecularFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using(var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("specularColor", AutoConstantType.ACT_SURFACE_SPECULAR_COLOUR);
                defaultParams.Value.setNamedAutoConstant("glossyness", AutoConstantType.ACT_SURFACE_SHININESS);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                if(alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNormalMapSpecularHighlightFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularHighlightFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("specularColor", AutoConstantType.ACT_SURFACE_SPECULAR_COLOUR);
                defaultParams.Value.setNamedAutoConstant("glossyness", AutoConstantType.ACT_SURFACE_SHININESS);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                defaultParams.Value.setNamedAutoConstant("highlightColor", AutoConstantType.ACT_CUSTOM, 1);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNormalMapSpecularMapFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularMapFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("glossyness", AutoConstantType.ACT_SURFACE_SHININESS);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNormalMapSpecularOpacityMapFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularOpacityMapFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("specularColor", AutoConstantType.ACT_SURFACE_SPECULAR_COLOUR);
                defaultParams.Value.setNamedAutoConstant("glossyness", AutoConstantType.ACT_SURFACE_SHININESS);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNormalMapSpecularMapGlossMapFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularMapGlossMapFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                defaultParams.Value.setNamedConstant("glossyStart", 40.0f);
                defaultParams.Value.setNamedConstant("glossyRange", 0.0f);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNormalMapSpecularMapOpacityMapFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularMapOpacityMapFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("glossyness", AutoConstantType.ACT_SURFACE_SHININESS);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNormalMapSpecularMapOpacityMapGlossMapFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "normalMapSpecularMapOpacityMapGlossMapFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                defaultParams.Value.setNamedConstant("glossyStart", 40.0f);
                defaultParams.Value.setNamedConstant("glossyRange", 0.0f);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createNoTexturesColoredFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "UnifiedFS.hlsl";
            program.Value.setParam("entry_point", "NoTexturesColoredFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("diffuseColor", AutoConstantType.ACT_SURFACE_DIFFUSE_COLOUR);
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("specularColor", AutoConstantType.ACT_SURFACE_SPECULAR_COLOUR);
                defaultParams.Value.setNamedAutoConstant("glossyness", AutoConstantType.ACT_SURFACE_SHININESS);
                defaultParams.Value.setNamedAutoConstant("emissiveColor", AutoConstantType.ACT_SURFACE_EMISSIVE_COLOUR);
                if (alpha)
                {
                    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                }
            }
        }

        public void createFeedbackBufferFP(String name, bool alpha)
        {
            alpha = false; //Never does alpha

            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "FeedbackBuffer.hlsl";
            program.Value.setParam("entry_point", "FeedbackBufferFP");
            program.Value.setParam("target", "ps_4_0");
        }

        public void createHiddenFP(String name, bool alpha)
        {
            alpha = false; //Never does alpha

            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = UnifiedShaderBase + "Hidden.hlsl";
            program.Value.setParam("entry_point", "hiddenFP");
            program.Value.setParam("target", "ps_4_0");
        }

        public void createEyeOuterFP(String name, bool alpha)
        {
            var program = HighLevelGpuProgramManager.Instance.createProgram(name, shaderResourceGroup.FullName, "hlsl", GpuProgramType.GPT_FRAGMENT_PROGRAM);
            createdPrograms.Add(name, program);

            program.Value.SourceFile = EyeShaderBase + "Eye.hlsl";
            program.Value.setParam("entry_point", "eyeOuterFP");
            program.Value.setParam("target", "ps_4_0");
            program.Value.setParam("preprocessor_defines", DetermineFragmentPreprocessorDefines(alpha));

            using (var defaultParams = program.Value.getDefaultParameters())
            {
                defaultParams.Value.setNamedAutoConstant("lightDiffuseColor", AutoConstantType.ACT_LIGHT_DIFFUSE_COLOUR, 0);
                defaultParams.Value.setNamedAutoConstant("specularColor", AutoConstantType.ACT_SURFACE_SPECULAR_COLOUR);
                //if (alpha)
                //{
                //    defaultParams.Value.setNamedAutoConstant("alpha", AutoConstantType.ACT_CUSTOM, 0);
                //}
            }
        }

        #endregion Fragment Programs

        private static String DetermineVertexShaderName(String baseName, int numHardwareBones, int numHardwarePoses, bool parity)
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

        private static String DetermineFragmentShaderName(String baseName, bool alpha)
        {
            if (alpha)
            {
                baseName += "Alpha";
            }
            return baseName;
        }

        private static String DetermineVertexPreprocessorDefines(int numHardwareBones, int numHardwarePoses, bool parity)
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
                definesBuilder.AppendFormat("POSE_COUNT={0};", numHardwarePoses);
            }
            return definesBuilder.ToString();
        }

        private static String DetermineFragmentPreprocessorDefines(bool alpha)
        {
            StringBuilder definesBuilder = new StringBuilder("VIRTUAL_TEXTURE;");
            if(alpha)
            {
                definesBuilder.Append("ALPHA;");
            }
            return definesBuilder.ToString();
        }
    }
}
