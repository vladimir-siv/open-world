﻿Phong Shader with Ambiental, Diffuse and Specular light components.

#pragma shader vertex

	#version 430 core
	
	uniform vec4 clip_plane;
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
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec3 eye;
	uniform vec4 skybox;
	
	uniform vec3 ambient_light_color;
	uniform float ambient_light_power;

	uniform uint light_source_count;
	uniform vec3 light_source_position[8];
	uniform vec3 light_source_color[8];
	uniform float light_source_power[8];
	uniform vec3 light_source_attenuation[8];
	
	uniform float dampening;
	uniform float reflectivity;
	uniform vec3 material_color;
	
	in vec3 position;		// fragment position
	in vec3 normal;			// fragment normal
	in float visibility;	// fog visibility
	
	out vec4 out_color;
	
	vec3 phong(int i, vec3 normal_vector, vec3 eye_vector)
	{
		vec3 light_vector = light_source_position[i] - position;
		float light_distance = length(light_vector);
		light_vector = normalize(light_vector);
		float attenuation =
			light_source_attenuation[i].x +
			light_source_attenuation[i].y * light_distance +
			light_source_attenuation[i].z * light_distance * light_distance;

		float diffuse = clamp(dot(light_vector, normal_vector), 0.0f, 1.0f);
		float specular = pow(clamp(dot(reflect(-light_vector, normal_vector), eye_vector), 0.0f, 1.0f), dampening * 2.0f + 1.0f) * clamp(reflectivity, 0.0f, 1.0f);

		return light_source_color[i] * light_source_power[i] * (diffuse + specular) / attenuation;
	}
	
	void main(void)
	{
		vec3 normal_vector = normalize(normal);
		vec3 eye_vector = normalize(eye - position);
		vec3 light = ambient_light_color * ambient_light_power;
		for (int i = 0; i < light_source_count; ++i) light = light + phong(i, normal_vector, eye_vector);
		out_color = vec4(material_color * light, 1.0f);
		out_color = mix(skybox, out_color, visibility);
	}
