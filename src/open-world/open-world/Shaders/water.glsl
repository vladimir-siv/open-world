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

	out float visibility;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		vec4 view_position = view * world_position;
		
		visibility = min(exp(-pow(length(view_position.xyz) * fog_density, fog_gradient)), 1.0f);
		
		gl_Position = project * view_position;
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec4 skybox;
	
	uniform vec4 material_color;

	in float visibility;
	
	out vec4 out_color;
	
	void main(void)
	{
		out_color = material_color;
		out_color = mix(skybox, out_color, visibility);
	}
