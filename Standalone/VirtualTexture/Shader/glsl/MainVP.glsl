#version 120

uniform mat4 worldViewProj;	//The world view projection matrix
uniform vec3 eyePosition;		//The eye position
uniform vec4 lightPosition;		//The position of the light in object space
uniform vec4 lightAttenuation;		//The attenuation of the light, Should this be uniform?

//Input
attribute vec4 vertex; //Vertex
attribute vec2 uv0; //Uv Coord
#ifdef PARITY
	attribute vec4 tangent; //Tangent
#else
	attribute vec3 tangent; //Tangent
#endif
attribute vec3 binormal; //Binormal
attribute vec3 normal; //Normal

//Output
varying vec2 texCoords;
varying vec3 lightVector; //Light vector in tangent space
varying vec3 halfVector; //Eye vector in tangent space
varying vec4 attenuation; //Attenuation per vertex

//External Functions
vec3 pack(vec3 toPack);

//----------------------------------
//Shared Vertex Program
//----------------------------------
void main(void)
{
	//Tangent space conversion matrix
#ifdef PARITY
	mat3 TBNMatrix = mat3(tangent.xyz, binormal * tangent.w, normal);
#else
	mat3 TBNMatrix = mat3(tangent, binormal, normal);
#endif

	//Calculate the local light vector
	lightVector = lightPosition.xyz - vertex.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(lightVector);
	attenuation = vec4(1.0 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1.0));

	//Calculate the half vector
	lightVector = normalize(lightVector);
	halfVector = normalize(eyePosition - vertex.xyz) + lightVector;

	//Transform the light vector from object space into tangent space
	lightVector = lightVector * TBNMatrix;
	lightVector = pack(lightVector);
	
	//Transform the eye vector to tangent space
	halfVector = halfVector * TBNMatrix;
	halfVector = pack(halfVector);
	
	//Copy the texture coords
	texCoords = uv0;

	// Transform the current vertex from object space to clip space
	gl_Position = worldViewProj * vertex;
}