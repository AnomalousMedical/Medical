precision highp float;

//Parameters
uniform vec4 lightDiffuseColor;			//The diffuse color of the light source
uniform vec4 specularColor;				//The specular color of the surface
uniform float glossyness;				//The glossyness of the surface
uniform vec4 emissiveColor;				//The emissive color of the surface
uniform vec4 diffuseColor;              //The diffuse color of the surface

//----------------------------Common----------------------------
#ifdef ALPHA
	uniform vec4 alpha;
#endif

//Vertex shader output
varying vec3 passNormal;
varying vec3 lightVector; //Light vector in tangent space
varying vec3 halfVector; //Eye vector in tangent space
varying vec4 attenuation; //Attenuation per vertex

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

//Unpack function unpacks values from 0 to 1 to -1 to 1
vec3 unpack(vec3 packValue)
{
	return 2.0 * (packValue - 0.5);
}

//----------------------------Main Shader----------------------------

void main()
{
	gl_FragColor = doLighting(unpack(lightVector), unpack(halfVector), lightDiffuseColor, attenuation, diffuseColor, specularColor, diffuseColor.a, glossyness, emissiveColor, passNormal);

	#ifdef ALPHA
		gl_FragColor.a = alpha.a;
	#endif
}