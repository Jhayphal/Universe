#version 330 core

layout(location = 0) in vec3 aPosition;  
layout(location = 1) in vec3 aColor;

out vec3 ourColor;

uniform float scaleX;
uniform mat4 transform;

void main(void)
{
    gl_Position = vec4((aPosition.x - 1.0) * scaleX, aPosition.y - 1.0, aPosition.z, 1.0) * transform;

    ourColor = aColor;
}