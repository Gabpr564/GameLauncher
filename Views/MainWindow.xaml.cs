using PsConsoleLauncher.Models;
using PsConsoleLauncher.Services;
using PsConsoleLauncher.ViewFunctions;
using PsConsoleLauncher.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;


namespace PsConsoleLauncher.Views
{
	public partial class MainWindow : Window
	{
		public ObservableCollection<GameViewModel> Games { get; } = new();
		private ItemSelector _selector;
		private int _selectedIndex = -1;

		// Optional: Controller timer
		private DispatcherTimer _controllerTimer;

		public MainWindow()
		{
			
		    
			// InitializeComponent();
			DataContext = this;

			// Load games from your library or seed sample
			var loaded = GameLibraryService.LoadGames();
			if (loaded == null || loaded.Count == 0)
			{
				// Remove and tie to game library service
				loaded = new System.Collections.Generic.List<Game>
				{
					new Game { Title = "Notepad (Demo)", Platform = "PC", ExecutablePath = "notepad.exe", CoverPath = "Themes/Default/assets/sample_cover.png" }
				};
			}

			foreach (var g in loaded)
				Games.Add(new GameViewModel(g));

			// select first
			if (Games.Count > 0) SelectIndex(0);

			// Controller polling (optional, disabled by default)
			// StartControllerPolling();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Focus window so keyboard input works immediately
			Keyboard.Focus(this);
		}

		#region Selection & UI sync
		private void SelectIndex(int idx)
		{
			if (idx < 0 || idx >= Games.Count) return;

			// Clear prev
			if (_selectedIndex >= 0 && _selectedIndex < Games.Count)
				Games[_selectedIndex].IsSelected = false;

			_selectedIndex = idx;
			Games[_selectedIndex].IsSelected = true;

			// Scroll tile into view
			var ic = FindName("TileItemsControl") as ItemsControl;
			var container = ic?.ItemContainerGenerator.ContainerFromIndex(_selectedIndex) as FrameworkElement;
			container?.BringIntoView();


			// Update details
			var gvm = Games[_selectedIndex];
			if (FindName("DetailTitle") is TextBlock detailTitle) detailTitle.Text = gvm.Title;
			if (FindName("DetailPlatform") is TextBlock detailPlatform) detailPlatform.Text = gvm.Platform;
			if (FindName("DetailPath") is TextBlock detailPath) detailPath.Text = gvm.ExecutablePath;
		}
		#endregion

		#region Keyboard navigation
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Right)
			{
				MoveRight();
				e.Handled = true;
				return;
			}
			if (e.Key == Key.Left)
			{
				MoveLeft();
				e.Handled = true;
				return;
			}
			if (e.Key == Key.Enter)
			{
				PlaySelected();
				e.Handled = true;
				return;
			}
			if (e.Key == Key.Escape)
			{
				// optionally exit fullscreen or app
				Application.Current.Shutdown();
			}
		}

		private void MoveRight()
		{
			if (Games.Count == 0) return;
			var next = Math.Min(Games.Count - 1, _selectedIndex + 1);
			SelectIndex(next);
		}

		private void MoveLeft()
		{
			if (Games.Count == 0) return;
			var next = Math.Max(0, _selectedIndex - 1);
			SelectIndex(next);
		}
		#endregion

		#region Mouse handlers
		private void Tile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var border = sender as Border;
			if (border?.Tag is GameViewModel gvm)
			{
				var idx = Games.IndexOf(gvm);
				if (idx >= 0) SelectIndex(idx);
				// double-click behavior is not needed here; single click selects
			}
		}
		#endregion

		#region Play / Add
		private void PlayButton_Click(object sender, RoutedEventArgs e) => PlaySelected();

		private void AddGameButton_Click(object sender, RoutedEventArgs e)
		{
			// Simple Add dialog: open file dialog to pick an exe and add to collection
			var ofd = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Select executable",
				Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
			};
			var res = ofd.ShowDialog();
			if (res == true)
			{
				var g = new Game
				{
					Title = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName),
					Platform = "PC",
					ExecutablePath = ofd.FileName,
					CoverPath = "Themes/Default/assets/sample_cover.png"
				};
				Games.Add(new GameViewModel(g));
				// Persist to disk
				GameLibraryService.SaveGames(Games.Select(x => x.ToModel()).ToList());
			}
		}

		private void PlaySelected()
		{
			if (_selectedIndex < 0 || _selectedIndex >= Games.Count) return;
			var g = Games[_selectedIndex].ToModel();
			try
			{
				var psi = new ProcessStartInfo
				{
					FileName = g.ExecutablePath,
					UseShellExecute = true,
					WorkingDirectory = System.IO.Path.GetDirectoryName(g.ExecutablePath)
				};
				Process.Start(psi);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to launch:\n{ex.Message}");
			}
		}
		#endregion

		#region Controller support (optional)
		// Below is a minimal controller polling setup.
		// It uses a DispatcherTimer to poll XInput. To enable, uncomment the StartControllerPolling() call in ctor.
		private void StartControllerPolling()
		{
			_controllerTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(90) };
			_controllerTimer.Tick += ControllerTimer_Tick;
			_controllerTimer.Start();
		}

		private DateTime _lastMove = DateTime.MinValue;
		private void ControllerTimer_Tick(object sender, EventArgs e)
		{
			// Poll XInput (see ControllerHelper below). This is non-blocking.
			var state = ControllerHelper.GetState(0); // player 0
			if (!state.Connected) return;

			// DPad Left/Right or left thumb X
			var now = DateTime.UtcNow;
			if ((state.DPadLeft || state.LeftThumbX < -0.5f) && (now - _lastMove).TotalMilliseconds > 180)
			{
				MoveLeft();
				_lastMove = now;
			}
			else if ((state.DPadRight || state.LeftThumbX > 0.5f) && (now - _lastMove).TotalMilliseconds > 180)
			{
				MoveRight();
				_lastMove = now;
			}

			// A button -> Enter
			if (state.ButtonA)
			{
				PlaySelected();
			}
		}
		// Cursor
		private void ItemSelector()
		{

			var libraryService = new GameLibraryService.GameMenuTie();
			_selector = new ItemSelector();

			DataContext = _selector;
		}
		#endregion
	}


	// lightweight ViewModel for a selection flag without adding heavy MVVM dependencies
	public class GameViewModel
	{
		public Game Model { get; }
		public string Title => Model.Title;
		public string Platform => Model.Platform;
		public string ExecutablePath => Model.ExecutablePath;
		public string CoverPath => Model.CoverPath;

		public bool IsSelected { get; set; } = false;

		public GameViewModel(Game g) { Model = g; }
		public Game ToModel() => Model;
	}
	// Add game icon selector as well as functionallity to the buttons. Also add top row buttons.
}
