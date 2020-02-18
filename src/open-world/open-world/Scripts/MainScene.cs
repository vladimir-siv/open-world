﻿using GlmNet;
using SharpGL;

using XEngine;
using XEngine.Core;
using XEngine.Data;
using XEngine.Shading;

namespace open_world.Scripts
{
	using Behaviours;

	[GenerateScene("OpenWorld.MainScene")]
	public class MainScene : Scene
	{
		private GameObject MaleHead;
		private GameObject UserController;

		protected override void Init()
		{
			var gl = XEngineContext.Graphics;
			gl.Enable(OpenGL.GL_DEPTH_TEST);
			gl.Enable(OpenGL.GL_CULL_FACE);
			gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

			MainCamera.Position = new vec3(-30.0f, 20.0f, 30.0f);
			MainCamera.LookAt(new vec3(0.0f, 0.0f, 0.0f));

			MaleHead = new GameObject("MaleHead");
			MaleHead.mesh = new Mesh();
			MaleHead.mesh.LoadModel("male_head", VertexAttribute.POSITION | VertexAttribute.NORMAL).Wait();
			MaleHead.mesh.material = new Material(Shader.Find("phong"));
			MaleHead.mesh.material.Set("material_color", new vec3(232 / 255f, 176 / 255f, 141 / 255f));
			MaleHead.mesh.material.Set("ambient_light_color", new vec3(1.0f, 1.0f, 1.0f));
			MaleHead.mesh.material.Set("ambient_light_power", 0.25f);
			MaleHead.mesh.material.Set("light_source_position", new vec3(-15.0f, 40.0f, 30.0f));
			MaleHead.mesh.material.Set("light_source_color", new vec3(1.0f, 1.0f, 1.0f));
			MaleHead.mesh.material.Set("light_source_power", 60.0f);

			UserController = new GameObject("UserController");
			UserController.AttachBehavior(new UserController { MaleHead = MaleHead });
		}

		protected override void Draw()
		{
			var gl = XEngineContext.Graphics;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
			gl.Viewport(0, 0, XEngineContext.GLControl.Width, XEngineContext.GLControl.Height);
			
			MaleHead.Sync();
			MaleHead.Draw();
			UserController.Sync();
			UserController.Draw();
		}
	}
}
