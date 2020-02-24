Phong Shader with Ambiental, Diffuse and Specular light components.

#pragma shader vertex

	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;
	uniform mat4 rotate;
	uniform float fog_density;
	uniform float fog_gradient;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec4 in_normal;
	
	out vec3 position;
	out vec3 normal;
	out float visibility;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 world_normal = rotate * in_normal;
		vec4 view_position = view * world_position;
		
		position = world_position.xyz;
		normal = world_normal.xyz;
		visibility = min(exp(-pow(length(view_position.xyz) * fog_density, fog_gradient)), 1.0f);

		gl_Position = project * view_position;
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec3 eye;
	uniform vec4 skybox;
	
	uniform vec3 ambient_light_color;
	uniform float ambient_light_power;
	
	uniform vec3 light_source_position;
	uniform vec3 light_source_color;
	uniform float light_source_power;
	
	uniform float dampening;
	uniform float reflectivity;
	uniform vec3 material_color;
	
	in vec3 position;		// fragment position
	in vec3 normal;			// fragment normal
	in float visibiloty;	// fog visibility
	
	out vec4 out_color;
	
	void main(void)
	{
		vec3 normal_vector = normalize(normal);
		vec3 light_vector = normalize(light_source_position - position);
		vec3 eye_vector = normalize(eye - position);
		float light_distance = distance(light_source_position, position);
		float attenuation = 1.05f + 0.05f * light_distance * light_distance;
		
		vec3 ambient = ambient_light_color * ambient_light_power;
		vec3 light_source = light_source_color * light_source_power;
		
		float diffuse = clamp(dot(light_vector, normal_vector), 0.0f, 1.0f);
		float specular = pow(clamp(dot(reflect(-light_vector, normal_vector), eye_vector), 0.0f, 1.0f), dampening * 2.0f + 1.0f) * clamp(reflectivity, 0.0f, 1.0f);
		
		out_color = vec4(material_color * (ambient + light_source * (diffuse + specular) / attenuation), 1.0f);
		out_color = mix(skybox, out_color, visibility);
	}
