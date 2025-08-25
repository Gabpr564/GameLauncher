using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsConsoleLauncher.Models
{
	public class Game
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Platform { get; set; }
		public string ExecutablePath { get; set; }
		public string LaunchArgs { get; set; }
		public string CoverPath { get; set; }
		public DateTime? LastPlayed { get; set; }
	}
}
