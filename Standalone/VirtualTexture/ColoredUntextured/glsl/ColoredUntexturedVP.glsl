#version 120

uniform mat4 worldViewProj;	//The world view projection matrix
uniform vec4 color;

//Input
attribute vec4 vertex; //Vertex
attribute vec3 normal; //Normal

//Output
varying vec4 passColor;
varying vec3 objectPos; //Light vector in tangent space
varying vec3 passNormal; //Eye vector in tangent space

//----------------------------------
//Shared Vertex Program
//----------------------------------
void main(void)
{
	passColor.rgb = color.rgb;
	passColor.a = 1.0;
	objectPos = vertex.xyz;
	passNormal = normal;

	// Transform the current vertex from object space to clip space
	gl_Position = worldViewProj * vertex;
}