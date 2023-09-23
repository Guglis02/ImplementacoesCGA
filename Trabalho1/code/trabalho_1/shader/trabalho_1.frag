// Fragment Shader

#version 400

uniform sampler2D colorTextureSampler;
uniform vec3 LightDir;

in vec2 gsTexCoord;

void main()
{
	float ambientStrength = 0.2;
	vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);

	vec3 objectColor = texture(colorTextureSampler, gsTexCoord).rgb;
	vec3 result = ambient * objectColor;

	gl_FragColor = vec4(result, 1.0);
}