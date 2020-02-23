using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpGL.SceneGraph.Assets;
using GlmNet;

namespace XEngine.Shading
{
    using XEngine.Common;

	public sealed class Material
	{
		private class ShaderDataCollection
		{
			private class Value
			{
				[StructLayout(LayoutKind.Explicit)]
				public struct InnerValue
				{
					[FieldOffset(0)] public int intv;
					[FieldOffset(0)] public uint uintv;
					[FieldOffset(0)] public float floatv;
					[FieldOffset(0)] public vec4 vecv;
					[FieldOffset(16)] public Array arrayv; // can be optimized with unsafe & IntPtr

					public InnerValue(int v)
					{
						uintv = 0u;
						floatv = 0.0f;
						arrayv = null;
						vecv = vector4.zero;

						intv = v;
					}
					public InnerValue(uint v)
					{
						intv = 0;
						floatv = 0.0f;
						arrayv = null;
						vecv = vector4.zero;

						uintv = v;
					}
					public InnerValue(float v)
					{
						intv = 0;
						uintv = 0u;
						arrayv = null;
						vecv = vector4.zero;

						floatv = v;
					}
					public InnerValue(Array v)
					{
						intv = 0;
						uintv = 0u;
						floatv = 0.0f;
						vecv = vector4.zero;

						arrayv = v;
					}
					public InnerValue(vec4 v)
					{
						intv = 0;
						uintv = 0u;
						floatv = 0.0f;
						arrayv = null;

						vecv = v;
					}

					public void Set(int v)
					{
						uintv = 0u;
						floatv = 0.0f;
						arrayv = null;
						vecv = vector4.zero;

						intv = v;
					}
					public void Set(uint v)
					{
						intv = 0;
						floatv = 0.0f;
						arrayv = null;
						vecv = vector4.zero;

						uintv = v;
					}
					public void Set(float v)
					{
						intv = 0;
						uintv = 0u;
						arrayv = null;
						vecv = vector4.zero;

						floatv = v;
					}
					public void Set(Array v)
					{
						intv = 0;
						uintv = 0u;
						floatv = 0.0f;
						vecv = vector4.zero;

						arrayv = v;
					}
					public void Set(vec4 v)
					{
						intv = 0;
						uintv = 0u;
						floatv = 0.0f;
						arrayv = null;

						vecv = v;
					}
				}

				public enum Type
				{
					NONE = 0,
					INT = 1,
					UINT = 2,
					FLOAT = 3,
					INT_ARRAY = 4,
					UINT_ARRAY = 5,
					FLOAT_ARRAY = 6,
					RGB = 7,
					RGBA = 8,
					VEC2 = 9,
					VEC3 = 10,
					VEC4 = 11,
					RGB_ARRAY = 12,
					RGBA_ARRAY = 13,
					VEC2_ARRAY = 14,
					VEC3_ARRAY = 15,
					VEC4_ARRAY = 16,
					MAT2 = 17,
					MAT2_T = 18,
					MAT3 = 19,
					MAT3_T = 20,
					MAT4 = 21,
					MAT4_T = 22,
					MAT2_ARRAY = 23,
					MAT2_T_ARRAY = 24,
					MAT3_ARRAY = 25,
					MAT3_T_ARRAY = 26,
					MAT4_ARRAY = 27,
					MAT4_T_ARRAY = 28
				}

				public InnerValue InnerVal;
				public Type ValType;

				#region Constructors

				public Value(int val) { InnerVal = new InnerValue(val); ValType = Type.INT; }
				public Value(uint val) { InnerVal = new InnerValue(val); ValType = Type.UINT; }
				public Value(float val) { InnerVal = new InnerValue(val); ValType = Type.FLOAT; }
				public Value(int[] val) { InnerVal = new InnerValue(val); ValType = Type.INT_ARRAY; }
				public Value(uint[] val) { InnerVal = new InnerValue(val); ValType = Type.UINT_ARRAY; }
				public Value(float[] val) { InnerVal = new InnerValue(val); ValType = Type.FLOAT_ARRAY; }
				public Value(Color val, bool rgb_only = false) { InnerVal = new InnerValue(val.vectorized); ValType = rgb_only ? Type.RGB : Type.RGBA; }
				public Value(vec2 val) { InnerVal = new InnerValue(new vec4(val.x, val.y, 0.0f, 1.0f)); ValType = Type.VEC2; }
				public Value(vec3 val) { InnerVal = new InnerValue(new vec4(val, 1.0f)); ValType = Type.VEC3; }
				public Value(vec4 val) { InnerVal = new InnerValue(val); ValType = Type.VEC4; }
				public Value(Color[] val, bool rgb_only = false) { InnerVal = new InnerValue(val.serialize(null, rgb_only)); ValType = rgb_only ? Type.RGB_ARRAY : Type.RGBA_ARRAY; }
				public Value(vec2[] val) { InnerVal = new InnerValue(val.serialize()); ValType = Type.VEC2_ARRAY; }
				public Value(vec3[] val) { InnerVal = new InnerValue(val.serialize()); ValType = Type.VEC3_ARRAY; }
				public Value(vec4[] val) { InnerVal = new InnerValue(val.serialize()); ValType = Type.VEC4_ARRAY; }
				public Value(mat2 val, bool transpose = false) { InnerVal = new InnerValue(val.serialize()); ValType = transpose ? Type.MAT2_T : Type.MAT2; }
				public Value(mat3 val, bool transpose = false) { InnerVal = new InnerValue(val.serialize()); ValType = transpose ? Type.MAT3_T : Type.MAT3; }
				public Value(mat4 val, bool transpose = false) { InnerVal = new InnerValue(val.serialize()); ValType = transpose ? Type.MAT4_T : Type.MAT4; }
				public Value(mat2[] val, bool transpose = false) { InnerVal = new InnerValue(val.serialize()); ValType = transpose ? Type.MAT2_T_ARRAY : Type.MAT2_ARRAY; }
				public Value(mat3[] val, bool transpose = false) { InnerVal = new InnerValue(val.serialize()); ValType = transpose ? Type.MAT3_T_ARRAY : Type.MAT3_ARRAY; }
				public Value(mat4[] val, bool transpose = false) { InnerVal = new InnerValue(val.serialize()); ValType = transpose ? Type.MAT4_T_ARRAY : Type.MAT4_ARRAY; }

				#endregion

				#region Setters

				public void Set(int val) { InnerVal.Set(val); ValType = Type.INT; }
				public void Set(uint val) { InnerVal.Set(val); ValType = Type.UINT; }
				public void Set(float val) { InnerVal.Set(val); ValType = Type.FLOAT; }
				public void Set(int[] val) { InnerVal.Set(val); ValType = Type.INT_ARRAY; }
				public void Set(uint[] val) { InnerVal.Set(val); ValType = Type.UINT_ARRAY; }
				public void Set(float[] val) { InnerVal.Set(val); ValType = Type.FLOAT_ARRAY; }
				public void Set(Color val, bool rgb_only = false) { InnerVal.Set(val.vectorized); ValType = rgb_only ? Type.RGB : Type.RGBA; }
				public void Set(vec2 val) { InnerVal.Set(new vec4(val.x, val.y, 0.0f, 1.0f)); ValType = Type.VEC2; }
				public void Set(vec3 val) { InnerVal.Set(new vec4(val, 1.0f)); ValType = Type.VEC3; }
				public void Set(vec4 val) { InnerVal.Set(val); ValType = Type.VEC4; }
				public void Set(Color[] val, bool rgb_only = false, bool keep_layout = true) { var t = rgb_only ? Type.RGB_ARRAY : Type.RGBA_ARRAY; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null, rgb_only)); ValType = t; }
				public void Set(vec2[] val, bool keep_layout = true) { InnerVal.Set(val.serialize(ValType == Type.VEC2_ARRAY && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = Type.VEC2_ARRAY; }
				public void Set(vec3[] val, bool keep_layout = true) { InnerVal.Set(val.serialize(ValType == Type.VEC3_ARRAY && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = Type.VEC3_ARRAY; }
				public void Set(vec4[] val, bool keep_layout = true) { InnerVal.Set(val.serialize(ValType == Type.VEC4_ARRAY && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = Type.VEC4_ARRAY; }
				public void Set(mat2 val, bool transpose = false, bool keep_layout = true) { var t = transpose ? Type.MAT2_T : Type.MAT2; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = t; }
				public void Set(mat3 val, bool transpose = false, bool keep_layout = true) { var t = transpose ? Type.MAT3_T : Type.MAT3; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = t; }
				public void Set(mat4 val, bool transpose = false, bool keep_layout = true) { var t = transpose ? Type.MAT4_T : Type.MAT4; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = t; }
				public void Set(mat2[] val, bool transpose = false, bool keep_layout = true) { var t = transpose ? Type.MAT2_T_ARRAY : Type.MAT2_ARRAY; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = t; }
				public void Set(mat3[] val, bool transpose = false, bool keep_layout = true) { var t = transpose ? Type.MAT3_T_ARRAY : Type.MAT3_ARRAY; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = t; }
				public void Set(mat4[] val, bool transpose = false, bool keep_layout = true) { var t = transpose ? Type.MAT4_T_ARRAY : Type.MAT4_ARRAY; InnerVal.Set(val.serialize(ValType == t && keep_layout ? (float[])InnerVal.arrayv : null)); ValType = t; }

				#endregion
			}

			private readonly Dictionary<string, Value> Values = new Dictionary<string, Value>();
			private readonly LinkedList<string> Keys = new LinkedList<string>();

			#region Setters

			public void Set(string name, int val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, uint val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, float val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, int[] val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, uint[] val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, float[] val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, Color val, bool rgb_only = false) { if (Values.TryGetValue(name, out var v)) { v.Set(val, rgb_only); } else { Values.Add(name, new Value(val, rgb_only)); Keys.AddLast(name); } }
			public void Set(string name, vec2 val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, vec3 val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, vec4 val) { if (Values.TryGetValue(name, out var v)) { v.Set(val); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, Color[] val, bool rgb_only = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, rgb_only, keep_layout); } else { Values.Add(name, new Value(val, rgb_only)); Keys.AddLast(name); } }
			public void Set(string name, vec2[] val, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, keep_layout); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, vec3[] val, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, keep_layout); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, vec4[] val, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, keep_layout); } else { Values.Add(name, new Value(val)); Keys.AddLast(name); } }
			public void Set(string name, mat2 val, bool transpose = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, transpose, keep_layout); } else { Values.Add(name, new Value(val, transpose)); Keys.AddLast(name); } }
			public void Set(string name, mat3 val, bool transpose = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, transpose, keep_layout); } else { Values.Add(name, new Value(val, transpose)); Keys.AddLast(name); } }
			public void Set(string name, mat4 val, bool transpose = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, transpose, keep_layout); } else { Values.Add(name, new Value(val, transpose)); Keys.AddLast(name); } }
			public void Set(string name, mat2[] val, bool transpose = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, transpose, keep_layout); } else { Values.Add(name, new Value(val, transpose)); Keys.AddLast(name); } }
			public void Set(string name, mat3[] val, bool transpose = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, transpose, keep_layout); } else { Values.Add(name, new Value(val, transpose)); Keys.AddLast(name); } }
			public void Set(string name, mat4[] val, bool transpose = false, bool keep_layout = true) { if (Values.TryGetValue(name, out var v)) { v.Set(val, transpose, keep_layout); } else { Values.Add(name, new Value(val, transpose)); Keys.AddLast(name); } }

			#endregion

			public void Prepare(Shader shader)
			{
				if (shader == null) return;
				
				foreach (var key in Keys)
				{
					var val = Values[key];

					switch (val.ValType)
					{
						case Value.Type.INT:			shader.SetScalar(key, val.InnerVal.intv); break;
						case Value.Type.UINT:			shader.SetScalar(key, val.InnerVal.uintv); break;
						case Value.Type.FLOAT:			shader.SetScalar(key, val.InnerVal.floatv); break;
						case Value.Type.INT_ARRAY:		shader.SetScalarArray(key, (int[])val.InnerVal.arrayv); break;
						case Value.Type.UINT_ARRAY:		shader.SetScalarArray(key, (uint[])val.InnerVal.arrayv); break;
						case Value.Type.FLOAT_ARRAY:	shader.SetScalarArray(key, (float[])val.InnerVal.arrayv); break;
						case Value.Type.RGB:			shader.SetVec3(key, val.InnerVal.vecv.x, val.InnerVal.vecv.y, val.InnerVal.vecv.z); break;
						case Value.Type.RGBA:			shader.SetVec4(key, val.InnerVal.vecv.x, val.InnerVal.vecv.y, val.InnerVal.vecv.z, val.InnerVal.vecv.w); break;
						case Value.Type.VEC2:			shader.SetVec2(key, val.InnerVal.vecv.x, val.InnerVal.vecv.y); break;
						case Value.Type.VEC3:			shader.SetVec3(key, val.InnerVal.vecv.x, val.InnerVal.vecv.y, val.InnerVal.vecv.z); break;
						case Value.Type.VEC4:			shader.SetVec4(key, val.InnerVal.vecv.x, val.InnerVal.vecv.y, val.InnerVal.vecv.z, val.InnerVal.vecv.w); break;
						case Value.Type.RGB_ARRAY:		shader.SetVec3Array(key, (float[])val.InnerVal.arrayv); break;
						case Value.Type.RGBA_ARRAY:		shader.SetVec4Array(key, (float[])val.InnerVal.arrayv); break;
						case Value.Type.VEC2_ARRAY:		shader.SetVec2Array(key, (float[])val.InnerVal.arrayv); break;
						case Value.Type.VEC3_ARRAY:		shader.SetVec3Array(key, (float[])val.InnerVal.arrayv); break;
						case Value.Type.VEC4_ARRAY:		shader.SetVec4Array(key, (float[])val.InnerVal.arrayv); break;
						case Value.Type.MAT2:			shader.SetMat2(key, (float[])val.InnerVal.arrayv, false); break;
						case Value.Type.MAT2_T:			shader.SetMat2(key, (float[])val.InnerVal.arrayv, true); break;
						case Value.Type.MAT3:			shader.SetMat3(key, (float[])val.InnerVal.arrayv, false); break;
						case Value.Type.MAT3_T:			shader.SetMat3(key, (float[])val.InnerVal.arrayv, true); break;
						case Value.Type.MAT4:			shader.SetMat4(key, (float[])val.InnerVal.arrayv, false); break;
						case Value.Type.MAT4_T:			shader.SetMat4(key, (float[])val.InnerVal.arrayv, true); break;
						case Value.Type.MAT2_ARRAY:		shader.SetMat2Array(key, (float[])val.InnerVal.arrayv, false); break;
						case Value.Type.MAT2_T_ARRAY:	shader.SetMat2Array(key, (float[])val.InnerVal.arrayv, true); break;
						case Value.Type.MAT3_ARRAY:		shader.SetMat3Array(key, (float[])val.InnerVal.arrayv, false); break;
						case Value.Type.MAT3_T_ARRAY:	shader.SetMat3Array(key, (float[])val.InnerVal.arrayv, true); break;
						case Value.Type.MAT4_ARRAY:		shader.SetMat4Array(key, (float[])val.InnerVal.arrayv, false); break;
						case Value.Type.MAT4_T_ARRAY:	shader.SetMat4Array(key, (float[])val.InnerVal.arrayv, true); break;
						default: break;
					}
				}
			}
		}

		private readonly ShaderDataCollection DataCollection = new ShaderDataCollection();
		
		public Shader shader { get; set; }
		public Texture texture { get; set; }

		public bool CullFace { get; set; } = true;

		public Material() { }
		public Material(Shader shader) { this.shader = shader; }

		#region Setters

		public void Set(string name, bool val) => Set(name, val ? 1.0f : 0.0f);
		public void Set(string name, int val) => DataCollection.Set(name, val);
		public void Set(string name, uint val) => DataCollection.Set(name, val);
		public void Set(string name, float val) => DataCollection.Set(name, val);
		public void Set(string name, int[] val) => DataCollection.Set(name, val);
		public void Set(string name, uint[] val) => DataCollection.Set(name, val);
		public void Set(string name, float[] val) => DataCollection.Set(name, val);
		public void Set(string name, Color val, bool rgb_only = false) => DataCollection.Set(name, val, rgb_only);
		public void Set(string name, vec2 val) => DataCollection.Set(name, val);
		public void Set(string name, vec3 val) => DataCollection.Set(name, val);
		public void Set(string name, vec4 val) => DataCollection.Set(name, val);
		public void Set(string name, Color[] val, bool rgb_only = false, bool keep_layout = true) => DataCollection.Set(name, val, rgb_only, keep_layout);
		public void Set(string name, vec2[] val, bool keep_layout = true) => DataCollection.Set(name, val, keep_layout);
		public void Set(string name, vec3[] val, bool keep_layout = true) => DataCollection.Set(name, val, keep_layout);
		public void Set(string name, vec4[] val, bool keep_layout = true) => DataCollection.Set(name, val, keep_layout);
		public void Set(string name, mat2 val, bool transpose = false, bool keep_layout = true) => DataCollection.Set(name, val, transpose, keep_layout);
		public void Set(string name, mat3 val, bool transpose = false, bool keep_layout = true) => DataCollection.Set(name, val, transpose, keep_layout);
		public void Set(string name, mat4 val, bool transpose = false, bool keep_layout = true) => DataCollection.Set(name, val, transpose, keep_layout);
		public void Set(string name, mat2[] val, bool transpose = false, bool keep_layout = true) => DataCollection.Set(name, val, transpose, keep_layout);
		public void Set(string name, mat3[] val, bool transpose = false, bool keep_layout = true) => DataCollection.Set(name, val, transpose, keep_layout);
		public void Set(string name, mat4[] val, bool transpose = false, bool keep_layout = true) => DataCollection.Set(name, val, transpose, keep_layout);

		#endregion

		internal void Prepare()
		{
			if (shader != null && shader.PrepareNeeded(this, markPrepared: true))
			{
				DataCollection.Prepare(shader);
			}

			texture?.Bind(XEngineContext.Graphics);
		}
	}
}
