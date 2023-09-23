// Geometry Shader

#version 400

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

uniform mat4 MVP;

in vec2 teTexCoord[];
out vec2 gsTexCoord;

out vec3 gsFragPos;
out vec3 Normal;

void main()
{
    vec3 v0 = vec3(inverse(MVP) * vec4(gl_in[0].gl_Position.xyz, 1.0));
    vec3 v1 = vec3(inverse(MVP) * vec4(gl_in[0].gl_Position.xyz, 1.0));
    vec3 v2 = vec3(inverse(MVP) * vec4(gl_in[0].gl_Position.xyz, 1.0));

    gsFragPos = (v0 + v1 + v2) / 3.0f;

    vec3 edge1 = v1 - v0;
    vec3 edge2 = v2 - v0;
    Normal = normalize(cross(edge1, edge2));

    for (int i = 0; i < 3; i++)
    {
        gsTexCoord = teTexCoord[i];
        gl_Position = gl_in[i].gl_Position;
        EmitVertex();
    }
    EndPrimitive();
}
