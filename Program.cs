using System;
using System.IO;
using System.Linq;

namespace DeleteTempFiles
{
	internal static class Program
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The main entry method along with any specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void Main(string[] args)
		{
			var processor = new Processor();
			try
			{
				Console.Title = "Deleting Temp Files";
				Console.SetBufferSize(150, 800);
				Console.SetWindowSize(80, 40);

				if (args.Length > 0)
					processor.ExcludedFolders = args[0].Trim('"').Split(';', ',', '|').ToList();

				// TODO: Read the excluded folder values from a file.

				string tempPath = Environment.GetEnvironmentVariable("TEMP");
				if (tempPath != null)
				{
					Console.WriteLine("\nProcessing: \"{0}\"\n", tempPath);

					if (!tempPath.ToUpper().EndsWith("TEMP"))
					{
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						Console.WriteLine("\nThe environment variable:\n");
						Console.ForegroundColor = ConsoleColor.White;
						Console.WriteLine("   TEMP=\"{0}\"\n", tempPath);
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						Console.WriteLine("does not appear to reference an actual 'temporary' folder!");
						DoExit();
						return;
					}
					processor.ClearFolder(new DirectoryInfo(tempPath)); // Execute ClearFolder() on the IE's cache folder
				}

				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("");
				Console.WriteLine("{0} folders skipped.", processor.NumFoldersSkipped);

				if (processor.NumItemsNotDeleted > 0)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("");
					Console.WriteLine("{0} folders and/or files could not be deleted.", processor.NumItemsNotDeleted);
				}

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("");
				Console.WriteLine("{0} folders and/or files deleted.", processor.NumFoldersDeleted + processor.NumFilesDeleted);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				DoExit();
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Does the exit.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void DoExit()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\n\nPress any key to continue...");
			Console.ResetColor();
			Console.ReadKey();
		}
	}
}