using PsConsoleLauncher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsConsoleLauncher.Services
{
	public class GameScannerService
	{
		private readonly string _gamesFolder;

		public GameScannerService(string gamesFolder)
		{
			_gamesFolder = gamesFolder;
		}

		public List<Game> ScanGames()
		{
			var games = new List<Game>();

			if (!Directory.Exists(_gamesFolder))
				return games;

			foreach (var exe in Directory.GetFiles(_gamesFolder, "*.exe", SearchOption.AllDirectories))
			{
				var folder = Path.GetDirectoryName(exe);
				var title = Path.GetFileNameWithoutExtension(exe);

				// Look for cover image
				string cover = null;
				foreach (var ext in new[] { "jpg", "png" })
				{
					var file = Path.Combine(folder, $"cover.{ext}");
					if (File.Exists(file))
					{
						cover = file;
						break;
					}
				}

				games.Add(new Game
				{
					Title = title,
					Platform = "PC", // or detect later
					ExecutablePath = exe,
					CoverPath = cover ?? "Assets/placeholder.png"
				});
			}



			return games;
		}
	}
}
