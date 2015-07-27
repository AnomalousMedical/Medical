#version 120

//----------------------------------
//Lighting function
//----------------------------------
vec4 doLighting
(
		//Lighting and eye
	    vec3 lightVector,
		vec3 halfVector,
		vec4 lightDiffuseColor,
		vec4 attenuation,

		//Diffuse color
		vec4 diffuseColor,

		//Specular color
		vec4 specularColor,
		float specularAmount,
		float glossyness,

		//Emissive color
		vec4 emissive,
		
		//Normal
		vec3 normal
)
{
	//Nvidia cg shader book equation
	vec3 N = normalize(normal);
	
	//Diffuse term
	vec3 L = normalize(lightVector);
	float diffuseLight = clamp(dot(N, L), 0.0, 1.0);
	vec3 diffuse = diffuseColor.rgb * lightDiffuseColor.rgb * diffuseLight;
	
	//Specular term
	vec3 H = normalize(halfVector);
	float specularLight = pow(clamp(dot(N, H), 0.0, 1.0), glossyness);
	vec3 specular = specularAmount * (specularColor.rgb * lightDiffuseColor.rgb * specularLight);
	
	vec4 finalColor;
	finalColor.rgb = diffuse + specular + emissive.rgb;
	finalColor.a = 1.0;
	
	return finalColor;
}