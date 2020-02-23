﻿Phong Shader with Ambiental, Diffuse and Specular light components.

#pragma shader vertex

	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;
	uniform mat4 rotate;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec4 in_normal;
	in layout(location = 2) vec2 in_uv;
	
	out vec3 position;
	out vec3 normal;
	out vec2 uv;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 world_normal = rotate * in_normal;
		
		position = world_position.xyz;
		normal = world_normal.xyz;
		uv = in_uv;

		gl_Position = project * view * world_position;
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec3 eye;
	
	uniform vec3 ambient_light_color;
	uniform float ambient_light_power;
	
	uniform vec3 light_source_position;
	uniform vec3 light_source_color;
	uniform float light_source_power;
	
	uniform float dampening;
	uniform float reflectivity;
	uniform float use_simulated_light;

	uniform sampler2D texture_sampler;
	
	in vec3 position;	// fragment position
	in vec3 normal;		// fragment normal
	in vec2 uv;			// fragment uv
	
	out vec4 out_color;
	
	void main(void)
	{
		vec4 material_color = texture(texture_sampler, uv);
		if (material_color.a < 0.1f) discard;

		vec3 normal_vector = normalize(normal) * (1.0f - use_simulated_light) + vec3(0.0f, 1.0f, 0.0f) * use_simulated_light;
		vec3 light_vector = normalize(light_source_position - position);
		vec3 eye_vector = normalize(eye - position);
		float light_distance = distance(light_source_position, position);
		float attenuation = 1.05f + 0.05f * light_distance * light_distance;
		
		vec3 ambient = ambient_light_color * ambient_light_power;
		vec3 light_source = light_source_color * light_source_power;
		
		float diffuse = clamp(dot(light_vector, normal_vector), 0.0f, 1.0f);
		float specular = pow(clamp(dot(reflect(-light_vector, normal_vector), eye_vector), 0.0f, 1.0f), dampening * 2.0f + 1.0f) * clamp(reflectivity, 0.0f, 1.0f);
		
		out_color = vec4(material_color.xyz * (ambient + light_source * (diffuse + specular) / attenuation), 1.0f);
	}