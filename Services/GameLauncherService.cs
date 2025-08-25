using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsConsoleLauncher.Services
{
	internal class GameLauncher
	{
		public class GameManager
		{
			private readonly string gameDirectory;

			public GameManager(string gameDir)
			{
				gameDirectory = gameDir;
			}

			// Get list of available games
			public List<string> GetAvailableGames()
			{
				var games = new List<string>();

				if (Directory.Exists(gameDirectory))
				{
					string[] executables = Directory.GetFiles(gameDirectory, "*.exe", SearchOption.TopDirectoryOnly);

					foreach (var exe in executables)
					{
						games.Add(Path.GetFileNameWithoutExtension(exe));
					}
				}
				else
				{
					Directory.CreateDirectory(gameDirectory); // ensure folder exists
				}

				return games;
			}

			// Launch a game by name
			public void LaunchGame(string gameName)
			{
				string exePath = Path.Combine(gameDirectory, gameName + ".exe");

				if (File.Exists(exePath))
				{
					try
					{
						Process.Start(exePath);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to launch {gameName}: {ex.Message}");
					}
				}
				else
				{
					MessageBox.Show($"{gameName} not found!");
				}
			}
		}
	}
}
