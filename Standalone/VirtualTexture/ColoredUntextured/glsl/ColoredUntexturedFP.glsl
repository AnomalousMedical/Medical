#version 120

uniform vec3 lightColor;
uniform vec4 lightPosition;

//Output
varying vec4 passColor;
varying vec3 objectPos; //Light vector in tangent space
varying vec3 passNormal; //Eye vector in tangent space

//----------------------------------
//Shared Vertex Program
//----------------------------------
void main(void)
{
    vec3 P = objectPos.xyz;
	vec3 N = normalize(passNormal);

	vec3 L = normalize(lightPosition.xyz - P);
	float diffuseLight = max(dot(N, L), 0.0);

	vec4 finalLight;
	finalLight.xyz = lightColor * diffuseLight;
	finalLight.w = 1.0;

	gl_FragColor = passColor * finalLight;
}