// Fragment Shader

#version 400

uniform sampler2D colorTextureSampler;
uniform vec3 LightPos;

in vec2 gsTexCoord;
in vec3 Normal;
in vec3 gsFragPos;

void main()
{
	float ambientStrength = 0.2;
	vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);

	vec3 objectColor = texture(colorTextureSampler, gsTexCoord).rgb;
	
	vec3 norm = normalize(Normal);
	vec3 lightDir = normalize(LightPos - gsFragPos);

	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = vec3(1.0, 1.0, 1.0) * diff;
	
	vec3 result = (ambient + diffuse) * objectColor;

	gl_FragColor = vec4(result, 1.0);
}