Unlit Simple Shader

#pragma shader vertex

	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform mat4 translate;
	uniform mat4 scale;
	uniform mat4 rotate;
	
	in layout(location = 0) vec4 in_position;
	in layout(location = 1) vec4 in_color;
	
	out vec4 color;
	
	void main(void)
	{
		color = in_color;
		gl_Position = project * view * translate * scale * rotate * in_position;
	}

#pragma shader fragment

	#version 430 core
	
	in vec4 color;
	
	out vec4 out_color;
	
	void main(void)
	{
		out_color = color;
	}
