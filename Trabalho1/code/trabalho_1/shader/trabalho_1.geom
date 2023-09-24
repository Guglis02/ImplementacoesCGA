// Geometry Shader
//
// Calcula as normais dos triangulos, e passa os triangulos para o fragment shader.

#version 400

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

in vec2 teTexCoord[];
out vec2 gsTexCoord;

out vec3 gsFragPos;
out vec3 Normal;

void main()
{
    vec3 v0 = gl_in[0].gl_Position.xyz;
    vec3 v1 = gl_in[1].gl_Position.xyz;
    vec3 v2 = gl_in[2].gl_Position.xyz;

    vec3 edge1 = v1 - v0;
    vec3 edge2 = v2 - v0;
    
    // Calcula normal do triangulo e passa pro fragment shader.
    Normal = normalize(cross(edge1, edge2));

    // Passa a posicao do fragmento para o fragment shader.
    gsFragPos = (v0 + v1 + v2) / 3.0f;

    for (int i = 0; i < 3; i++)
    {
        gsTexCoord = teTexCoord[i];
        gl_Position = gl_in[i].gl_Position;
        EmitVertex();
    }
    EndPrimitive();
}
