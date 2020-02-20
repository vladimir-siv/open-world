using System.Collections.Generic;
using GlmNet;

namespace XEngine.Shading
{
	using XEngine.Common;

	public sealed class Material
	{
		private readonly SortedDictionary<string, int>								ParamsInt		= new SortedDictionary<string, int>();
		private readonly SortedDictionary<string, uint>								ParamsUInt		= new SortedDictionary<string, uint>();
		private readonly SortedDictionary<string, float>							ParamsFloat		= new SortedDictionary<string, float>();
		private readonly SortedDictionary<string, int[]>							ParamsInts		= new SortedDictionary<string, int[]>();
		private readonly SortedDictionary<string, uint[]>							ParamsUInts		= new SortedDictionary<string, uint[]>();
		private readonly SortedDictionary<string, float[]>							ParamsFloats	= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, Color>							ParamsColor		= new SortedDictionary<string, Color>();
		private readonly SortedDictionary<string, vec2>								ParamsVec2		= new SortedDictionary<string, vec2>();
		private readonly SortedDictionary<string, vec3>								ParamsVec3		= new SortedDictionary<string, vec3>();
		private readonly SortedDictionary<string, vec4>								ParamsVec4		= new SortedDictionary<string, vec4>();
		private readonly SortedDictionary<string, (float, float, float)>			Params3f		= new SortedDictionary<string, (float, float, float)>();
		private readonly SortedDictionary<string, (float, float, float, float)>		Params4f		= new SortedDictionary<string, (float, float, float, float)>();
		private readonly SortedDictionary<string, float[]>							ParamsRGBColors	= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, float[]>							ParamsColors	= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, float[]>							ParamsVec2s		= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, float[]>							ParamsVec3s		= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, float[]>							ParamsVec4s		= new SortedDictionary<string, float[]>();
		private readonly SortedDictionary<string, (float[], bool)>					ParamsMat2		= new SortedDictionary<string, (float[], bool)>();
		private readonly SortedDictionary<string, (float[], bool)>					ParamsMat3		= new SortedDictionary<string, (float[], bool)>();
		private readonly SortedDictionary<string, (float[], bool)>					ParamsMat4		= new SortedDictionary<string, (float[], bool)>();
		private readonly SortedDictionary<string, (float[], bool)>					ParamsMat2s		= new SortedDictionary<string, (float[], bool)>();
		private readonly SortedDictionary<string, (float[], bool)>					ParamsMat3s		= new SortedDictionary<string, (float[], bool)>();
		private readonly SortedDictionary<string, (float[], bool)>					ParamsMat4s		= new SortedDictionary<string, (float[], bool)>();
		
		public Shader shader { get; set; }

		public Material() { }
		public Material(Shader shader) { this.shader = shader; }

		private void SetColors(string name, Color[] values, bool includeAlpha = true)
		{
			var colors = includeAlpha ? ParamsColors : ParamsRGBColors;
			colors[name] = values.serialize(colors.TryGetSafe(name), includeAlpha);
		}

		public void Set(string name, int value) => ParamsInt[name] = value;
		public void Set(string name, uint value) => ParamsUInt[name] = value;
		public void Set(string name, float value) => ParamsFloat[name] = value;
		public void Set(string name, int[] values) => ParamsInts[name] = values;
		public void Set(string name, uint[] values) => ParamsUInts[name] = values;
		public void Set(string name, float[] values) => ParamsFloats[name] = values;
		public void Set(string name, Color value) => ParamsColor[name] = value;
		public void Set(string name, vec2 value) => ParamsVec2[name] = value;
		public void Set(string name, vec3 value) => ParamsVec3[name] = value;
		public void Set(string name, vec4 value) => ParamsVec4[name] = value;
		public void Set(string name, float x, float y, float z) => Params3f[name] = (x, y, z);
		public void Set(string name, float x, float y, float z, float w) => Params4f[name] = (x, y, z, w);
		public void Set(string name, Color[] values, bool includeAlpha = true) => SetColors(name, values, includeAlpha);
		public void Set(string name, vec2[] values) => ParamsVec2s[name] = values.serialize(ParamsVec2s.TryGetSafe(name));
		public void Set(string name, vec3[] values) => ParamsVec3s[name] = values.serialize(ParamsVec3s.TryGetSafe(name));
		public void Set(string name, vec4[] values) => ParamsVec4s[name] = values.serialize(ParamsVec4s.TryGetSafe(name));
		public void Set(string name, mat2 value, bool transpose = false) => ParamsMat2[name] = (value.serialize(ParamsMat2.TryGetSafe(name).Item1), transpose);
		public void Set(string name, mat3 value, bool transpose = false) => ParamsMat3[name] = (value.serialize(ParamsMat3.TryGetSafe(name).Item1), transpose);
		public void Set(string name, mat4 value, bool transpose = false) => ParamsMat4[name] = (value.serialize(ParamsMat4.TryGetSafe(name).Item1), transpose);
		public void Set(string name, mat2[] values, bool transpose = false) => ParamsMat2s[name] = (values.serialize(ParamsMat2s.TryGetSafe(name).Item1), transpose);
		public void Set(string name, mat3[] values, bool transpose = false) => ParamsMat3s[name] = (values.serialize(ParamsMat3s.TryGetSafe(name).Item1), transpose);
		public void Set(string name, mat4[] values, bool transpose = false) => ParamsMat4s[name] = (values.serialize(ParamsMat4s.TryGetSafe(name).Item1), transpose);
		
		internal void Prepare()
		{
			if (shader == null) return;
			foreach (var v in ParamsInt) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsUInt) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsFloat) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsInts) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsUInts) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsFloats) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsColor) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec2) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec3) shader.Set(v.Key, v.Value);
			foreach (var v in ParamsVec4) shader.Set(v.Key, v.Value);
			foreach (var v in Params3f) shader.Set(v.Key, v.Value.Item1, v.Value.Item2, v.Value.Item2);
			foreach (var v in Params4f) shader.Set(v.Key, v.Value.Item1, v.Value.Item2, v.Value.Item3);
			foreach (var v in ParamsRGBColors) shader.SetRGBColors(v.Key, v.Value);
			foreach (var v in ParamsColors) shader.SetColors(v.Key, v.Value);
			foreach (var v in ParamsVec2s) shader.SetVec2s(v.Key, v.Value);
			foreach (var v in ParamsVec3s) shader.SetVec3s(v.Key, v.Value);
			foreach (var v in ParamsVec4s) shader.SetVec4s(v.Key, v.Value);
			foreach (var v in ParamsMat2) shader.SetMat2(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat3) shader.SetMat3(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat4) shader.SetMat4(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat2s) shader.SetMat2s(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat3s) shader.SetMat3s(v.Key, v.Value.Item1, v.Value.Item2);
			foreach (var v in ParamsMat4s) shader.SetMat4s(v.Key, v.Value.Item1, v.Value.Item2);
		}
	}
}
