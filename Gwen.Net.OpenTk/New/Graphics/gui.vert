#version 400

out vec2 TextureCoordinates;

void main(void)
{
    vec2 position = vec2(gl_VertexID % 2, gl_VertexID / 2) * 4.0 - 1;
    gl_Position = vec4(position, 0, 1);

    TextureCoordinates = vec2((position.x + 1) * 0.5, 1.0 - (position.y + 1) * 0.5);
}
