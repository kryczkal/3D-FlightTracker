#version 330 core
in vec3 vec3Position;

void main()
{
    gl_Position = vec4(vec3Position, 1.0);
}