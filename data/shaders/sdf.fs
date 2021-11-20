uniform vec3 u_camera_position;
uniform vec4 u_color;
uniform vec2 u_aspect_ratio;

varying vec3 v_local_cam_pos;
varying vec2 v_uv;


float sdfCircle(vec3 point, vec3 center, float r) {    
    return length(center - point) - r;
}

float sdfUnion(float dist1, float dist2) {
	return min(dist1, dist2);
}

float sdfSmoothUnion(float d1, float d2, float k) {
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return mix( d2, d1, h ) - k*h*(1.0-h); 
}

float sdfHorizontalPlane(vec3 point, float depth) {
	return point.y - depth;
}

float scene(vec3 position) {
	float dist = 1000.0;

	/*dist = sdfUnion(dist, sdfCircle(position, vec3(0.1, 0.0, 0.5), 0.50));

	dist = sdfUnion(dist, sdfCircle(position, vec3(0.9, 0.0, 1.0), 0.250));*/

	dist = sdfSmoothUnion(sdfCircle(position, vec3(0.1, 0.0, 0.5), 0.50), 
						  sdfCircle(position, vec3(0.9, 0.0, 0.8), 0.250),
						  0.5);

	dist = sdfSmoothUnion(dist, sdfHorizontalPlane(position, -0.25), 0.4);

	return dist;
}

vec3 gradient(float h, vec3 coords) {
	vec3 r = vec3(0.0);
	float grad_x = scene(vec3(coords.x + h, coords.y, coords.z)) - 
				   scene(vec3(coords.x - h, coords.y, coords.z));

	float grad_y = scene(vec3(coords.x, coords.y + h, coords.z)) - 
				   scene(vec3(coords.x, coords.y - h, coords.z));
	
	float grad_z = scene(vec3(coords.x, coords.y, coords.z + h)) - 
				   scene(vec3(coords.x, coords.y, coords.z - h));
	
	return normalize(vec3(grad_x, grad_y, grad_z)  /  (h * 2));
}


vec3 phong(vec3 position) {
	vec3 grad = gradient(0.0001, position);

	vec3 l = normalize( vec3(-0.9, 3.0, 3.0) - position );
	vec3 r = normalize(reflect(-l, grad));
    vec3 v = normalize(position);
	float reflect_dot_view = clamp(dot(r, v), 0.0, 1.0);

	vec3 specular = pow(reflect_dot_view, 64.0) * vec3(1.0);

	vec3 diff = vec3(0.5) * clamp( dot(l, grad), 0.0, 1.0);

	return specular + diff + vec3(0.07);
}


vec4 spheremarch(vec3 start_pos, vec3 ray_dir) {
    vec3 it_position = vec3(0.0);

	// Ray loop
	for(int i = 0; i < 100; i++){
		vec3 sample_position = start_pos + it_position;

		float min_length = scene(sample_position);

		if (min_length < 0.01) {
			// HIT!!
			//return vec4(gradient(0.0001, sample_position), 1.0);
			return vec4(phong(sample_position), 1.0);
			return vec4(1.0);
		} 
		// Iterate
		it_position = it_position + (ray_dir * min_length);
	}

	return vec4(0.0);
}

void main(){
	// Centering the UV coordinates while keeping the aspect ratio
	vec2 uv = v_uv - 0.5;
	uv.x *= u_aspect_ratio.x / u_aspect_ratio.y;

	vec3 ray_origin = vec3(0.0, 0.0, 3.0);
	vec3 ray_direction = normalize(vec3(uv, -1));

  // Output to screen
  	gl_FragColor = vec4(spheremarch(ray_origin, ray_direction));
}