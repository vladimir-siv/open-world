using System;
using System.Collections.Generic;
using System.Text;
using SharpGL;

namespace XEngine.Shading
{
	using XEngine.Core;

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
			gl.ValidateProgram(shaderProgramId);
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

		public int Eye { get; private set; }
		public int Project { get; private set; }
		public int View { get; private set; }
		public int Model { get; private set; }
		public int Rotate { get; private set; }
		public int Skybox { get; private set; }
		public int FogDensity { get; private set; }
		public int FogGradient { get; private set; }

		private Material Prepared = null;
		private readonly Dictionary<string, int> Uniforms = new Dictionary<string, int>();

		private Shader(uint id, string name)
		{
			Id = id;
			Name = name;

			var gl = XEngineContext.Graphics;
			Eye = gl.GetUniformLocation(id, "eye");
			Project = gl.GetUniformLocation(id, "project");
			View = gl.GetUniformLocation(id, "view");
			Model = gl.GetUniformLocation(id, "model");
			Rotate = gl.GetUniformLocation(id, "rotate");
			Skybox = gl.GetUniformLocation(id, "skybox");
			FogDensity = gl.GetUniformLocation(id, "fog_density");
			FogGradient = gl.GetUniformLocation(id, "fog_gradient");
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

		public void Use(bool dontSendEyeProjectView = false)
		{
			if (Id == 0) throw new InvalidOperationException("Shader object was disposed.");
			if (Id == CurrentShaderId) return;
			var gl = XEngineContext.Graphics;
			gl.UseProgram(Id);
			CurrentShaderId = Id;
			if (dontSendEyeProjectView) return;
			var scene = SceneManager.CurrentScene;
			var camera = scene.MainCamera;
			var skybox = scene.Skybox.SkyColor;
			if (Eye != -1) gl.Uniform3(Eye, camera.Position.x, camera.Position.y, camera.Position.z);
			if (Project != -1) gl.UniformMatrix4(Project, 1, false, camera.ViewToProjectData);
			if (View != -1) gl.UniformMatrix4(View, 1, false, camera.WorldToViewData);
			if (Skybox != -1) gl.Uniform4(Skybox, skybox.r, skybox.g, skybox.b, skybox.a);
			if (FogDensity != -1) gl.Uniform1(FogDensity, scene.FogDensity);
			if (FogGradient != -1) gl.Uniform1(FogGradient, scene.FogGradient);
		}
		public bool IsUsing(string uniform)
		{
			if (Uniforms.ContainsKey(uniform)) return true;
			var gl = XEngineContext.Graphics;
			var location = gl.GetUniformLocation(Id, uniform);
			bool uses = location != -1;
			if (uses) Uniforms.Add(uniform, location);
			return uses;
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

		internal bool PrepareNeeded(Material material, bool markPrepared = false)
		{
			if (material.shader != this) throw new InvalidOperationException("Material is not using this shader.");
			if (CurrentShaderId != Id) throw new InvalidOperationException("Shader not active (call Use() method before this).");
			if (material == Prepared || material == null) return false;
			if (markPrepared) Prepared = material;
			return true;
		}
		internal void SetScalar(string name, int value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		internal void SetScalar(string name, uint value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		internal void SetScalar(string name, float value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		internal void SetScalarArray(string name, int[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		internal void SetScalarArray(string name, uint[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		internal void SetScalarArray(string name, float[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		internal void SetVec2(string name, float x, float y) => XEngineContext.Graphics.Uniform2(GetLocation(name), x, y);
		internal void SetVec3(string name, float x, float y, float z) => XEngineContext.Graphics.Uniform3(GetLocation(name), x, y, z);
		internal void SetVec4(string name, float x, float y, float z, float w) => XEngineContext.Graphics.Uniform4(GetLocation(name), x, y, z, w);
		internal void SetVec2Array(string name, float[] values) => XEngineContext.Graphics.Uniform2(GetLocation(name), values.Length / 2, values);
		internal void SetVec3Array(string name, float[] values) => XEngineContext.Graphics.Uniform3(GetLocation(name), values.Length / 3, values);
		internal void SetVec4Array(string name, float[] values) => XEngineContext.Graphics.Uniform4(GetLocation(name), values.Length / 4, values);
		internal void SetMat2(string name, float[] value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix2(GetLocation(name), 1, transpose, value);
		internal void SetMat3(string name, float[] value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix3(GetLocation(name), 1, transpose, value);
		internal void SetMat4(string name, float[] value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix4(GetLocation(name), 1, transpose, value);
		internal void SetMat2Array(string name, float[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix2(GetLocation(name), values.Length / 4, transpose, values);
		internal void SetMat3Array(string name, float[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix3(GetLocation(name), values.Length / 9, transpose, values);
		internal void SetMat4Array(string name, float[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix4(GetLocation(name), values.Length / 16, transpose, values);
	}
}
