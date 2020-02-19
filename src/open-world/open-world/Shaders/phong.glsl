Phong Shader with Ambiental, Diffuse and Specular light components.

#pragma shader vertex

	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;
	uniform mat4 rotate;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec4 in_normal;
	
	out vec3 position;
	out vec3 normal;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 world_normal = rotate * in_normal;
		
		position = world_position.xyz;
		normal = world_normal.xyz;
		
		gl_Position = project * view * world_position;
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec3 material_color;
	
	uniform vec3 ambient_light_color;
	uniform float ambient_light_power;
	
	uniform vec3 light_source_position;
	uniform vec3 light_source_color;
	uniform float light_source_power;
	
	uniform vec3 eye_position;
	
	in vec3 position;	// fragment position
	in vec3 normal;		// fragment normal
	
	out vec4 out_color;
	
	void main(void)
	{
		vec3 normal_vector = normalize(normal);
		vec3 light_vector = normalize(light_source_position - position);
		vec3 eye_vector = normalize(eye_position - position);
		float light_distance = distance(light_source_position, position);
		float attenuation = 1.05f + 0.05f * light_distance * light_distance;
		
		vec3 ambient = ambient_light_color * ambient_light_power;
		vec3 light_source = light_source_color * light_source_power;
		
		float diffuse = clamp(dot(light_vector, normal_vector), 0.0f, 1.0f); // cos(angle)
		float specular = pow(clamp(dot(reflect(-light_vector, normal_vector), eye_vector), 0.0f, 1.0f), 21.0f); // cos(angle)^21
		
		out_color = vec4(material_color * (ambient + light_source * (diffuse + specular) / attenuation), 1.0f);
	}
