﻿using System;
using System.Threading.Tasks;
using SharpGL;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Data;
	using XEngine.Shapes;
	using XEngine.Models;
	using XEngine.Shading;
	using XEngine.Extensions;

	public sealed class Mesh : IDisposable
	{
		private readonly uint[] ArrayIds = new uint[1];
		private readonly uint[] BufferIds = new uint[2];

		public uint VertexArrayId => ArrayIds[0];
		public uint VertexBufferId => BufferIds[0];
		public uint IndexBufferId => BufferIds[1];

		private GeometricShape _shape = null;
		public GeometricShape shape
		{
			get
			{
				return _shape;
			}
			set
			{
				if (_shape == value) return;
				if (_shape != null) Dispose();
				_shape = value;
				Init();
			}
		}
		private Material _material = null;
		public Material material
		{
			get
			{
				return _material;
			}
			set
			{
				_material = value;
			}
		}

		public async Task LoadModel(string name, VertexAttribute attributes = VertexAttribute.ALL)
		{
			var model = await Model.Load(name);
			model.Attributes = attributes;
			shape = model;
		}

		private void Init()
		{
			if (ArrayIds[0] != 0u) throw new InvalidOperationException("Cannot reinitialize mesh while the last one was not disposed.");
			if (shape == null) throw new InvalidOperationException("Shape not provided.");

			var gl = XEngineContext.Graphics;
			gl.GenVertexArrays(ArrayIds.Length, ArrayIds);
			
			gl.BindVertexArray(ArrayIds[0]);
			gl.GenBuffers(BufferIds.Length, BufferIds);

			gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, BufferIds[0]);
			gl.BufferData(OpenGL.GL_ARRAY_BUFFER, shape.Data, OpenGL.GL_STATIC_DRAW);

			gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, BufferIds[1]);
			gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, shape.Indices, OpenGL.GL_STATIC_DRAW);

			for (var i = 0u; i < shape.AttribCount; ++i)
			{
				gl.VertexAttribPointer(i, shape.GetAttribSize(i), shape.GetAttribType(i), shape.ShouldAttribNormalize(i), shape.GetAttribStride(i), shape.GetAttribOffset(i));
				gl.EnableVertexAttribArray(i);
			}
		}

		public void Dispose()
		{
			if (ArrayIds[0] == 0u) throw new InvalidOperationException("Already disposed.");

			var gl = XEngineContext.Graphics;
			gl.DeleteBuffers(BufferIds.Length, BufferIds);
			gl.DeleteVertexArrays(ArrayIds.Length, ArrayIds);

			ArrayIds[0] = 0u;
		}

		public void Draw(Transform transform)
		{
			if (shape == null || material == null || material.shader == null || VertexArrayId == 0) return;
			var camera = SceneManager.CurrentScene.MainCamera;
			material.shader.Use();
			var gl = XEngineContext.Graphics;
			gl.BindVertexArray(VertexArrayId);
			if (material.shader.Project != -1) gl.UniformMatrix4(material.shader.Project, 1, false, camera.ViewToProject.to_array());
			if (material.shader.View != -1) gl.UniformMatrix4(material.shader.View, 1, false, camera.WorldToView.to_array());
			if (material.shader.Translate != -1) gl.UniformMatrix4(material.shader.Translate, 1, false, glm.translate(mat4.identity(), transform.position).to_array());
			if (material.shader.Scale != -1) gl.UniformMatrix4(material.shader.Scale, 1, false, glm.scale(mat4.identity(), transform.scale).to_array());
			if (material.shader.Rotate != -1) gl.UniformMatrix4(material.shader.Rotate, 1, false, glm.rotate(glm.rotate(glm.rotate(transform.rotation.y.ToRad(), vec.up), transform.rotation.x.ToRad(), vec.right), transform.rotation.z.ToRad(), vec.backward).to_array());
			if (material.shader.Eye != -1) gl.Uniform3(material.shader.Eye, camera.Position.x, camera.Position.y, camera.Position.z);
			material.Prepare();
			gl.DrawElements(shape.OpenGLShapeType, shape.Indices.Length, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);
		}
	}
}
