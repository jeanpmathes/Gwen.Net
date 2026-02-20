#version 400

in vec2 TextureCoordinates;

uniform sampler2D Texture;

out vec4 Color;

void main(void)
{
	Color = texture(Texture, TextureCoordinates);
}
