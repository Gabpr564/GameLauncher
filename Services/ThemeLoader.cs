using System.IO;
using System.Text.Json;
namespace PsConsoleLauncher.Services
{
	public class ThemeManifest
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		public string Version { get; set; }
		public string DefaultMusic { get; set; }
	}

	public class ThemeLoader
	{
		public ThemeManifest Load(string folder)
		{
			var manifestPath = Path.Combine(folder, "manifest.json");
			if (!File.Exists(manifestPath)) return null;
			var json = File.ReadAllText(manifestPath);
			return JsonSerializer.Deserialize<ThemeManifest>(json);
		}
	}
}
