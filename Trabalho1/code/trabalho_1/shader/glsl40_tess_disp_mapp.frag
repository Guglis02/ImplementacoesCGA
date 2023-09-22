// Fragment Shader
//
// Este Fragment Shader amostra a textura com id 'colorTextureSampler' e aplicada 
// esta amostra ao fragmento.
// Nenhum cálculo de iluminação é feito.
//
// Abril 2016 - Alex Frasson - afrasson@inf.ufsm.br

#version 400

uniform sampler2D colorTextureSampler;
uniform vec3 LightDir;

in vec2 teTexCoord;
in vec3 teNormal;


void main()
{
	float intensity;
	vec3 Color = vec3(1.0f, 0.0f, 0.0f);
	vec4 color;

	intensity = max(dot(LightDir, teNormal), 1.0); 
	
	color = vec4(Color,1.0) * intensity;
	gl_FragColor = color;
}