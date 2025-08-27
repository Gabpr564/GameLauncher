using Microsoft.Win32;
using Newtonsoft.Json;
using PsConsoleLauncher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsConsoleLauncher.Services
{
	public static class GameLibraryService
	{
		private static readonly string libraryFile = "games.json";

		public static List<Game> LoadGames()
		{
			if (!File.Exists(libraryFile))
				return new List<Game>();

			var json = File.ReadAllText(libraryFile);
			return JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
		}

		public static void SaveGames(List<Game> games)
		{
			var json = JsonConvert.SerializeObject(games, Formatting.Indented);
			File.WriteAllText(libraryFile, json);
		}
		public class GameMenuTie()
		{
			//Will make a list of the last played games to display in the main menu.

			
		
		

		}

	}
}