#version 120

uniform mat4 worldViewProj;	//The world view projection matrix

//Input
attribute vec4 vertex; //Vertex

//----------------------------------
//Depth Check Vertex Program
//----------------------------------
void main(void)
{
	// Transform the current vertex from object space to clip space
	gl_Position = worldViewProj * vertex;
}