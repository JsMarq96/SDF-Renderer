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

// Jittering
uniform sampler2D u_noise_tex;
uniform int u_noise_size;

// IsoSurfaces
uniform float u_iso_threshold;
uniform float u_gradient_delta;

#define MAX_ITERATIONS 100

varying vec3 v_local_cam_pos;


// ======================================
// RENDER VAINILLA VOLUMES
// ======================================
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
}

// ======================================
// RENDER ISOSURFACES
// ======================================
vec3 gradient(float h, vec3 coords) {
	vec3 r = vec3(0.0);
	float grad_x = texture(u_texture, vec3(coords.x + h, coords.y, coords.z)).x - 
				   texture(u_texture, vec3(coords.x - h, coords.y, coords.z)).x;

	float grad_y = texture(u_texture, vec3(coords.x, coords.y + h, coords.z)).x - 
				   texture(u_texture, vec3(coords.x, coords.y - h, coords.z)).x;
	
	float grad_z = texture(u_texture, vec3(coords.x, coords.y, coords.z + h)).x - 
				   texture(u_texture, vec3(coords.x, coords.y, coords.z - h)).x;
	
	return (1.0 /  (h * 2) ) * vec3(grad_x, grad_y, grad_z);
}


vec4 render_isosurface() {
	vec3 ray_dir = normalize(u_camera_position - v_world_position);
    vec4 final_color = vec4(0.0);

    vec3 it_position = vec3(0.0);

	// Add noise to aboud jitter
	float noise_sample = (texture(u_noise_tex, gl_FragCoord.xy / 128.0)).r;
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
		
        float depth = texture(u_texture, sample_position).x;

		if (depth >= u_iso_threshold) {
			return vec4(gradient(u_gradient_delta, sample_position), 1.0);
		}

        it_position = it_position + (ray_dir * u_step_size);
	}

	return vec4(0.0);
}

void main(){
    vec4 final_color = render_volume();
	final_color.rgb = final_color.rgb * u_brightness;
	gl_FragColor = final_color;
}