//Application to vertex program
struct a2v
{
	//Input position
	float4 position : POSITION;

	//Input normal
	float3 normal : NORMAL0;
};

//Vertex program to fragment program struct
struct v2f
{
	//Input position
	float4 position : SV_POSITION;

	//Light vector in tangent space
	float3 lightVector : TEXCOORD2;

	//Eye vector in tangent space
	float3 eyeVec : TEXCOORD3;

	//Attenuation per vertex
	float4 attenuation : TEXCOORD4;
	
	//Normal
	float3 normal : TEXCOORD5;
};

//Pack function packs values from -1 to 1 to 0 and 1
float3 pack(float3 input)
{
	return 0.5 * input + 0.5;
}

//Unpack function unpacks values from 0 to 1 to -1 to 1
float3 unpack(float3 input)
{
	return 2.0f * (input - 0.5);
}

//-------------------------------------------------------------------------------------------------

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
	//Calculate the local light vector
	output.lightVector = lightPosition.xyz - input.position.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(output.lightVector);
	output.attenuation = 1 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1);

	//Transform the light vector from object space into tangent space
	output.lightVector = normalize(output.lightVector);
	output.lightVector = pack(output.lightVector);
	
	//Transform the eye vector to vertex space
	output.eyeVec = eyePosition - input.position;
	
	//Transform the eye vector to tangent space
	output.eyeVec = normalize(output.eyeVec);
	output.eyeVec = pack(output.eyeVec);

	output.normal = input.normal;

	// Transform the current vertex from object space to clip space
	output.position = mul(worldViewProj, input.position);
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//EyeOuterFP
//----------------------------------
float4 eyeOuterFP
(
	    in v2f input,

		uniform float4 lightDiffuseColor,
		uniform float4 specularColor				//The specular color of the surface
) : SV_TARGET
{
	float4 color;

	float4 diffuseTest;
	diffuseTest.r = 0;
	diffuseTest.g = 0;
	diffuseTest.b = 0;
	diffuseTest.a = 1;

	float3 eyeVector = unpack(input.eyeVec);
	float3 lightVector = unpack(input.lightVector);
	float3 normal = normalize(input.normal);
	float3 attenuation = input.attenuation;

	//Perform specular calculation in blinn style
	float3 H = normalize(eyeVector + lightVector);
	float4 lighting = lit(dot(lightVector, normal), dot(H, normal), 500); //Hardcoded glossyness, cannot pass such a large value using opengl
	float4 blinn = diffuseTest * lighting.y + specularColor * lighting.z;

	//Return the final color
	color.rgb = lightDiffuseColor * blinn * attenuation;

	color.a = color.r;

	return color;
}

//----------------------------------
//EyeOuterFPAlpha
//----------------------------------
float4 eyeOuterFPAlpha
(
	    in v2f input,

		uniform float4 lightDiffuseColor,
		uniform float4 specularColor				//The specular color of the surface
) : SV_TARGET
{
	float4 color;

	float3 eyeVector = unpack(input.eyeVec);
	float3 lightVector = unpack(input.lightVector);
	float3 normal = unpack(input.normal);
	float3 attenuation = input.attenuation;

	//Perform specular calculation in blinn style
	float3 H = normalize(eyeVector + lightVector);
	float4 lighting = lit(dot(lightVector, normal), dot(H, normal), 500); //Hardcoded glossyness, cannot pass such a large value using opengl
	float4 blinn = lighting.y + specularColor * lighting.z;

	//Return the final color
	color.rgb = lightDiffuseColor * blinn * attenuation;

	color.a = 0.5f;

	return color;
}

//-------------------------------------------------------------------------------------------------