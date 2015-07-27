struct vertin
{
	float4   position  : POSITION;
	float3   normal    : NORMAL;
};

struct vertout
{
	float4   oPosition : SV_POSITION;
	float4   oColor    : COLOR0;
	float3   objectPos : TEXCOORD0;
	float3   normal    : TEXCOORD1;
};

//Vertex program
vertout main_vp(vertin input,
             uniform float4x4 worldViewProj,
             uniform float4 color)	
{
	vertout output;
	
	output.oPosition = mul(worldViewProj, input.position);
	output.oColor.rgb = float3(color.r, color.g, color.b);
	output.oColor.a = 1.0;
	output.objectPos = input.position.xyz;
	output.normal = input.normal;

	return output;
}

//Fragment program, no alpha
float4 main_fp(vertout input,
				const uniform float3 lightColor,
				const uniform float3 lightPosition) : SV_TARGET	
{
	float3 P = input.objectPos.xyz;
	float3 N = normalize(input.normal);

	float3 L = normalize(lightPosition - P);
	float diffuseLight = max(dot(N, L), 0);

	float4 finalLight;
	finalLight.xyz = lightColor * diffuseLight;
	finalLight.w = 1.0;

	return input.oColor * finalLight;
}

//Fragment program, alpha
float4 main_fp_alpha(vertout input,
				const uniform float3 lightColor,
				const uniform float3 lightPosition,
				uniform float4 alpha
				) : SV_TARGET	
{
	float3 P = input.objectPos.xyz;
	float3 N = normalize(input.normal);

	float3 L = normalize(lightPosition - P);
	float diffuseLight = max(dot(N, L), 0);

	float4 finalLight;
	finalLight.xyz = lightColor * diffuseLight;
	finalLight.w = 1.0;

	float4 finalColor = input.oColor * finalLight;
	finalColor.a = alpha.a;
	return finalColor;
}
