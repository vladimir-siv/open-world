using System;
using System.Text;
using SharpGL;

namespace XEngine.Shaders
{
	public static class ShaderProgram
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

		public static uint Build(OpenGL gl, string shaderName)
		{
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

			uint vertexShaderId = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
			uint fragmentShaderId = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);

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
	}
}
