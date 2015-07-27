#version 120

//Parameters
uniform vec4 lightDiffuseColor;			//The diffuse color of the light source
uniform float glossyness;					//The glossyness of the surface
uniform vec4 emissiveColor;				//The emissive color of the surface

//----------------------------Common----------------------------
#ifdef ALPHA
	uniform vec4 alpha;
#endif

//Textures
uniform sampler2D normalTexture;	//The normal map
uniform sampler2D colorTexture;  //The color map
uniform sampler2D specularTexture;  //The specular color map
uniform sampler2D opacityTexture; //The Opacity map, uses r channel for opacity

//Vertex shader output
varying vec2 texCoords;
varying vec3 lightVector; //Light vector in tangent space
varying vec3 halfVector; //Eye vector in tangent space
varying vec4 attenuation; //Attenuation per vertex

//External function prototypes
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
);

vec3 unpack(vec3 toUnpack);

//----------------------------Main Shader----------------------------

void main()
{
	//Get color value
	vec4 colorMap = texture2D(colorTexture, texCoords.xy);

	//Get the specular value
	vec4 specularMapColor = texture2D(specularTexture, texCoords.xy);

	//Unpack the normal map.
	vec3 normal;
	normal.rg = 2.0 * (texture2D(normalTexture, texCoords).ag - 0.5);
	normal.b = sqrt(1.0 - normal.r * normal.r - normal.g * normal.g);

	gl_FragColor = doLighting(unpack(lightVector), unpack(halfVector), lightDiffuseColor, attenuation, colorMap, specularMapColor, specularMapColor.a, glossyness, emissiveColor, normal);

#ifdef ALPHA
	gl_FragColor.a = texture2D(opacityTexture, texCoords).r - (1.0 - alpha.a);
#else
	gl_FragColor.a = texture2D(opacityTexture, texCoords).r;
#endif
}