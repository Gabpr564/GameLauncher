using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using PsConsoleLauncher.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace PsConsoleLauncher.Services
{

	namespace PsConsoleLauncher.Services
	{
		public class DatabaseService
		{
			private string _dbPath = "launcher.db";
			public DatabaseService() { Initialize(); }
			private void Initialize()
			{
				using var conn = new SqliteConnection($"Data Source={_dbPath}");
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Games (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Title TEXT, Platform TEXT, ExecutablePath TEXT,
                                LaunchArgs TEXT, CoverPath TEXT, LastPlayed TEXT);
                              ";
				cmd.ExecuteNonQuery();
			}
			// Add minimal CRUD operations below (GetGames, AddGame, UpdateGame, RemoveGame)
		}
	}
}
