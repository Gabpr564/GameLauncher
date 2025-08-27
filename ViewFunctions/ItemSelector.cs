using PsConsoleLauncher.Models;
using PsConsoleLauncher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsConsoleLauncher.ViewModels
{
	internal class ItemSelector
	{
		//Creates border around icons to know which icon the cursor is on.

        
		private List<Game> _games;
		private int _selectedIndex;

		public ItemSelector()
		{

			_games = GameLibraryService.LoadGames();
			_selectedIndex = 0;
		}

		public Game CurrentGame => _games[_selectedIndex];

		public void NextGame()
		{
			_selectedIndex = (_selectedIndex + 1) % _games.Count;
		}

		public void PreviousGame()
		{
			_selectedIndex = (_selectedIndex - 1 + _games.Count) % _games.Count;
		}

		public List<Game> GetAllGames() => _games;
	}
}

