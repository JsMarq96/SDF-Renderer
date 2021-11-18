uniform vec3 u_camera_position;
uniform vec4 u_color;

varying vec3 v_local_cam_pos;
varying vec2 v_uv;

// ======================================
// RENDER VAINILLA VOLUMES
// ======================================
/*
vec4 render_volume() {
	vec3 ray_dir = normalize(v_local_cam_pos - v_position);
    vec4 final_color = vec4(0.0);

    vec3 it_position = vec3(0.0);

	// Add noise to aboud jitter
	float noise_sample = normalize(texture(u_noise_tex, gl_FragCoord.xy / 1.0)).r;
	it_position = it_position + (noise_sample * ray_dir);

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
		
        float depth = texture3D(u_texture, sample_position).x;

		vec4 sample_color = vec4(depth, depth, depth, depth);
		//vec4 sample_color = vec4(depth, 1.0 - depth, 0.0, depth * depth);

		final_color = final_color + (u_step_size * (1.0 - final_color.a) * sample_color);

        it_position = it_position + (ray_dir * u_step_size);
	}

	return final_color;
}*/


void main(){
	gl_FragColor = vec4(v_uv, 1.0, 1.0);
}