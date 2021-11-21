uniform vec3 u_camera_position;
uniform vec4 u_color;
uniform vec2 u_aspect_ratio;
uniform float u_cam_rotation;

varying vec3 v_local_cam_pos;
varying vec2 v_uv;

// ====================================
//  PRIMITIVES
// ====================================
vec4 sdfBox(vec3 point, vec3 position, vec3 b, vec3 color) {
  vec3 q = abs(point - position) - b;
  return vec4(length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0), color);
}

vec4 sdfCircle(vec3 point, vec3 center, float r, vec3 color) {    
    return vec4(length(center - point) - r, color);
}

vec4 sdfHorizontalPlane(vec3 point, float depth, vec3 color) {
	return vec4(point.y - depth, color);
}


// ====================================
//  OPERATIONS
// ====================================
vec4 opUnion(vec4 dist1, vec4 dist2) {
	if (dist1.x < dist2.x) {
		return dist1;
	}
	return dist2;
}

vec4 opSubstraction(vec4 dist1, vec4 dist2) {
	return vec4(max(-dist2.x, dist1.x), dist1.yzw);
}

vec4 opIntersection(vec4 dist1, vec4 dist2) {
	if (dist1.x > dist2.x) {
		return dist1;
	}
	return dist2;
}

vec4 opSmoothUnion(vec4 d1, vec4 d2, float k) {
    float h = clamp( 0.5 + 0.5*(d2.x-d1.x)/k, 0.0, 1.0 );
    return vec4(mix( d2.x, d1.x, h ) - k*h*(1.0-h), mix(d2.yzw, d1.yzw, h)); 
}

vec4 scene(vec3 position) {
	// BIggest distance, black color
	vec4 dist = vec4(1000.0, 0.0, 0.0, 0.0);

	dist = opSmoothUnion(sdfCircle(position, 
									vec3(0.0, 0.0, 0.0), 
									0.50, 
									vec3(1.0, 0.0, 0.0)), 
						  sdfCircle(position, 
						  			vec3(0.6, 0.3, 0.2), 
									  0.20, 
									  vec3(0.0, 1.0, 0.0)),
						  0.5);

	dist = opSmoothUnion(dist, sdfHorizontalPlane(position, 
													-0.25, 
													vec3(0.0, 0.0, 1.0)), 
						 0.5);

	dist = opUnion(dist, opSubstraction( sdfBox(position, vec3(1.0, 1.0, 1.0), vec3(1.0, 1.0, 1.0) / 3.0, vec3(1.0, 1.0, 0.0)), 
										 sdfCircle(position, vec3(1.0, 1.5, 1.0), 0.5, vec3(0.0))));

	return dist;
}


// ====================================
//  REPRESENTATION
// ====================================
vec3 gradient(float h, vec3 coords) {
	vec3 r = vec3(0.0);
	float grad_x = scene(vec3(coords.x + h, coords.y, coords.z)).x - 
				   scene(vec3(coords.x - h, coords.y, coords.z)).x;

	float grad_y = scene(vec3(coords.x, coords.y + h, coords.z)).x - 
				   scene(vec3(coords.x, coords.y - h, coords.z)).x;
	
	float grad_z = scene(vec3(coords.x, coords.y, coords.z + h)).x - 
				   scene(vec3(coords.x, coords.y, coords.z - h)).x;
	
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

		vec4 min_length = scene(sample_position);

		if (min_length.x < 0.001) {
			// HIT!!
			//return vec4(gradient(0.0001, sample_position), 1.0);
			return vec4(phong(sample_position) * min_length.yzw, 1.0);
			return vec4(1.0);
		} 
		// Iterate
		it_position = it_position + (ray_dir * min_length.x);
	}

	return vec4(0.0);
}

mat3 camera_matrix(vec3 cam_pos, vec3 look_at) {
	vec3 camera_dir = normalize(look_at - cam_pos);
	vec3 camera_right = normalize(cross(vec3(0.0, 1.0, 0.0), camera_dir));
	vec3 camera_up = normalize(cross(camera_dir, camera_right));

	return mat3(-camera_right, camera_up, -camera_dir);
}

void main(){
	// Centering the UV coordinates while keeping the aspect ratio
	vec2 uv = v_uv - 0.5;
	uv.x *= u_aspect_ratio.x / u_aspect_ratio.y;

	vec3 ray_origin = vec3(4.0, 2.0, 4.0);

	// Rotate arround
	float s = sin(u_cam_rotation);
	float c = cos(u_cam_rotation);

	ray_origin.x = ray_origin.x * c - ray_origin.z * s;
	ray_origin.z = ray_origin.z * s + ray_origin.x * c;

	vec3 ray_direction = camera_matrix(ray_origin, vec3(0.0)) * normalize(vec3(uv, -1));

	ray_origin = ray_origin - ray_direction;

  // Output to screen
  	gl_FragColor = vec4(spheremarch(ray_origin, ray_direction));
}