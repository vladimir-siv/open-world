﻿Unlit Texture Shader

#pragma shader vertex

	#version 430 core

	uniform vec4 clip_plane;
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;

	uniform vec2 material_texture_offset;
	uniform uint material_texture_rank;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec2 in_uv;
	
	out vec2 uv;
	
	void main(void)
	{
		vec4 world_position = model * in_position;
		uv = (in_uv / material_texture_rank) + material_texture_offset;
		gl_Position = project * view * world_position;
		gl_ClipDistance[0] = dot(world_position, clip_plane);
	}

#pragma shader fragment

	#version 430 core
	
	uniform sampler2D material_texture;
	
	in vec2 uv;
	
	out vec4 out_color;
	
	void main(void)
	{
		out_color = texture(material_texture, uv);
	}
