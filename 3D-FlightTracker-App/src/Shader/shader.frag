#version 330 core

out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform sampler2D texture0;

void main()
{
    // Ambient lighting
    float ambientStrength = 0.85;
    vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0); // white light

    // Combine results
    vec3 result = (ambient) * texture(texture0, TexCoord).rgb;
    FragColor = vec4(result, 1.0);
}
