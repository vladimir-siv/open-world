Unlit Texture Shader

#pragma shader vertex

	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 model;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec2 in_uv;
	
	out vec2 uv;
	
	void main(void)
	{
		uv = in_uv;
		gl_Position = project * view * model * in_position;
	}

#pragma shader fragment

	#version 430 core
	
	uniform sampler2D texture_sampler;
	
	in vec2 uv;
	
	out vec4 out_color;
	
	void main(void)
	{
		out_color = texture(texture_sampler, uv);
	}
