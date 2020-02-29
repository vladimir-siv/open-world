Unlit Simple Shader

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
	in layout(location = 2) vec2 in_uv;

	out vec4 world_position;
	out vec4 clip_position;
	out vec4 normal;
	out vec2 uv;
	out float visibility;
	
	void main(void)
	{
		world_position = model * in_position;

		vec4 view_position = view * world_position;
		
		visibility = min(exp(-pow(length(view_position.xyz) * fog_density, fog_gradient)), 1.0f);
		
		clip_position = project * view_position;
		normal = rotate * in_normal;
		uv = in_uv;

		gl_Position = clip_position;
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec3 eye;
	uniform vec4 skybox;
	
	uniform vec4 water_color;
	uniform float wave_strength;
	uniform float wave_speed;
	uniform float wave_timestamp;
	uniform float reflectiveness;

	uniform sampler2D reflection;
	uniform sampler2D refraction;
	uniform sampler2D dudv;

	in vec4 world_position;
	in vec4 clip_position;
	in vec4 normal;
	in vec2 uv;
	in float visibility;
	
	out vec4 out_color;
	
	void main(void)
	{
		vec3 normal_vector = normalize(normal.xyz);
		vec3 eye_vector = normalize(eye - world_position.xyz);

		vec2 refract = (clip_position.xy / clip_position.w) / 2.0f + 0.5f;
		vec2 reflect = vec2(refract.x, -refract.y);

		float wave_form = mod(wave_speed * wave_timestamp, 1.0f);

		vec2 duv = texture(dudv, vec2(uv.x + wave_form, uv.y)).rg * 0.1;
		duv = uv + vec2(duv.x, duv.y + wave_form);
		vec2 distortion = (texture(dudv, duv).rg * 2.0 - 1.0) * wave_strength;

		reflect += distortion;
		refract += distortion;

		reflect.x = clamp(reflect.x, +0.001f, +0.999f);
		reflect.y = clamp(reflect.y, -0.999f, -0.001f);

		refract.x = clamp(refract.x, +0.001f, +0.999f);
		refract.y = clamp(refract.y, +0.001f, +0.999f);

		vec4 reflect_color = texture(reflection, reflect);
		vec4 refract_color = texture(refraction, refract);

		float refractiveness = dot(eye_vector, normal_vector);
		refractiveness = pow(refractiveness, max(reflectiveness, 0.0f));

		out_color = mix(reflect_color, refract_color, refractiveness);
		out_color = mix(water_color, out_color, 0.75f);
		out_color = mix(skybox, out_color, visibility);
	}
