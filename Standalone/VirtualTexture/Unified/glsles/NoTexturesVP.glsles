precision highp float;

uniform mat4 worldViewProj;	//The world view projection matrix
uniform vec3 eyePosition;		//The eye position
uniform vec4 lightPosition;		//The position of the light in object space
uniform vec4 lightAttenuation;		//The attenuation of the light, Should this be uniform?

//Input
attribute vec4 vertex; //Vertex
attribute vec3 normal; //Normal

//Output
varying vec3 passNormal;
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
	//Calculate the local light vector
	lightVector = lightPosition.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(lightVector);
	attenuation = vec4(1.0 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1.0));

	//Calculate the half vector
	halfVector = eyePosition + lightVector;
	
	//Copy the texture coords
	passNormal = normal;

	// Transform the current vertex from object space to clip space
	gl_Position = worldViewProj * vertex;
}