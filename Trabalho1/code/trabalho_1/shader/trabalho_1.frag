// Fragment Shader

#version 400

uniform sampler2D colorTextureSampler;
uniform vec3 LightPos;
uniform mat4 MVP;

in vec2 gsTexCoord;
in vec3 Normal;
in vec3 gsFragPos;

void main()
{
	float ambientStrength = 0.1;
	vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);

	vec3 objectColor = texture(colorTextureSampler, gsTexCoord).rgb;
	
	vec3 lightDir = normalize((MVP * vec4(LightPos, 1.0)).xyz - gsFragPos);

	float intensity = max(dot(Normal, lightDir), 0.0);
	vec3 diffuse = vec3(1.0, 1.0, 1.0) * intensity;
	
	vec3 result = (ambient + diffuse) * objectColor;

	gl_FragColor = vec4(result, 1.0);
}