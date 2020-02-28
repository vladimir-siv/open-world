using XEngine.Scripting;
using XEngine.Lighting;
using XEngine.Shading;
using XEngine.Common;

namespace open_world
{
	public class LightingController : XBehaviour
	{
		private float sky_duration;
		private float transition_duration;
		private float day_duration;

		private float dusk;
		private float midnight;
		private float dawn;
		private float noon;

		protected override void Start()
		{
			var sky = CurrentScene.Sky;

			sky_duration = sky.SkyboxDuration / sky.TransitionSpeed;
			transition_duration = sky.TransitionDuration / sky.TransitionSpeed;
			day_duration = (sky_duration + transition_duration) * sky.Cycle.SkyCount;

			var day_offset = day_duration * 0.05f;

			dusk = sky_duration - day_offset;
			midnight = sky_duration + transition_duration - day_offset;
			dawn = midnight + sky_duration + day_offset;
			noon = day_duration;
		}

		protected override void Update()
		{
			var sky = CurrentScene.Sky;
			var current_time = (CurrentScene.ElapsedTime / 1000.0f) % day_duration;

			if (dusk <= current_time && current_time <= midnight)
			{
				var k = (current_time - dusk) / (midnight - dusk);
				var ambient_intensity = algebra.lerp(25.0f, 10.0f, k);
				sky.AmbientLight = new AmbientLight(Color.White, ambient_intensity);
				var sun_intensity = algebra.lerp(100.0f, 0.0f, k);
				CurrentScene.SetLightIntensity("Sun", sun_intensity);
			}

			if (dawn <= current_time && current_time <= noon)
			{
				var k = (current_time - dawn) / (noon - dawn);
				var ambient_intensity = algebra.lerp(10.0f, 25.0f, k);
				sky.AmbientLight = new AmbientLight(Color.White, ambient_intensity);
				var sun_intensity = algebra.lerp(0.0f, 100.0f, k);
				CurrentScene.SetLightIntensity("Sun", sun_intensity);
			}
		}
	}
}
