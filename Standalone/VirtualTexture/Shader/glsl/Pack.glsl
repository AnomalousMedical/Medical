#version 120

//Pack function packs values from -1 to 1 to 0 and 1
vec3 pack(vec3 toPack)
{
	return 0.5 * toPack + 0.5;
}