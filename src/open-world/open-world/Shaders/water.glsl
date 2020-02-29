Unlit Simple Shader

#pragma shader vertex

	#version 430 core

	uniform vec4 clip_plane;
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;
	uniform float fog_density;
	uniform float fog_gradient;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec2 in_uv;

	out vec4 clip_space;
	out vec2 uv;
	out float visibility;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 view_position = view * world_position;
		
		visibility = min(exp(-pow(length(view_position.xyz) * fog_density, fog_gradient)), 1.0f);
		
		clip_space = project * view_position;
		uv = in_uv;

		gl_Position = clip_space;
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec4 skybox;
	
	uniform vec4 water_color;
	uniform float wave_strength;
	uniform float wave_speed;
	uniform float wave_timestamp;

	uniform sampler2D reflection;
	uniform sampler2D refraction;
	uniform sampler2D dudv;

	in vec4 clip_space;
	in vec2 uv;
	in float visibility;
	
	out vec4 out_color;
	
	void main(void)
	{
		vec2 refract = (clip_space.xy / clip_space.w) / 2.0f + 0.5f;
		vec2 reflect = vec2(refract.x, -refract.y);

		float wave_form = mod(wave_speed * wave_timestamp, 1.0f);

		vec2 distortion1 = (texture(dudv, vec2(uv.x + wave_form, uv.y)).rg * 2.0f - 1.0f) * wave_strength;
		vec2 distortion2 = (texture(dudv, vec2(+uv.x + wave_form, uv.y + wave_form)).rg * 2.0f - 1.0f) * wave_strength;

		vec2 distortion = distortion1 + distortion2;

		reflect += distortion;
		refract += distortion;

		reflect.x = clamp(reflect.x, +0.001f, +0.999f);
		reflect.y = clamp(reflect.y, -0.999f, -0.001f);

		refract.x = clamp(refract.x, +0.001f, +0.999f);
		refract.y = clamp(refract.y, +0.001f, +0.999f);

		vec4 reflect_color = texture(reflection, reflect);
		vec4 refract_color = texture(refraction, refract);

		out_color = mix(reflect_color, refract_color, 0.5f);
		out_color = mix(water_color, out_color, 0.75f);
		out_color = mix(skybox, out_color, visibility);
	}
