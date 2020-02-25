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
	in layout(location = 2) vec2 in_uv;
	
	out vec3 position;
	out vec3 normal;
	out vec2 uv;
	out float visibility;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 world_normal = rotate * in_normal;
		vec4 view_position = view * world_position;
		
		position = world_position.xyz;
		normal = world_normal.xyz;
		uv = in_uv;
		visibility = min(exp(-pow(length(view_position.xyz) * fog_density, fog_gradient)), 1.0f);

		gl_Position = project * view_position;
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

	uniform uint tiles;

	uniform sampler2D main_texture;
	uniform sampler2D r_texture;
	uniform sampler2D g_texture;
	uniform sampler2D b_texture;
	uniform sampler2D terrain_map;
	
	in vec3 position;		// fragment position
	in vec3 normal;			// fragment normal
	in vec2 uv;				// fragment uv
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
		vec4 weights = texture(terrain_map, uv / tiles);
		vec4 main_texture_c = texture(main_texture, uv) * (1.0f - (weights.r + weights.g + weights.b));
		vec4 r_texture_c = texture(r_texture, uv) * weights.r;
		vec4 g_texture_c = texture(g_texture, uv) * weights.g;
		vec4 b_texture_c = texture(b_texture, uv) * weights.b;
		vec4 material_color = main_texture_c + r_texture_c + g_texture_c + b_texture_c;
		
		vec3 normal_vector = normalize(normal);
		vec3 eye_vector = normalize(eye - position);
		vec3 light = ambient_light_color * ambient_light_power;
		for (int i = 0; i < light_source_count; ++i) light = light + phong(i, normal_vector, eye_vector);
		out_color = vec4(material_color.xyz * light, 1.0f);
		out_color = mix(skybox, out_color, visibility);
	}
