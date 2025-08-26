using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PsConsoleLauncher.Models;
using PsConsoleLauncher.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace PsConsoleLauncher.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		public ObservableCollection<Game> Games { get; } = new ObservableCollection<Game>();
		public Game SelectedGame { get; set; }
		public ICommand PlayCommand { get; }
		public ICommand AddGameCommand { get; }

		private LaunchService _launcher = new LaunchService();

		public MainWindowViewModel()
		{
			PlayCommand = new RelayCommand(Play, () => SelectedGame != null);
			AddGameCommand = new RelayCommand(AddGame);
			// Seed sample
			Games.Add(new Game { Title = "Sample Game", Platform = "PC", ExecutablePath = "C:\\Windows\\notepad.exe", CoverPath = "assets/sample_cover.png" });
			SelectedGame = Games[0];
		}

		private void Play()
		{
			if (SelectedGame == null) return;
			_launcher.Launch(SelectedGame.ExecutablePath, SelectedGame.LaunchArgs);
		}

		private void AddGame()
		{
			// Simple OpenFileDialog flow - left for you to implement via ICommand binding and dialog service
		}
	}
}