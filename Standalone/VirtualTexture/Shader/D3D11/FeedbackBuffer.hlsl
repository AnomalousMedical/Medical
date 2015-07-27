//Application to vertex program
struct a2vFeedback
{
	//Input position
	float4 position : POSITION;

	//Input texture coords
	float2 texCoords : TEXCOORD0;
};

struct v2fFeedback
{
	//Output position
	float4 position : SV_POSITION;

	//Output texture coords
	float2 texCoords : TEXCOORD0;
};

float texMipLevel(float2 coord, float2 texSize)
{
	float2 dxScaled, dyScaled;
	float2 coord_scaled = coord * texSize;

	dxScaled = ddx(coord_scaled);
	dyScaled = ddy(coord_scaled);

	float2 dtex = dxScaled*dxScaled + dyScaled*dyScaled;
	float minDelta = max(dtex.x, dtex.y);
	float miplevel = max(0.5 * log2(minDelta), 0.0);

	return miplevel;
}

//----------------------------------
//FeedbackBufferVP
//----------------------------------
void FeedbackBufferVP
(
in a2vFeedback input,
out v2fFeedback output,

const uniform float4x4 worldViewProj
)
{
	output.position = mul(worldViewProj, input.position);
	output.texCoords = input.texCoords;
}

//----------------------------------
//Depth Check with Hardware Skinning and (optional) Pose
//----------------------------------
void FeedbackBufferVPHardwareSkinPose
(
in a2vFeedback input,
out v2fFeedback output,

int4 blendIdx : BLENDINDICES,
float4 blendWgt : BLENDWEIGHT,

const uniform float3x4 worldMatrix3x4Array[60], //This is an array of bones, the index is the maximum amount of bones supported
const uniform float4x4 viewProjectionMatrix

//Pose Animation Args
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
	//-----------Skinning Unrolled Loop--------------
	float4 blendPos = float4(0, 0, 0, 0);
		float3x4 worldMatrix;
	float weight;

	//First Bone
#if BONES_PER_VERTEX > 0
	worldMatrix = worldMatrix3x4Array[blendIdx[0]];
	weight = blendWgt[0];

	blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
#endif

	//Second Bone
#if BONES_PER_VERTEX > 1
	worldMatrix = worldMatrix3x4Array[blendIdx[1]];
	weight = blendWgt[1];

	blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
#endif

	//Third Bone
#if BONES_PER_VERTEX > 2
	worldMatrix = worldMatrix3x4Array[blendIdx[2]];
	weight = blendWgt[2];

	blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
#endif

	//Fourth Bone
#if BONES_PER_VERTEX > 3
	worldMatrix = worldMatrix3x4Array[blendIdx[3]];
	weight = blendWgt[3];

	blendPos += float4(mul(worldMatrix, input.position).xyz, 1.0) * weight;
#endif
	//---------------End Skinning Unrolled Loop------------------

	// Transform the current vertex from object space to clip space
	output.position = mul(viewProjectionMatrix, blendPos);
	output.texCoords = input.texCoords;
}

//----------------------------------
//FeedbackBufferFP
//Outputs the following:
// R,G - U, V coord
// B - SpaceId
// A - mipLevel
//----------------------------------
float4 FeedbackBufferFP
(
in v2fFeedback input,

uniform float2 virtTexSize,
uniform float mipSampleBias,
uniform float spaceId
) : SV_TARGET
{
	float4 result;
	result.rg = input.texCoords.xy;

	float mipLevel = texMipLevel(input.texCoords.xy, virtTexSize) + mipSampleBias;
	mipLevel = clamp(mipLevel, 0.0, 15.0); //Could clamp this smaller, remember a higher mip index is a smaller actual texture (full size is 0)
	result.b = mipLevel / 255.0;

	result.a = spaceId / 255.0;

	return result;
}