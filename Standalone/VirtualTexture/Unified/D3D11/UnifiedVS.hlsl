//----------------------------------
// This file contains our mega shader. You can control how it works with some preprocessor defines
//
// PARITY - Define this to turn on parity for the shader.
// BONES_PER_VERTEX - Set to 1, 2, 3, or 4 depending on how many bones can effect each vertex
// POSE_COUNT - Set to 1 or 2 depending on how many poses you want to hardware skin
//
//----------------------------------

//Application to vertex program
struct a2v
{
	//Input position
	float4 position : POSITION;

	//Input texture coords
	float2 texCoords : TEXCOORD0;

	//Input tangent
	#ifdef PARITY
		float4 tangent : TANGENT0;
	#else
		float3 tangent : TANGENT0;
	#endif

	//Input binormal
	float3 binormal : BINORMAL0;

	//Input normal
	float3 normal : NORMAL0;
};

//Application to vertex program
struct a2vNoTexture
{
	//Input position
	float4 position : POSITION;

	//Input normal
	float3 normal : NORMAL0;
};

//Vertex program to fragment program struct
struct v2f
{
	//Output position
	float4 position : SV_POSITION;

	//Output texture coords
	float2 texCoords : TEXCOORD0;

	//Light vector in tangent space
	float3 lightVector : TEXCOORD1;

	//Eye vector in tangent space
	float3 halfVector : TEXCOORD2;

	//Attenuation per vertex
	float4 attenuation : TEXCOORD3;
};

//Vertex program to fragment program no texture struct
struct v2fNoTexture
{
	//Output position
	float4 position : SV_POSITION;

	//Normal
	float3 normal : TEXCOORD0;

	//Light vector in tangent space
	float3 lightVector : TEXCOORD1;

	//Eye vector in tangent space
	float3 halfVector : TEXCOORD2;

	//Attenuation per vertex
	float4 attenuation : TEXCOORD3;
};

//Pack function packs values from -1 to 1 to 0 and 1
float3 pack(float3 input)
{
	return 0.5 * input + 0.5;
}

//-------------------------------------------------------------------------------------------------

void computeOutput( float3x3 TBNMatrix, 
				    float4 position,
				    float2 texCoords, 
				    float4x4 worldViewProj,
					float3 eyePosition,
					float4 lightAttenuation,
					float4 lightPosition,
					out v2f output
					)
{
	//Calculate the local light vector
	output.lightVector = lightPosition.xyz - position.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(output.lightVector);
	output.attenuation = 1 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1);

	//Calculate the half vector
	output.lightVector = normalize(output.lightVector);
	output.halfVector = normalize(eyePosition - position.xyz) + output.lightVector;

	//Transform the light vector from object space into tangent space
	output.lightVector = mul(TBNMatrix, output.lightVector);
	output.lightVector = pack(output.lightVector);
	
	//Transform the eye vector to tangent space
	output.halfVector = mul(TBNMatrix, output.halfVector);
	output.halfVector = pack(output.halfVector);
	
	//Copy the texture coords
	output.texCoords = texCoords;

	// Transform the current vertex from object space to clip space
	output.position = mul(worldViewProj, position);
}

//----------------------------------
//Shared Vertex Program
//----------------------------------
void mainVP
(
		in a2v input,
		out v2f output,		

		const uniform float4x4 worldViewProj,	//The world view projection matrix
		const uniform float3 eyePosition,		//The eye position
		uniform float4 lightAttenuation,		//The attenuation of the light
		const uniform float4 lightPosition		//The position of the light in object space
)
{
	//Tangent space conversion matrix
	#ifdef PARITY
		float3x3 TBNMatrix = float3x3(input.tangent.xyz, input.binormal * input.tangent.w, input.normal);
	#else
		float3x3 TBNMatrix = float3x3(input.tangent, input.binormal, input.normal);
	#endif

	computeOutput(TBNMatrix, input.position, input.texCoords, worldViewProj, eyePosition, lightAttenuation, lightPosition, output);
}

//----------------------------------
//Shared Vertex Program with Hardware Skinning.
//----------------------------------
void mainVPHardwareSkin
(
		in a2v input,
		out v2f output,		

		int4 blendIdx : BLENDINDICES,
		float4 blendWgt : BLENDWEIGHT,

		const uniform float3 worldEyePosition,		//The eye position
		uniform float4 lightAttenuation,		//The attenuation of the light
		const uniform float4 worldLightPosition,		//The position of the light in object space

		const uniform float3x4 worldMatrix3x4Array[60], //This is an array of bones, the index is the maximum amount of bones supported
		const uniform float4x4 viewProjectionMatrix

		#if POSE_COUNT > 0
			, float3 pose1pos  : TEXCOORD1
			#if POSE_COUNT > 1
				, float3 pose2pos  : TEXCOORD2
			#endif
			, uniform float4 poseAnimAmount
		#endif
)
{
	//Hardware Pose Animation
	#if POSE_COUNT == 1
		input.position.xyz = input.position.xyz + poseAnimAmount.x * pose1pos;
	#elif POSE_COUNT == 2
		input.position.xyz = input.position.xyz + poseAnimAmount.x * pose1pos + poseAnimAmount.y * pose2pos;
	#endif

	//Hardware Skinning
	float4 blendPos = float4(0,0,0,0);
	float3 newNormal = float3(0,0,0);
	float3 newTangent = float3(0,0,0);
	//-----------Skinning Unrolled Loop--------------
		float3x4 worldMatrix;
		float weight;

		//First Bone
		#if BONES_PER_VERTEX > 0
			worldMatrix = worldMatrix3x4Array[blendIdx[0]];
			weight = blendWgt[0];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
			newTangent += mul((float3x3)worldMatrix, input.tangent.xyz) * weight;
		#endif

		//Second Bone
		#if BONES_PER_VERTEX > 1
			worldMatrix = worldMatrix3x4Array[blendIdx[1]];
			weight = blendWgt[1];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
			newTangent += mul((float3x3)worldMatrix, input.tangent.xyz) * weight;
		#endif

		//Third Bone
		#if BONES_PER_VERTEX > 2
			worldMatrix = worldMatrix3x4Array[blendIdx[2]];
			weight = blendWgt[2];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
			newTangent += mul((float3x3)worldMatrix, input.tangent.xyz) * weight;
		#endif

		//Fourth Bone
		#if BONES_PER_VERTEX > 3
			worldMatrix = worldMatrix3x4Array[blendIdx[3]];
			weight = blendWgt[3];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
			newTangent += mul((float3x3)worldMatrix, input.tangent.xyz) * weight;
		#endif
	//---------------End Skinning Unrolled Loop------------------
	newNormal = normalize(newNormal);
	newTangent = normalize(newTangent);
	float3 newBinormal = cross(newTangent, newNormal);

	//Normal Mapping
	//Tangent space conversion matrix
	#ifdef PARITY
		float3x3 TBNMatrix = float3x3(newTangent, newBinormal * input.tangent.w, newNormal);
	#else
		float3x3 TBNMatrix = float3x3(newTangent, newBinormal, newNormal);
	#endif

	computeOutput(TBNMatrix, blendPos, input.texCoords, viewProjectionMatrix, worldEyePosition, lightAttenuation, worldLightPosition, output);
}


//----------------------------------
//No Textures
//----------------------------------
void computeNoTextureOutput(float4 position,
							float3 normal, 
							float4x4 worldViewProj,
							float3 eyePosition,
							float4 lightAttenuation,
							float4 lightPosition,
							out v2fNoTexture output)
{
	//Calculate the local light vector
	output.lightVector = lightPosition.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(output.lightVector);
	output.attenuation = 1 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1);

	//Calculate the half vector
	output.halfVector = eyePosition + output.lightVector;
	
	//Copy the texture coords
	output.normal = normal;

	// Transform the current vertex from object space to clip space
	output.position = mul(worldViewProj, position);
}

//----------------------------------
//NoTexturesVP
//----------------------------------
void NoTexturesVP
(
		in a2vNoTexture input,
		out v2fNoTexture output,		

		const uniform float4x4 worldViewProj,	//The world view projection matrix
		const uniform float3 eyePosition,		//The eye position
		uniform float4 lightAttenuation,		//The attenuation of the light
		const uniform float4 lightPosition		//The position of the light in object space
)
{
	computeNoTextureOutput(input.position, input.normal, worldViewProj, eyePosition, lightAttenuation, lightPosition, output);
}

//----------------------------------
//NoTexturesVPHardwareSkin
//----------------------------------
void NoTexturesVPHardwareSkin
(
		in a2vNoTexture input,
		out v2fNoTexture output,		

		const uniform float3 worldEyePosition,		//The eye position
		uniform float4 lightAttenuation,		//The attenuation of the light
		const uniform float4 worldLightPosition,		//The position of the light

		int4 blendIdx : BLENDINDICES,
		float4 blendWgt : BLENDWEIGHT,

		const uniform float3x4 worldMatrix3x4Array[60], //This is an array of bones, the index is the maximum amount of bones supported
		const uniform float4x4 viewProjectionMatrix

		#if POSE_COUNT > 0
				, float3 pose1pos  : TEXCOORD1
			#if POSE_COUNT > 1
					, float3 pose2pos  : TEXCOORD2
			#endif
				, uniform float4 poseAnimAmount
		#endif
)
{
	//Hardware Pose Animation
#if POSE_COUNT == 1
	input.position.xyz = input.position.xyz + poseAnimAmount.x * pose1pos;
#elif POSE_COUNT == 2
	input.position.xyz = input.position.xyz + poseAnimAmount.x * pose1pos + poseAnimAmount.y * pose2pos;
#endif

	//Hardware Skinning
	float4 blendPos = float4(0,0,0,0);
	float3 newNormal = float3(0,0,0);
	//-----------Skinning Unrolled Loop--------------
		float3x4 worldMatrix;
		float weight;

		//First Bone
		#if BONES_PER_VERTEX > 0
			worldMatrix = worldMatrix3x4Array[blendIdx[0]];
			weight = blendWgt[0];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
		#endif

		//Second Bone
		#if BONES_PER_VERTEX > 1
			worldMatrix = worldMatrix3x4Array[blendIdx[1]];
			weight = blendWgt[1];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
		#endif

		//Third Bone
		#if BONES_PER_VERTEX > 2
			worldMatrix = worldMatrix3x4Array[blendIdx[2]];
			weight = blendWgt[2];

			blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
			newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
		#endif

		#if BONES_PER_VERTEX > 3
		//Fourth Bone
		worldMatrix = worldMatrix3x4Array[blendIdx[3]];
		weight = blendWgt[3];

		blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
		newNormal += mul((float3x3)worldMatrix, input.normal) * weight;
		#endif
	//---------------End Skinning Unrolled Loop------------------

	newNormal = normalize(newNormal);

	computeNoTextureOutput(blendPos, newNormal, viewProjectionMatrix, worldEyePosition, lightAttenuation, worldLightPosition, output);
}