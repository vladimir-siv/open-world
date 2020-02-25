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

	const float LOWER_L = +00.0f;
	const float UPPER_L = +30.0f;
	
	uniform vec4 sky_color;
	uniform samplerCube sky_map;
	
	in vec3 tex;
	
	out vec4 out_color;
	
	void main(void)
	{
		float factor = clamp((tex.y - LOWER_L) / (UPPER_L - LOWER_L), 0.0f, 1.0f);
		out_color = mix(sky_color, texture(sky_map, tex), factor);
	}
