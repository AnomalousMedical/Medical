#version 120

uniform mat4 worldViewProj;	//The world view projection matrix
uniform vec3 eyePosition;		//The eye position
uniform vec4 lightPosition;		//The position of the light in object space
uniform vec4 lightAttenuation;		//The attenuation of the light, Should this be uniform?

//Input
attribute vec4 vertex; //Vertex
attribute vec3 normal; //Normal

//Output
varying vec3 lightVector; //Light vector in tangent space
varying vec3 eyeVec; //Eye vector in tangent space
varying vec4 attenuation; //Attenuation per vertex
varying vec3 passNormal;

vec3 pack(vec3 toPack)
{
	return 0.5 * toPack + 0.5;
}

//----------------------------------
//Eye Outer VP
//----------------------------------
void main(void)
{
    	//Calculate the local light vector
	lightVector = lightPosition.xyz - vertex.xyz;
	
	//Calculate the light attenuation, ax^2 + bx + c
	float dist = length(lightVector);
	attenuation = vec4(1.0 / max((lightAttenuation.y + (dist * lightAttenuation.z) + (dist * dist * lightAttenuation.w)), 1.0));

	//Transform the light vector from object space into tangent space
	lightVector = normalize(lightVector);
	lightVector = pack(lightVector);
	
	//Transform the eye vector to vertex space
	eyeVec = eyePosition - vertex.xyz;
	
	//Transform the eye vector to tangent space
	eyeVec = normalize(eyeVec);
	eyeVec = pack(eyeVec);

	passNormal = normal;

	// Transform the current vertex from object space to clip space
	gl_Position = worldViewProj * vertex;
}