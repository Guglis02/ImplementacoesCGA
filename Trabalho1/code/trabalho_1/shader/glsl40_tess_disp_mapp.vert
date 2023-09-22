// Vertex Shader
//
// Como as tranformções serão aplicadas somente no estágio Tessellation Evaluation Shader, este
// vertex shader apenas passa os vértices de entrada adiante para o Tessellation Control Shader.
//
// Abril 2016 - Alex Frasson - afrasson@inf.ufsm.br

#version 400

layout(location = 0) in vec3 VertexPosition;
layout(location = 1) in vec2 TexCoord;

out vec2 vTexCoord;
out vec4 vPosition;

uniform mat3 NormalMatrix;

uniform vec3 lightSource1;
uniform vec3 lightSource2;

out vec3 vNormal;

void main()
{
	vNormal = NormalMatrix * vPosition.xyz;

	vTexCoord = TexCoord;
	vPosition = vec4(VertexPosition, 1.0);
}
