Unlit Simple Shader

#pragma shader vertex

	#version 430 core

	uniform vec4 clip_plane;
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;
	
	in layout(location = 0) vec4 in_position;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		gl_Position = project * view * world_position;
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform vec4 material_color;
	
	out vec4 out_color;
	
	void main(void)
	{
		out_color = material_color;
	}
