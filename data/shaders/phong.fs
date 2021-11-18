varying vec3 v_position;
varying vec3 v_world_position;
varying vec3 v_normal;
varying vec3 v_light_local_position;
varying vec3 v_local_camera_pos;

uniform float u_material_ambient;
uniform float u_material_diffuse;
uniform float u_material_specular;
uniform float u_material_shininess;

uniform vec3 u_light_ambient;
uniform vec3 u_light_diffuse;
uniform vec3 u_light_specular;
uniform vec4 u_light_color;
uniform vec3 u_light_position;

uniform vec4 u_color;
uniform vec3 u_camera_position;

void main()
{
	vec3 normal = normalize(v_normal);
    vec3 light_vector = normalize(u_light_position - v_world_position);

    // Ambient component
    vec3 ambient_component = u_material_ambient * u_light_ambient;

    // Diffuse component
    float light_dot_norm = max(dot(normal, light_vector), 0.0);
    vec3 diffuse_componenet = light_dot_norm * u_material_diffuse * u_light_diffuse;

    // Specular compomenet
    vec3 r = normalize(reflect(light_vector, normal));
    vec3 v = normalize(v_world_position - u_camera_position);
    float reflect_dot_view = max(dot(r, v), 0.0);

    vec3 specular_component = pow(reflect_dot_view, u_material_shininess) * u_material_specular * u_light_specular;

    // Diffuse and ambient lights depend on the base color of the object
    // Specular deppedns on the light color
    gl_FragColor = vec4( (u_color.xyz * (ambient_component + diffuse_componenet)) + (u_light_color.xyz * specular_component), 1.0);
}