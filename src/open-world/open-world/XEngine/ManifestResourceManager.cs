using System.Reflection;
using System.IO;

namespace XEngine
{
	public static class ManifestResourceManager
	{
		public static string LoadShader(string shaderName) => LoadFile($"Shaders/{shaderName}.glsl");
		public static Stream LoadFromResources(string resource) => Load($"Resources/{resource}");

		public static string LoadFile(string fileName)
		{
			using (var stream = Load(fileName))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
		public static Stream Load(string resourceName)
		{
			var executingAssembly = Assembly.GetExecutingAssembly();
			var pathToDots = resourceName.Replace("\\", ".").Replace("/", ".");
			var location = string.Format("{0}.{1}", executingAssembly.GetName().Name.Replace('-', '_'), pathToDots);
			return executingAssembly.GetManifestResourceStream(location);
		}
	}
}
