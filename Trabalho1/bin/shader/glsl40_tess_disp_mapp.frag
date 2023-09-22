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

	intensity = max(dot(teLightDir, teNormal), 0.0); 
	
	if(intensity > 1)
	{
		color = vec4(1.0, 1.0, 1.0, 1.0);
	}

	//color = texture(colorTextureSampler, teTexCoord) * vec4(intensity, intensity, intensity, 1.0);
	/*
	if (intensity > 0.98)
		color = vec4(Color,1.0) * vec4(0.9,0.9,0.9,1.0);
	else if (intensity > 0.5)
		color = vec4(Color,1.0) * vec4(0.4,0.4,0.4,1.0);	
	else if (intensity > 0.25)
		color = vec4(Color,1.0) * vec4(0.2,0.2,0.2,1.0);
	else
		color = vec4(Color,1.0) * vec4(0.1,0.1,0.1,1.0);	
		*/
	//color = vec4(Color,1.0) * intensity;
	gl_FragColor = color;
}