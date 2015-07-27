//----------------------------------
//Depth Check Vertex Program
//----------------------------------
float4 depthCheckVP
(
	float4 position : POSITION,

	const uniform float4x4 worldViewProj	//The world view projection matrix
) : SV_POSITION
{
	return mul(worldViewProj, position);
}

//----------------------------------
//Depth Check with Hardware Skinning and (optional) Pose
//----------------------------------
float4 depthCheckSkinPose
(
		float4 position : POSITION,

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
) : SV_POSITION
{
	//Hardware Pose Animation
	#if POSE_COUNT == 1
		position.xyz = position.xyz + poseAnimAmount.x * pose1pos;
	#elif POSE_COUNT == 2
		position.xyz = position.xyz + poseAnimAmount.x * pose1pos + poseAnimAmount.y * pose2pos;
	#endif

	//Hardware Skinning
	//-----------Skinning Unrolled Loop--------------
	float4 blendPos = float4(0,0,0,0);
	float3x4 worldMatrix;
	float weight;

	//First Bone
	#if BONES_PER_VERTEX > 0
		worldMatrix = worldMatrix3x4Array[blendIdx[0]];
		weight = blendWgt[0];

		blendPos += float4(mul(worldMatrix, position).xyz, 1.0) * weight;
	#endif

	//Second Bone
	#if BONES_PER_VERTEX > 1
		worldMatrix = worldMatrix3x4Array[blendIdx[1]];
		weight = blendWgt[1];

		blendPos += float4(mul(worldMatrix, position).xyz, 1.0) * weight;
	#endif

	//Third Bone
	#if BONES_PER_VERTEX > 2
		worldMatrix = worldMatrix3x4Array[blendIdx[2]];
		weight = blendWgt[2];

		blendPos += float4(mul(worldMatrix, position).xyz, 1.0) * weight;
	#endif

	//Fourth Bone
	#if BONES_PER_VERTEX > 3
		worldMatrix = worldMatrix3x4Array[blendIdx[3]];
		weight = blendWgt[3];

		blendPos += float4(mul(worldMatrix, position).xyz, 1.0) * weight;
	#endif
	//---------------End Skinning Unrolled Loop------------------

	// Transform the current vertex from object space to clip space
	return mul(viewProjectionMatrix, blendPos);
}