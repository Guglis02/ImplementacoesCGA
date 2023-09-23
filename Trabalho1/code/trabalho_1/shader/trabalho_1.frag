// Fragment Shader

#version 400

uniform sampler2D colorTextureSampler;
uniform vec3 LightPos1;
uniform vec3 LightPos2;
uniform vec3 CameraPosition;
uniform mat4 MVP;

in vec2 gsTexCoord;
in vec3 Normal;
in vec3 gsFragPos;

vec3 toClipSpace(vec3 pos)
{
	return (MVP * vec4(pos, 1.0)).xyz;
}

vec3 calculateSpecular(vec3 lightPos, vec3 color, vec3 viewDir, float shininess)
{
    lightPos = toClipSpace(lightPos);
    vec3 lightDir = normalize(lightPos - gsFragPos);
    vec3 halfwayDir = normalize(lightDir + viewDir);
    
    float specIntensity = pow(max(dot(Normal, halfwayDir), 0.0), shininess);
    
    vec3 specular = color * specIntensity;
    return specular;
}

vec3 calculateDiffuse(vec3 lightPos, vec3 color)
{
    lightPos = toClipSpace(lightPos);
	vec3 lightDir = normalize(lightPos - gsFragPos);
	float intensity = max(dot(Normal, lightDir), 0.0);
	vec3 diffuse = color * intensity;
	return diffuse;
}

void main()
{
	float ambientStrength = 0.3;
	vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);

	vec3 objectColor = texture(colorTextureSampler, gsTexCoord).rgb;
	//vec3 objectColor = vec3(1.0, 1.0, 1.0);
	
	vec3 diffuse1 = calculateDiffuse(LightPos1, vec3(1.0, 0.4, 0.1));
	vec3 diffuse2 = calculateDiffuse(LightPos2, vec3(0.5, 0.5, 0.6));

    vec3 viewDir = normalize(toClipSpace(CameraPosition) - gsFragPos);

    float shininessValue = 32.0;

    vec3 specular1 = calculateSpecular(LightPos1, vec3(1.0, 1.0, 1.0), viewDir, shininessValue);
    vec3 specular2 = calculateSpecular(LightPos2, vec3(1.0, 1.0, 1.0), viewDir, shininessValue);

    vec3 result = (ambient + diffuse1 + diffuse2 + specular1 + specular2) * objectColor;

    gl_FragColor = vec4(result, 1.0);
}