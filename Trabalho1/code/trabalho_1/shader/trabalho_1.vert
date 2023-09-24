// Vertex Shader
// 
// Aqui, o vertex shader apenas passa os dados para o tcs.

#version 400

layout(location = 0) in vec3 VertexPosition;
layout(location = 1) in vec2 TexCoord;

out vec2 vTexCoord;
out vec4 vPosition;

void main()
{
	vTexCoord = TexCoord;
	vPosition = vec4(VertexPosition, 1.0);
}
