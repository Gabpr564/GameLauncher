using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace PsConsoleLauncher.Services
{
	public class LaunchService
	{
		public void Launch(string path, string args = null)
		{
			var psi = new ProcessStartInfo
			{
				FileName = path,
				Arguments = args ?? string.Empty,
				UseShellExecute = true,
				WorkingDirectory = System.IO.Path.GetDirectoryName(path)
			};
			Process.Start(psi);
		}
	}
}