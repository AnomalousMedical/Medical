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

//Unpack function unpacks values from 0 to 1 to -1 to 1
float3 unpack(float3 input)
{
	return 2.0f * (input - 0.5);
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//Lighting function
//----------------------------------
float4 doLighting
(
		//Lighting and eye
	    float3 lightVector,
		float3 halfVector,
		float4 lightDiffuseColor,
		float4 attenuation,

		//Diffuse color
		float4 diffuseColor,

		//Specular color
		float4 specularColor,
		float specularAmount,
		float glossyness,

		//Emissive color
		float4 emissive,
		
		//Normal
		float3 normal
)
{
	//Nvidia cg shader book equation
	float3 N = normalize(normal);
	
	//Diffuse term
	float3 L = normalize(lightVector);
	float diffuseLight = saturate(dot(N, L));
	float3 diffuse = diffuseColor.rgb * lightDiffuseColor.rgb * diffuseLight;
	
	//Specular term
	float3 H = normalize(halfVector);
	float specularLight = pow(saturate(dot(N, H)), glossyness);
	float3 specular = specularAmount * (specularColor.rgb * lightDiffuseColor.rgb * specularLight);
	
	float4 finalColor;
	finalColor.rgb = diffuse + specular + emissive.rgb;
	finalColor.a = 1.0;
	
	return finalColor;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//Virtual Texture Coords
//----------------------------------

float2 vtexCoord(float2 address, Texture2D indirectionTex, SamplerState indirectionTexSampler, float2 physicalSizeRecip, float2 pageSizeLog2, float2 pagePaddingScale, float2 pagePaddingOffset)
{
	//Need to add bias for mip levels, the bias adjusts the size of the indirection texture to the size of the physical texture
	float4 redirectInfo = indirectionTex.SampleBias(indirectionTexSampler, address.xy, pageSizeLog2 - 0.5);

	float mip2 = floor(exp2(redirectInfo.b * 255.0) + 0.5); //Figure out how far to shift the original address, based on the mip level, highest mip level (1x1 indirection texture) is 0 counting up from there
	float2 coordLow = frac(address * mip2); //Get fractional part of page location, this is shifted left by the mip level

	float2 page = floor(redirectInfo.rg * 255.0 + 0.5); //Get the page number on the physical texture

	float2 finalCoord = page + coordLow * pagePaddingScale + pagePaddingOffset; //Add these together to get the coords in page space 64.0 / 66.0  1.0 / 66.0
	finalCoord = finalCoord * physicalSizeRecip; //Multiple by the physical texture page size to get back to uv space, that is our final coord
	
	return finalCoord;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//NormalMapSpecular
//----------------------------------
float4 normalMapSpecularFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float4 specularColor,				//The specular color of the surface
		uniform float glossyness,					//The glossyness of the surface
		uniform float4 emissiveColor			    //The emissive color of the surface
		#ifdef ALPHA
			,uniform float4 alpha
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t2),
			uniform SamplerState indirectionTexSampler : register(s2),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale, 
			uniform float2 pagePaddingOffset
#endif
			) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.Sample(colorTextureSampler, texCoords.xy);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.Sample(normalTextureSampler, texCoords).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularColor, colorMap.a, glossyness, emissiveColor, normal);
	#ifdef ALPHA
		color.a = alpha.a;
	#endif
	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//normalMapSpecularHighlightFP
//----------------------------------
float4 normalMapSpecularHighlightFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float4 specularColor,				//The specular color of the surface
		uniform float glossyness,					//The glossyness of the surface
		uniform float4 emissiveColor,			    //The emissive color of the surface
		uniform float4 highlightColor				//A color to multiply the final color by to create a highlight effect
		#ifdef ALPHA
			,uniform float4 alpha
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t2),
			uniform SamplerState indirectionTexSampler : register(s2),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale,
			uniform float2 pagePaddingOffset
#endif
			) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.Sample(colorTextureSampler, texCoords.xy);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.Sample(normalTextureSampler, texCoords).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularColor, colorMap.a, glossyness, emissiveColor, normal) * highlightColor;
	#ifdef ALPHA
		color.a = alpha.a;
	#endif
	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//normalMapSpecularOpacityMapFP
//----------------------------------
float4 normalMapSpecularOpacityMapFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform Texture2D opacityTexture : register(t2), //The Opacity map, uses r channel for opacity
		uniform SamplerState opacityTextureSampler : register(s2), //The Opacity map, uses r channel for opacity
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float4 specularColor,				//The specular color of the surface
		uniform float glossyness,					//The glossyness of the surface
		uniform float4 emissiveColor			    //The emissive color of the surface
		#ifdef ALPHA
			,uniform float4 alpha
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t3),
			uniform SamplerState indirectionTexSampler : register(s3),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale,
			uniform float2 pagePaddingOffset
#endif
			) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.Sample(colorTextureSampler, texCoords.xy);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.Sample(normalTextureSampler, texCoords).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularColor, colorMap.a, glossyness, emissiveColor, normal);

	float opacity = opacityTexture.Sample(opacityTextureSampler, texCoords).r;
#ifdef ALPHA
	color.a = opacity - (1.0f - alpha.a);
#else
	color.a = opacity;
#endif
	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//normalMapSpecularMapFP
//----------------------------------
float4 normalMapSpecularMapFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform Texture2D specularTexture : register(t2),  //The specular color map
		uniform SamplerState specularTextureSampler : register(s2),  //The specular color map
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float glossyness,					//The glossyness of the surface
		uniform float4 emissiveColor			    //The emissive color of the surface
		#ifdef ALPHA
			,uniform float4 alpha
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t3),
			uniform SamplerState indirectionTexSampler : register(s3),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale,
			uniform float2 pagePaddingOffset
#endif
			) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.Sample(normalTextureSampler, texCoords.xy);

	//Get the specular value
	float4 specularMapColor = specularTexture.Sample(specularTextureSampler, texCoords.xy);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.Sample(normalTextureSampler, texCoords).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularMapColor, specularMapColor.a, glossyness, emissiveColor, normal);
	#ifdef ALPHA
		color.a = alpha.a;
	#endif
	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//NormalMapSpecularMapGlossMapAlpha
//----------------------------------
float4 normalMapSpecularMapGlossMapFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform Texture2D specularTexture : register(t2),  //The specular color map
		uniform SamplerState specularTextureSampler : register(s2),  //The specular color map
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float4 emissiveColor,			    //The emissive color of the surface
		uniform float glossyStart,
		uniform float glossyRange
		#ifdef ALPHA
			,uniform float4 alpha
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t3),
			uniform SamplerState indirectionTexSampler : register(s3),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale,
			uniform float2 pagePaddingOffset
#endif
) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.SampleLevel(normalTextureSampler, texCoords, 0);

	//Get the specular value
	float4 specularMapColor = specularTexture.SampleLevel(specularTextureSampler, texCoords, 0);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.SampleLevel(normalTextureSampler, texCoords, 0).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	//Compute the glossyness
	float glossyness = glossyStart + glossyRange * 1;// colorMap.a;

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularMapColor, specularMapColor.a, glossyness, emissiveColor, normal);
	#ifdef ALPHA
		color.a = alpha.a;
	#endif

	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//normalMapSpecularMapOpacityMapFP
//----------------------------------
float4 normalMapSpecularMapOpacityMapFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform Texture2D specularTexture : register(t2),  //The specular color map
		uniform SamplerState specularTextureSampler : register(s2),  //The specular color map
		uniform Texture2D opacityTexture : register(t3), //The Opacity map, uses r channel for opacity
		uniform SamplerState opacityTextureSampler : register(s3), //The Opacity map, uses r channel for opacity
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float glossyness,					//The glossyness of the surface
		uniform float4 emissiveColor			    //The emissive color of the surface
		#ifdef ALPHA
			,uniform float4 alpha
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t4),
			uniform SamplerState indirectionTexSampler : register(s4),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale,
			uniform float2 pagePaddingOffset
#endif
			) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.Sample(colorTextureSampler, texCoords.xy);

	//Get the specular value
	float4 specularMapColor = specularTexture.Sample(specularTextureSampler, texCoords.xy);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.Sample(normalTextureSampler, texCoords).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularMapColor, specularMapColor.a, glossyness, emissiveColor, normal);

	float opacity = opacityTexture.Sample(opacityTextureSampler, texCoords).r;
	#ifdef ALPHA
		color.a = opacity - (1.0f - alpha.a);
	#else
		color.a = opacity;
	#endif
	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//normalMapSpecularMapOpacityMapGlossMapFP
//----------------------------------
float4 normalMapSpecularMapOpacityMapGlossMapFP
(
	    in v2f input,

		uniform Texture2D normalTexture : register(t0),	//The normal map
		uniform SamplerState normalTextureSampler : register(s0),	//The normal map
		uniform Texture2D colorTexture : register(t1),  //The color map
		uniform SamplerState colorTextureSampler : register(s1),  //The color map
		uniform Texture2D specularTexture : register(t2),  //The specular color map
		uniform SamplerState specularTextureSampler : register(s2),  //The specular color map
		uniform Texture2D opacityGlossTexture : register(t3), //The Opacity map, uses r channel for opacity
		uniform SamplerState opacityGlossTextureSampler : register(s3), //The Opacity map, uses r channel for opacity
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float4 emissiveColor,				//The emissive color of the surface
		uniform float glossyStart,
		uniform float glossyRange
		#ifdef ALPHA
			,uniform float4 alpha						//The total opacity of the object.
		#endif
#ifdef VIRTUAL_TEXTURE
			, uniform Texture2D indirectionTex : register(t4),
			uniform SamplerState indirectionTexSampler : register(s4),
			uniform float2 physicalSizeRecip,
			uniform float2 pageSizeLog2,
			uniform float2 pagePaddingScale,
			uniform float2 pagePaddingOffset
#endif
			) : SV_TARGET
{
	float2 texCoords = input.texCoords;

#ifdef VIRTUAL_TEXTURE
	texCoords = vtexCoord(texCoords, indirectionTex, indirectionTexSampler, physicalSizeRecip, pageSizeLog2, pagePaddingScale, pagePaddingOffset);
#endif

	//Get color value
	float4 colorMap = colorTexture.Sample(colorTextureSampler, texCoords.xy);

	//Get the specular value
	float4 specularMapColor = specularTexture.Sample(specularTextureSampler, texCoords.xy);

	//Unpack the normal map.
	float3 normal;
	normal.rg = 2.0f * (normalTexture.Sample(normalTextureSampler, texCoords).ag - 0.5f);
	normal.b = sqrt(1 - normal.r * normal.r - normal.g * normal.g);

	float2 opacityGloss = opacityGlossTexture.Sample(opacityGlossTextureSampler, texCoords).rg;

	//Compute the glossyness
	float glossyness = glossyStart + glossyRange * opacityGloss.g;

	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, colorMap, specularMapColor, specularMapColor.a, glossyness, emissiveColor, normal);
	
	#ifdef ALPHA
		color.a = opacityGloss.r - (1.0f - alpha.a);
	#else
		color.a = opacityGloss.r;
	#endif
	return color;
}

//-------------------------------------------------------------------------------------------------

//----------------------------------
//NoTexturesColoredFP
//----------------------------------
float4 NoTexturesColoredFP
(
	    in v2fNoTexture input,

		uniform float4 diffuseColor,				//The diffuse color of the object
		uniform float4 lightDiffuseColor,			//The diffuse color of the light source
		uniform float4 specularColor,				//The specular color of the surface
		uniform float glossyness,					//The glossyness of the surface
		uniform float4 emissiveColor				//The emissive color of the surface
		#ifdef ALPHA
			,uniform float4 alpha						//The total opacity of the object.
		#endif
) : SV_TARGET
{
	float4 color = doLighting(unpack(input.lightVector), unpack(input.halfVector), lightDiffuseColor, input.attenuation, diffuseColor, specularColor, diffuseColor.a, glossyness, emissiveColor, input.normal);
	#ifdef ALPHA
		color.a = alpha.a;
	#endif
	return color;
}