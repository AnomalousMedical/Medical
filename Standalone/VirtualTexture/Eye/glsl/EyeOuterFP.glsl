#version 120

uniform vec4 lightDiffuseColor;			//The diffuse color of the light source
uniform vec4 specularColor;				//The specular color of the surface

varying vec3 lightVector; //Light vector in tangent space
varying vec3 eyeVec; //Eye vector in tangent space
varying vec4 attenuation; //Attenuation per vertex
varying vec3 passNormal;

vec3 unpack(vec3 toUnpack)
{
	return 2.0 * (toUnpack - 0.5);
}

void main (void)
{
	vec4 diffuseTest;
	diffuseTest.r = 0.0;
	diffuseTest.g = 0.0;
	diffuseTest.b = 0.0;
	diffuseTest.a = 1.0;

	vec3 eyeVectorUp = unpack(eyeVec);
	vec3 lightVectorUp = unpack(lightVector);

	//Perform specular calculation in blinn style
	vec3 H = normalize(eyeVectorUp + lightVectorUp);
    float diffuse = dot(lightVectorUp, passNormal);
	float specular = pow(dot(H, passNormal), 500.0f); //Hardcoded glossyness, cannot pass such a large value using opengl
        
    vec4 blinn = diffuseTest * diffuse + specularColor * specular;

	//Return the final color
	gl_FragColor.rgb = lightDiffuseColor.xyz * blinn.xyz * attenuation.xyz;

#ifdef ALPHA
	gl_FragColor.a = 0.5f;
#else
	gl_FragColor.a = gl_FragColor.r;
#endif
}