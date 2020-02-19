using System;
using System.Collections.Generic;
using System.Text;
using SharpGL;
using GlmNet;

namespace XEngine.Shading
{
	using XEngine.Common;

	public sealed class Shader : IDisposable
	{
		private static (string, string) Preprocess(string code)
		{
			int LineCount(string str) { return str.Length - str.Replace("\n", string.Empty).Length; }

			const string PRAGMA = "#pragma shader ";

			var shader_part1 = code.IndexOf(PRAGMA);
			if (shader_part1 == -1) throw new FormatException("No shader parts found.");
			var shader_part2 = code.IndexOf(PRAGMA, shader_part1 + PRAGMA.Length);
			if (shader_part2 == -1) throw new FormatException("Only one shader part found.");
			var shader_part3 = code.IndexOf(PRAGMA, shader_part2 + PRAGMA.Length);
			if (shader_part3 != -1) throw new FormatException("More than 2 shader parts found.");

			var part1pragma = shader_part1 + PRAGMA.Length;
			var part1start = code.IndexOf('\n', part1pragma);
			var part1type = code.Substring(part1pragma, part1start - part1pragma).Trim().ToLower();
			var part1 = code.Substring(part1start + 1, shader_part2 - (part1start + 1));

			var part2pragma = shader_part2 + PRAGMA.Length;
			var part2start = code.IndexOf('\n', part2pragma);
			var part2type = code.Substring(part2pragma, part2start - part2pragma).Trim().ToLower();
			var part2 = code.Substring(part2start + 1);

			if (part1type != "vertex" && part1type != "fragment") throw new FormatException($"First shader type unknown. [{part1type}]");
			if (part2type != "vertex" && part2type != "fragment") throw new FormatException($"Second shader type unknown. [{part2type}]");

			if (part1type == part2type) throw new FormatException("Both first and second shader types are the same.");

			var part1lineshift = LineCount(code.Substring(0, shader_part1)) + 1;
			var part2lineshift = LineCount(part1) + 1;
			var sb = new StringBuilder();
			for (var i = 0; i < part1lineshift; ++i) sb.Append('\n');
			part1 = sb.ToString() + part1;
			for (var i = 0; i < part2lineshift; ++i) sb.Append('\n');
			part2 = sb.ToString() + part2;

			var vertex_shader = part1type == "vertex" ? part1 : part2;
			var fragment_shader = part1type == "fragment" ? part1 : part2;

			return (vertex_shader, fragment_shader);
		}
		private static uint Build(string shaderName)
		{
			var gl = XEngineContext.Graphics;

			string GetCompileError(uint shaderId)
			{
				var compileStatus = new int[1];
				gl.GetShader(shaderId, OpenGL.GL_COMPILE_STATUS, compileStatus);
				if (compileStatus[0] != OpenGL.GL_TRUE)
				{
					var infoLogLength = new int[1];
					gl.GetShader(shaderId, OpenGL.GL_INFO_LOG_LENGTH, infoLogLength);
					var buffer = new StringBuilder(infoLogLength[0]);
					gl.GetShaderInfoLog(shaderId, infoLogLength[0], IntPtr.Zero, buffer);
					return buffer.ToString();
				}

				return null;
			}
			string GetLinkError(uint progId)
			{
				var linkStatus = new int[1];
				gl.GetProgram(progId, OpenGL.GL_LINK_STATUS, linkStatus);
				if (linkStatus[0] != OpenGL.GL_TRUE)
				{
					var infoLogLength = new int[1];
					gl.GetProgram(progId, OpenGL.GL_INFO_LOG_LENGTH, infoLogLength);
					var buffer = new StringBuilder(infoLogLength[0]);
					gl.GetProgramInfoLog(progId, infoLogLength[0], IntPtr.Zero, buffer);
					return buffer.ToString();
				}

				return null;
			}

			var shaderCode = Preprocess(ManifestResourceManager.LoadShader(shaderName));
			var shaderProgramId = gl.CreateProgram();

			var vertexShaderId = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
			var fragmentShaderId = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);

			gl.ShaderSource(vertexShaderId, shaderCode.Item1);
			gl.ShaderSource(fragmentShaderId, shaderCode.Item2);

			gl.CompileShader(vertexShaderId);
			var vertexCompileError = GetCompileError(vertexShaderId);
			if (vertexCompileError != null) throw new ApplicationException($"[VERTEX COMPILE ERROR]\r\n{vertexCompileError}");

			gl.CompileShader(fragmentShaderId);
			var fragmentCompileError = GetCompileError(fragmentShaderId);
			if (fragmentCompileError != null) throw new ApplicationException($"[FRAGMENT COMPILE ERROR]\r\n{fragmentCompileError}");

			gl.AttachShader(shaderProgramId, vertexShaderId);
			gl.AttachShader(shaderProgramId, fragmentShaderId);

			gl.LinkProgram(shaderProgramId);
			var linkError = GetLinkError(shaderProgramId);
			if (linkError != null) throw new ApplicationException($"[LINK ERROR]\r\n{linkError}");

			gl.DeleteShader(vertexShaderId);
			gl.DeleteShader(fragmentShaderId);

			return shaderProgramId;
		}

		public static Shader Find(string shaderName)
		{
			var shaders = XEngineContext.Shaders;
			var found = shaders.TryGetValue(shaderName, out var shader);
			if (found) return shader;
			var id = Build(shaderName);
			shader = new Shader(id, shaderName);
			shaders.Add(shaderName, shader);
			return shader;
		}

		public static uint CurrentShaderId = 0;

		public uint Id { get; private set; }
		public string Name { get; private set; }

		public int Project { get; private set; }
		public int View { get; private set; }
		public int Model { get; private set; }
		public int Rotate { get; private set; }
		public int Eye { get; private set; }

		private readonly Dictionary<string, int> Uniforms = new Dictionary<string, int>();

		private Shader(uint id, string name)
		{
			Id = id;
			Name = name;

			var gl = XEngineContext.Graphics;
			Project = gl.GetUniformLocation(id, "project");
			View = gl.GetUniformLocation(id, "view");
			Model = gl.GetUniformLocation(id, "model");
			Rotate = gl.GetUniformLocation(id, "rotate");
			Eye = gl.GetUniformLocation(id, "eye_position");
		}

		internal void Clean()
		{
			var gl = XEngineContext.Graphics;
			if (CurrentShaderId == Id) gl.UseProgram(0);
			gl.DeleteProgram(Id);
			Id = 0;
		}
		public void Dispose()
		{
			Clean();
			XEngineContext.Shaders.Remove(Name);
		}

		public void Use()
		{
			if (Id == 0) throw new InvalidOperationException("Shader object was disposed.");
			if (Id == CurrentShaderId) return;
			var gl = XEngineContext.Graphics;
			gl.UseProgram(Id);
			CurrentShaderId = Id;
		}

		public int GetLocation(string name)
		{
			if (Id == 0) throw new InvalidOperationException("Shader object was disposed.");
			var gl = XEngineContext.Graphics;
			var found = Uniforms.TryGetValue(name, out var location);
			if (found) return location;
			location = gl.GetUniformLocation(Id, name);
			if (location == -1) throw new ArgumentException($"Unknown uniform '{name}'.");
			Uniforms.Add(name, location);
			return location;
		}

		public void Set(string name, int value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		public void Set(string name, uint value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		public void Set(string name, float value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		public void Set(string name, Color value) => XEngineContext.Graphics.Uniform4(GetLocation(name), value.r, value.g, value.b, value.a);
		public void Set(string name, vec2 value) => XEngineContext.Graphics.Uniform2(GetLocation(name), value.x, value.y);
		public void Set(string name, vec3 value) => XEngineContext.Graphics.Uniform3(GetLocation(name), value.x, value.y, value.z);
		public void Set(string name, vec4 value) => XEngineContext.Graphics.Uniform4(GetLocation(name), value.x, value.y, value.z, value.w);
		public void Set(string name, mat2 value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix2(GetLocation(name), 1, transpose, value.to_array());
		public void Set(string name, mat3 value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix3(GetLocation(name), 1, transpose, value.to_array());
		public void Set(string name, mat4 value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix4(GetLocation(name), 1, transpose, value.to_array());
		public void Set(string name, int[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		public void Set(string name, uint[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		public void Set(string name, float[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		public void Set(string name, Color[] values, bool includeAlpha = true) => XEngineContext.Graphics.Uniform4(GetLocation(name), values.Length, values.serialize(includeAlpha));
		public void Set(string name, vec2[] values) => XEngineContext.Graphics.Uniform2(GetLocation(name), values.Length, values.serialize());
		public void Set(string name, vec3[] values) => XEngineContext.Graphics.Uniform3(GetLocation(name), values.Length, values.serialize());
		public void Set(string name, vec4[] values) => XEngineContext.Graphics.Uniform4(GetLocation(name), values.Length, values.serialize());
		public void Set(string name, mat2[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix2(GetLocation(name), values.Length, transpose, values.serialize());
		public void Set(string name, mat3[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix3(GetLocation(name), values.Length, transpose, values.serialize());
		public void Set(string name, mat4[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix4(GetLocation(name), values.Length, transpose, values.serialize());
		public void Set(string name, float x, float y, float z) => XEngineContext.Graphics.Uniform3(GetLocation(name), x, y, z);
		public void Set(string name, float x, float y, float z, float w) => XEngineContext.Graphics.Uniform4(GetLocation(name), x, y, z, w);
	}
}
