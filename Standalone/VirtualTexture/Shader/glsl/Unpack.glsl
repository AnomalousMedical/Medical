#version 120

//Unpack function unpacks values from 0 to 1 to -1 to 1
vec3 unpack(vec3 toUnpack)
{
	return 2.0 * (toUnpack - 0.5);
}