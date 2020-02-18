using System.Collections.Generic;
using GlmNet;

namespace XEngine.Shading
{
	public sealed class Material
	{
		private readonly SortedDictionary<string, int>								ParamsInt		= new SortedDictionary<string, int>();
		private readonly SortedDictionary<string, uint>								ParamsUInt		= new SortedDictionary<string, uint>();
		private readonly SortedDictionary<string, float>							ParamsFloat		= new SortedDictionary<string, float>();
		private readonly SortedDictionary<string, Color>							ParamsColor		= new SortedDictionary<string, Color>();
		private readonly SortedDictionary<string, vec2>								ParamsVec2		= new SortedDictionary<string, vec2>();
		private readonly SortedDictionary<string, vec3>								ParamsVec3		= new SortedDictionary<string, vec3>();
		private readonly SortedDictionary<string, vec4>								ParamsVec4		= new SortedDictionary<string, vec4>();
		private readonly SortedDictionary<string, (mat2, bool)>						ParamsMat2		= new SortedDictionary<string, (mat2, bool)>();
		private readonly SortedDictionary<string, (mat3, bool)>						ParamsMat3		= new SortedDictionary<string, (mat3, bool)>();
		private readonly SortedDictionary<string, (mat4, bool)>						ParamsMat4		= new SortedDictionary<string, (mat4, bool)>();
		private readonly SortedDictionary<string, int[]>							ParamsInts		= new SortedDictionary<string, int[]>();
		private readonly SortedDictionary<string, uint[]>							ParamsUInts		= new SortedDictionary<string, uint[]>();
		private readonly SortedDictionary<string, float[]>							ParamsFloats	= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, (Color[], bool)>					ParamsColors	= new SortedDictionary<string, (Color[], bool)>();
		private readonly SortedDictionary<string, vec2[]>							ParamsVec2s		= new SortedDictionary<string, vec2[]>();
		private readonly SortedDictionary<string, vec3[]>							ParamsVec3s		= new SortedDictionary<string, vec3[]>();
		private readonly SortedDictionary<string, vec4[]>							ParamsVec4s		= new SortedDictionary<string, vec4[]>();
		private readonly SortedDictionary<string, (mat2[], bool)>					ParamsMat2s		= new SortedDictionary<string, (mat2[], bool)>();
		private readonly SortedDictionary<string, (mat3[], bool)>					ParamsMat3s		= new SortedDictionary<string, (mat3[], bool)>();
		private readonly SortedDictionary<string, (mat4[], bool)>					ParamsMat4s		= new SortedDictionary<string, (mat4[], bool)>();
		private readonly SortedDictionary<string, (float, float, float)>			Params3f		= new SortedDictionary<string, (float, float, float)>();
		private readonly SortedDictionary<string, (float, float, float, float)>		Params4f		= new SortedDictionary<string, (float, float, float, float)>();

		public Shader shader { get; set; }

		public Material() { }
		public Material(Shader shader) { this.shader = shader; }

		public void Set(string name, int value) => ParamsInt[name] = value;
		public void Set(string name, uint value) => ParamsUInt[name] = value;
		public void Set(string name, float value) => ParamsFloat[name] = value;
		public void Set(string name, Color value) => ParamsColor[name] = value;
		public void Set(string name, vec2 value) => ParamsVec2[name] = value;
		public void Set(string name, vec3 value) => ParamsVec3[name] = value;
		public void Set(string name, vec4 value) => ParamsVec4[name] = value;
		public void Set(string name, mat2 value, bool transpose = false) => ParamsMat2[name] = (value, transpose);
		public void Set(string name, mat3 value, bool transpose = false) => ParamsMat3[name] = (value, transpose);
		public void Set(string name, mat4 value, bool transpose = false) => ParamsMat4[name] = (value, transpose);
		public void Set(string name, int[] values) => ParamsInts[name] = values;
		public void Set(string name, uint[] values) => ParamsUInts[name] = values;
		public void Set(string name, float[] values) => ParamsFloats[name] = values;
		public void Set(string name, Color[] values, bool includeAlpha = true) => ParamsColors[name] = (values, includeAlpha);
		public void Set(string name, vec2[] values) => ParamsVec2s[name] = values;
		public void Set(string name, vec3[] values) => ParamsVec3s[name] = values;
		public void Set(string name, vec4[] values) => ParamsVec4s[name] = values;
		public void Set(string name, mat2[] values, bool transpose = false) => ParamsMat2s[name] = (values, transpose);
		public void Set(string name, mat3[] values, bool transpose = false) => ParamsMat3s[name] = (values, transpose);
		public void Set(string name, mat4[] values, bool transpose = false) => ParamsMat4s[name] = (values, transpose);
		public void Set(string name, float x, float y, float z) => Params3f[name] = (x, y, z);
		public void Set(string name, float x, float y, float z, float w) => Params4f[name] = (x, y, z, w);

		internal void Prepare()
		{
			if (shader == null) return;
			shader.BeginInit();
			foreach (var v in ParamsInt) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsUInt) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsFloat) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsColor) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec2) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec3) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec4) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsMat2) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat3) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat4) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsInts) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsUInts) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsFloats) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsColors) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsVec2s) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec3s) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec4s) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsMat2s) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat3s) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat4s) shader.Set(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in Params3f) shader.Set(v.Key, v.Value.Item1, v.Value.Item2, v.Value.Item2);
			foreach (var v in Params4f) shader.Set(v.Key, v.Value.Item1, v.Value.Item2, v.Value.Item3);
			shader.EndInit();
		}
	}
}
