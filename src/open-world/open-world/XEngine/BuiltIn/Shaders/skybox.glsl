Skybox Shader

#pragma shader vertex

	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform float scale;

	in layout(location = 0) vec3 in_position;
	
	out vec3 tex;
	
	void main(void)
	{
		vec3 position = in_position * scale;
		tex = position;
		gl_Position = project * view * vec4(position, 1.0f);
	}

#pragma shader fragment

	#version 430 core
	
	uniform samplerCube sky_map;
	
	in vec3 tex;
	
	out vec4 out_color;
	
	void main(void)
	{
		out_color = texture(sky_map, tex);
	}
