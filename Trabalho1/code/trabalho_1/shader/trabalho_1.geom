// Geometry Shader

#version 400

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

in vec2 teTexCoord[];
out vec2 gsTexCoord;

void main()
{
    for (int i = 0; i < 3; i++)
    {
        gsTexCoord = teTexCoord[i];
        gl_Position = gl_in[i].gl_Position;
        EmitVertex();
    }
    EndPrimitive();
}
