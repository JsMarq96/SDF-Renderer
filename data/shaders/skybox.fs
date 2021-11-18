varying vec3 v_position;
varying vec3 v_world_position;
varying vec3 v_normal;

uniform vec4 u_color;
uniform samplerCube u_texture;

void main()
{
	// Tint the base texture
    gl_FragColor = u_color * textureCube(u_texture, v_position);
}