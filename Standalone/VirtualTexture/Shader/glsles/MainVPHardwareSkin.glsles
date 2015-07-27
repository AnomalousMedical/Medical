precision highp float;

uniform vec3 worldEyePosition;		//The eye position
uniform vec4 worldLightPosition;		//The position of the light in object space
uniform vec4 lightAttenuation;		//The attenuation of the light, Should this be uniform?

uniform vec4 worldMatrix3x4Array[90]; //This is an array of bones, the index is the maximum amount of bones supported * 3
uniform mat4 viewProjectionMatrix;

attribute vec4 blendIndices;
attribute vec4 blendWeights;

//Input
attribute vec4 vertex; //Vertex
attribute vec2 uv0; //Uv Coord
#ifdef PARITY
	attribute vec4 tangent; //Tangent
#else
	attribute vec3 tangent; //Tangent
#endif
attribute vec3 normal; //Normal

//Pose Animation
#if POSE_COUNT > 0
	attribute vec3 uv1; //Pose1Pos
	#if POSE_COUNT > 1
		attribute vec3 uv2; //Pose2Pos
	#endif
	uniform vec4 poseAnimAmount;
#endif

//Output
varying vec2 texCoords;
varying vec3 lightVector; //Light vector in tangent space
varying vec3 halfVector; //Eye vector in tangent space
varying vec4 attenuation; //Attenuation per vertex

//Pack function packs values from -1 to 1 to 0 and 1
vec3 pack(vec3 packValue)
{
	return 0.5 * packValue + 0.5;
}

//----------------------------------
//Shared Vertex Program
//----------------------------------
void main(void)
{
	vec4 poseVertex = vertex;
	//Hardware Pose Animation
	#if POSE_COUNT == 1
		poseVertex.xyz = poseVertex.xyz + poseAnimAmount.x * uv1;
	#else //elsif not working, so do it this way instead
		#if POSE_COUNT == 2
			poseVertex.xyz = poseVertex.xyz + poseAnimAmount.x * uv1 + poseAnimAmount.y * uv2;
		#endif
	#endif

	//Hardware Skinning
	vec4 blendPos = vec4(0,0,0,0);
    vec3 newNormal = vec3(0,0,0);
    vec3 newTangent = vec3(0,0,0);
	//-----------Skinning Unrolled Loop--------------
	//Remove blocks for bones that are not needed
        int idx;
        mat4 worldMatrix;
        float weight;
        mat3 worldMatrixRot; //Rotation only matrix, prevents translation from screwing up normals

		//First Bone
		#if BONES_PER_VERTEX > 0
			idx = int(blendIndices[0]) * 3; //Have to multiply by 3 since we are taking vec4 instead of matrices
			worldMatrix[0] = worldMatrix3x4Array[idx];
      		worldMatrix[1] = worldMatrix3x4Array[idx + 1];
      		worldMatrix[2] = worldMatrix3x4Array[idx + 2];
      		worldMatrix[3] = vec4(0.0);
			weight = blendWeights[0];
			worldMatrixRot = mat3(worldMatrix);
        
			blendPos += vec4((worldMatrix * poseVertex).xyz, 1.0) * weight;
			newNormal += (worldMatrixRot * normal) * weight;
			newTangent += (worldMatrixRot * tangent.xyz) * weight;
		#endif

		//Second Bone
		#if BONES_PER_VERTEX > 1
			idx = int(blendIndices[1]) * 3; //Have to multiply by 3 since we are taking vec4 instead of matrices
			worldMatrix[0] = worldMatrix3x4Array[idx];
      		worldMatrix[1] = worldMatrix3x4Array[idx + 1];
      		worldMatrix[2] = worldMatrix3x4Array[idx + 2];
      		worldMatrix[3] = vec4(0.0);
			weight = blendWeights[1];
			worldMatrixRot = mat3(worldMatrix);
        
			blendPos += vec4((worldMatrix * poseVertex).xyz, 1.0) * weight;
			newNormal += (worldMatrixRot * normal) * weight;
			newTangent += (worldMatrixRot * tangent.xyz) * weight;
		#endif

		//Third Bone
		#if BONES_PER_VERTEX > 2
			idx = int(blendIndices[2]) * 3; //Have to multiply by 3 since we are taking vec4 instead of matrices
			worldMatrix[0] = worldMatrix3x4Array[idx];
      		worldMatrix[1] = worldMatrix3x4Array[idx + 1];
      		worldMatrix[2] = worldMatrix3x4Array[idx + 2];
      		worldMatrix[3] = vec4(0.0);
			weight = blendWeights[2];
			worldMatrixRot = mat3(worldMatrix);
        
			blendPos += vec4((worldMatrix * poseVertex).xyz, 1.0) * weight;
			newNormal += (worldMatrixRot * normal) * weight;
			newTangent += (worldMatrixRot * tangent.xyz) * weight;
		#endif

		//Fourth Bone
		#if BONES_PER_VERTEX > 3
			idx = int(blendIndices[3]) * 3; //Have to multiply by 3 since we are taking vec4 instead of matrices
			worldMatrix[0] = worldMatrix3x4Array[idx];
      		worldMatrix[1] = worldMatrix3x4Array[idx + 1];
      		worldMatrix[2] = worldMatrix3x4Array[idx + 2];
      		worldMatrix[3] = vec4(0.0);
			weight = blendWeights[3];
			worldMatrixRot = mat3(worldMatrix);
        
			blendPos += vec4((worldMatrix * poseVertex).xyz, 1.0) * weight;
			newNormal += (worldMatrixRot * normal) * weight;
			newTangent += (worldMatrixRot * tangent.xyz) * weight;
		#endif
	//---------------End Skinning Unrolled Loop------------------

	newNormal = normalize(newNormal);
	newTangent = normalize(newTangent);
	vec3 newBinormal = cross(newTangent.xyz, newNormal.xyz);
	
    //Normal Map

	//Tangent space conversion matrix
#ifdef PARITY
	mat3 TBNMatrix = mat3(newTangent.xyz, newBinormal * tangent.w, newNormal.xyz);
#else
	mat3 TBNMatrix = mat3(newTangent, newBinormal, newNormal.xyz);
#endif

	//Calculate the local light vector
	lightVector = worldLightPosition.xyz - blendPos.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(lightVector);
	attenuation = vec4(1.0 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1.0));

	//Calculate the half vector
	lightVector = normalize(lightVector);
	halfVector = normalize(worldEyePosition - blendPos.xyz) + lightVector;

	//Transform the light vector from object space into tangent space
	lightVector = lightVector * TBNMatrix;
	lightVector = pack(lightVector);
	
	//Transform the eye vector to tangent space
	halfVector = halfVector * TBNMatrix;
	halfVector = pack(halfVector);
	
	//Copy the texture coords
	texCoords = uv0;

	// Transform the current vertex from object space to clip space
	gl_Position = viewProjectionMatrix * blendPos;
}