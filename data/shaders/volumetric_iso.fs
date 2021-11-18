varying vec3 v_position;
varying vec3 v_world_position;
varying vec3 v_normal;
varying vec2 v_uv;
varying vec4 v_color;

uniform float u_brightness;
uniform float u_step_size;
uniform vec3 u_camera_position;
uniform vec4 u_color;
uniform sampler3D u_texture;

uniform sampler2D u_noise_tex;
uniform int u_noise_size;
uniform float u_noise_contribution;

#define MAX_ITERATIONS 100

void main(){
    // Is the direction correct??
    vec3 ray_dir = normalize(u_camera_position - v_world_position);
    vec4 final_color = vec4(0.0);

    vec3 it_position = vec3(0.0);

	// Add noise to aboud jitter
	it_position = it_position + (texture(u_noise_tex, gl_FragCoord.xy / u_noise_size).rgb * u_noise_contribution);

	// Ray loop
	for(int i = 0; i < MAX_ITERATIONS; i++){
		vec3 sample_position = ((v_position - it_position) / 2.0) + 0.5;

		if (sample_position.x < 0.0 && sample_position.y < 0.0 && sample_position.z < 0.0) {
			break;
		}
		if (sample_position.x > 1.0 && sample_position.y > 1.0 && sample_position.z > 1.0) {
			break;
		}

		if (final_color.a == 1.0) {
			break;
		}
		
        float depth = texture(u_texture, sample_position).x;

		vec4 sample_color = vec4(depth, depth, depth, depth);
		//vec4 sample_color = vec4(depth, 1.0 - depth, 0.0, depth * depth);

		final_color = final_color + (u_step_size * (1.0 - final_color.a) * sample_color);

        it_position = it_position + (ray_dir * u_step_size);
	}

	final_color.rgb = final_color.rgb * u_brightness;
	
	gl_FragColor = final_color;
}