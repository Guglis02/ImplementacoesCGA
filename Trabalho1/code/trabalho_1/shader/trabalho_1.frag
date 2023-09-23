// Fragment Shader

#version 400

uniform sampler2D colorTextureSampler;
uniform vec3 LightPos1;
uniform vec3 LightPos2;
uniform mat4 MVP;

in vec2 gsTexCoord;
in vec3 Normal;
in vec3 gsFragPos;

vec3 calculateDiffuse(vec3 lightPos, vec3 color)
{
	vec3 lightDir = normalize((MVP * vec4(lightPos, 1.0)).xyz - gsFragPos);
	float intensity = max(dot(Normal, lightDir), 0.0);
	vec3 diffuse = color * intensity;
	return diffuse;
}

void main()
{
	float ambientStrength = 0.1;
	vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);

	vec3 objectColor = texture(colorTextureSampler, gsTexCoord).rgb;
	
	vec3 diffuse1 = calculateDiffuse(LightPos1, vec3(1.0, 0.0, 0.0));
	vec3 diffuse2 = calculateDiffuse(LightPos2, vec3(1.0, 1.0, 1.0));

	vec3 result = (ambient + diffuse1 + diffuse2) * objectColor;

	gl_FragColor = vec4(result, 1.0);
}