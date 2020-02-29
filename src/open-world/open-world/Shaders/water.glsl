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

	out vec4 clip_space;
	out float visibility;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 view_position = view * world_position;
		
		visibility = min(exp(-pow(length(view_position.xyz) * fog_density, fog_gradient)), 1.0f);
		
		clip_space = project * view_position;
		gl_Position = clip_space;
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec4 skybox;
	
	uniform vec4 water_color;
	uniform sampler2D reflection;
	uniform sampler2D refraction;

	in vec4 clip_space;
	in float visibility;
	
	out vec4 out_color;
	
	void main(void)
	{
		vec2 refract = (clip_space.xy / clip_space.w) / 2.0f + 0.5f;
		vec2 reflect = vec2(refract.x, -refract.y);

		vec4 reflect_color = texture(reflection, reflect);
		vec4 refract_color = texture(refraction, refract);

		out_color = mix(reflect_color, refract_color, 0.5f);
		out_color = mix(water_color, out_color, 0.75f);
		out_color = mix(skybox, out_color, visibility);
	}
