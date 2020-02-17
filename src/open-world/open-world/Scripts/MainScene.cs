using System;
using System.Windows.Input;

using GlmNet;
using SharpGL;

using XEngine;
using XEngine.Core;
using XEngine.Data;
using XEngine.Shapes;
using XEngine.Models;
using XEngine.Shaders;

using Cursor = System.Windows.Forms.Cursor;
using Control = System.Windows.Forms.Control;
using MouseButtons = System.Windows.Forms.MouseButtons;

namespace open_world.Scripts
{
	[GenerateScene("OpenWorld.MainScene")]
	public class MainScene : Scene
	{
		#region Shader Program Properties

		private uint ShaderProgramId { get; set; }

		private int ProjectMatrixLocation { get; set; }
		private int ViewMatrixLocation { get; set; }
		private int TranslateMatrixLocation { get; set; }
		private int ScaleMatrixLocation { get; set; }
		private int RotateMatrixLocation { get; set; }

		private int MaterialColorLocation { get; set; }

		private int AmbientLightColorLocation { get; set; }
		private int AmbientLightPowerLocation { get; set; }

		private int LightSourcePositionLocation { get; set; }
		private int LightSourceColorLocation { get; set; }
		private int LightSourcePowerLocation { get; set; }

		private int EyePositionLocation { get; set; }

		private uint[] ArrayIds { get; } = new uint[1];
		private uint[] ModelBufferIds { get; } = new uint[2];

		#endregion

		private GeometricShape Shape { get; set; }

		private vec3 MaterialColor { get; } = new vec3(232 / 255f, 176 / 255f, 141 / 255f);

		private vec3 AmbientLightColor { get; } = new vec3(1.0f, 1.0f, 1.0f);
		private float AmbientLightPower { get; } = 0.25f;
		
		private vec3 LightSourcePosition { get; } = new vec3(-15.0f, 40.0f, 30.0f);
		private vec3 LightSourceColor { get; } = new vec3(1.0f, 1.0f, 1.0f);
		private float LightSourcePower { get; } = 60.0f; // Watts for instance

		private vec2 ShapeRotationAngle { get; set; } = new vec2(0.0f, 0.0f);
		private float Scroll { get; set; } = 0.0f;
		private vec2 LastMousePosition { get; set; } = new vec2(Cursor.Position.X, Cursor.Position.Y);

		private void LoadShader(OpenGL gl)
		{
			ShaderProgramId = 0;
			try { ShaderProgramId = ShaderProgram.Build(gl, "phong"); }
			catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, ex.GetType().FullName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); }
			if (ShaderProgramId == 0) return;

			ProjectMatrixLocation = gl.GetUniformLocation(ShaderProgramId, "project");
			ViewMatrixLocation = gl.GetUniformLocation(ShaderProgramId, "view");
			TranslateMatrixLocation = gl.GetUniformLocation(ShaderProgramId, "translate");
			ScaleMatrixLocation = gl.GetUniformLocation(ShaderProgramId, "scale");
			RotateMatrixLocation = gl.GetUniformLocation(ShaderProgramId, "rotate");

			MaterialColorLocation = gl.GetUniformLocation(ShaderProgramId, "material_color");

			AmbientLightColorLocation = gl.GetUniformLocation(ShaderProgramId, "ambient_light_color");
			AmbientLightPowerLocation = gl.GetUniformLocation(ShaderProgramId, "ambient_light_power");

			LightSourcePositionLocation = gl.GetUniformLocation(ShaderProgramId, "light_source_position");
			LightSourceColorLocation = gl.GetUniformLocation(ShaderProgramId, "light_source_color");
			LightSourcePowerLocation = gl.GetUniformLocation(ShaderProgramId, "light_source_power");

			EyePositionLocation = gl.GetUniformLocation(ShaderProgramId, "eye_position");
		}

		private void LoadSceneData(OpenGL gl)
		{
			var modelLoader = Model.Load("male_head");
			modelLoader.Wait();
			Shape = modelLoader.Result;
			Shape.Attributes = VertexAttribute.POSITION | VertexAttribute.NORMAL;
			
			gl.Enable(OpenGL.GL_DEPTH_TEST);
			gl.Enable(OpenGL.GL_CULL_FACE);

			gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

			gl.GenVertexArrays(ArrayIds.Length, ArrayIds);

			// Model data
			gl.BindVertexArray(ArrayIds[0]);
			gl.GenBuffers(ModelBufferIds.Length, ModelBufferIds);

			gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, ModelBufferIds[0]);
			gl.BufferData(OpenGL.GL_ARRAY_BUFFER, Shape.Data, OpenGL.GL_STATIC_DRAW);

			gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, ModelBufferIds[1]);
			gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, Shape.Indices, OpenGL.GL_STATIC_DRAW);

			gl.EnableVertexAttribArray(0);
			gl.EnableVertexAttribArray(1);
			gl.VertexAttribPointer(0, Shape.GetAttribSize(0), Shape.GetAttribType(0), Shape.ShouldAttribNormalize(0), Shape.GetAttribStride(0), Shape.GetAttribOffset(0));
			gl.VertexAttribPointer(1, Shape.GetAttribSize(1), Shape.GetAttribType(1), Shape.ShouldAttribNormalize(1), Shape.GetAttribStride(1), Shape.GetAttribOffset(1));
		}

		public override void Init(OpenGLControl control, float width, float height)
		{
			MainCamera = new Camera(new vec3(-30.0f, 20.0f, 30.0f));
			control.MouseWheel += (s, e) => Scroll += e.Delta / System.Windows.Forms.SystemInformation.MouseWheelScrollDelta;
			LoadShader(control.OpenGL);
			LoadSceneData(control.OpenGL);
			control.OpenGL.UseProgram(ShaderProgramId);
		}

		private void Update(OpenGLControl control)
		{
			if (!Host.CurrentApplicationIsActive) return;

			if (control.ClientRectangle.Contains(control.PointToClient(Cursor.Position)))
			{
				var delta = new vec2(Cursor.Position.X, Cursor.Position.Y) - LastMousePosition;

				// Rotate camera
				if (Control.MouseButtons.HasFlag(MouseButtons.Middle))
				{
					MainCamera.Rotate(delta);
				}

				// Rotate shape
				if (Control.MouseButtons.HasFlag(MouseButtons.Left))
				{
					ShapeRotationAngle += new vec2(delta.y, delta.x);
				}
			}

			// Move camera
			var moveDelta = new vec3(0.0f, 0.0f, 0.0f);

			if (Keyboard.IsKeyDown(Key.W)) moveDelta.z -= 1.0f;
			if (Keyboard.IsKeyDown(Key.S)) moveDelta.z += 1.0f;
			if (Keyboard.IsKeyDown(Key.A)) moveDelta.x -= 1.0f;
			if (Keyboard.IsKeyDown(Key.D)) moveDelta.x += 1.0f;
			if (Control.MouseButtons.HasFlag(MouseButtons.XButton1)) moveDelta.y -= 1.0f;
			if (Control.MouseButtons.HasFlag(MouseButtons.XButton2)) moveDelta.y += 1.0f;

			moveDelta.z -= Scroll;
			Scroll = 0.0f;

			MainCamera.Move(moveDelta);
		}

		private void LateUpdate(OpenGLControl control)
		{
			LastMousePosition = new vec2(Cursor.Position.X, Cursor.Position.Y);
		}

		public override void Draw(OpenGLControl control)
		{
			var gl = control.OpenGL;

			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
			gl.Viewport(0, 0, control.Width, control.Height);

			MainCamera.SetAspectRatio((float)control.Width / (float)control.Height);

			Update(control);
			LateUpdate(control);

			// Render Model
			gl.BindVertexArray(ArrayIds[0]);

			var translate = glm.translate(mat4.identity(), new vec3(+0.0f, +0.0f, +0.0f));
			var scale = glm.scale(mat4.identity(), new vec3(1.0f, 1.0f, 1.0f));
			var rotate = glm.rotate
			(
				glm.rotate
				(
					ShapeRotationAngle.y * (float)Math.PI / 180.0f, new vec3(+0.0f, +1.0f, +0.0f)
				),
					ShapeRotationAngle.x * (float)Math.PI / 180.0f, new vec3(+1.0f, +0.0f, +0.0f)
			);
			
			gl.UniformMatrix4(ProjectMatrixLocation, 1, false, MainCamera.ViewToProject.to_array());
			gl.UniformMatrix4(ViewMatrixLocation, 1, false, MainCamera.WorldToView.to_array());
			gl.UniformMatrix4(TranslateMatrixLocation, 1, false, translate.to_array());
			gl.UniformMatrix4(ScaleMatrixLocation, 1, false, scale.to_array());
			gl.UniformMatrix4(RotateMatrixLocation, 1, false, rotate.to_array());

			gl.Uniform3(MaterialColorLocation, MaterialColor.x, MaterialColor.y, MaterialColor.z);
			gl.Uniform3(AmbientLightColorLocation, AmbientLightColor.x, AmbientLightColor.y, AmbientLightColor.z);
			gl.Uniform1(AmbientLightPowerLocation, AmbientLightPower);
			gl.Uniform3(LightSourcePositionLocation, LightSourcePosition.x, LightSourcePosition.y, LightSourcePosition.z);
			gl.Uniform3(LightSourceColorLocation, LightSourceColor.x, LightSourceColor.y, LightSourceColor.z);
			gl.Uniform1(LightSourcePowerLocation, LightSourcePower);
			gl.Uniform3(EyePositionLocation, MainCamera.Position.x, MainCamera.Position.y, MainCamera.Position.z);

			gl.DrawElements(Shape.OpenGLShapeType, Shape.Indices.Length, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);
		}

		public override void Exit(OpenGLControl control)
		{
			var gl = control.OpenGL;
			gl.DeleteBuffers(ModelBufferIds.Length, ModelBufferIds);
			gl.DeleteVertexArrays(ArrayIds.Length, ArrayIds);
			gl.UseProgram(0);
			gl.DeleteProgram(ShaderProgramId);
		}
	}
}
