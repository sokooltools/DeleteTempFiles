using System;
using System.IO;
using System.Linq;

namespace DeleteTempFiles
{
	internal static class Program
	{
		private const string HORIZ_LINE = "===============================================================================";

		//----------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Serves as the entry point for the application, performing the deletion of temporary files and directories from 
		/// system-defined temporary folders.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method processes the system's temporary folder and the Windows Temp folder, deleting files and directories 
		/// while skipping excluded folders specified via command-line arguments. The method also displays the results of the 
		/// operation, including the number of items deleted and any items that could not be deleted.
		/// </para>
		/// <para>
		/// Command-line arguments can be used to specify folders to exclude from processing. These should be provided as a 
		/// single string, with folder paths separated by semicolons, commas, or pipe characters.
		/// </para>
		/// <para> The method ensures that the TEMP environment variable references a valid temporary folder before proceeding. 
		/// If the TEMP variable does not point to a folder ending with "TEMP", the operation is aborted with a warning 
		/// message.
		/// </para>
		/// </remarks>
		/// <param name="args">
		/// An array of command-line arguments. The first argument, if provided, specifies folders to exclude from processing.
		/// Folder paths should be separated by semicolons, commas, or pipe characters.
		/// </param>
		//----------------------------------------------------------------------------------------------------------------------
		private static void Main(string[] args)
		{
			var processor = new Processor();
			try
			{
				Console.Title = "Deleting Temp Files";
				Console.SetBufferSize(150, 800);
				Console.SetWindowSize(80, 40);

				// Ensure console output is flushed immediately as lines are written.
				Stream stdout = Console.OpenStandardOutput();
				Console.SetOut(new StreamWriter(stdout) { AutoFlush = true });
				Stream stderr = Console.OpenStandardError();
				Console.SetError(new StreamWriter(stderr) { AutoFlush = true });

				if (args.Length > 0)
					processor.ExcludedFolders = args[0].Trim('"').Split(';', ',', '|').ToList();

				// TODO: Read the excluded folder values from a file.

				string tempPath = Environment.GetEnvironmentVariable("TEMP");
				if (tempPath != null)
				{
					WriteBanner($"Processing: \"{tempPath}\"");

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

				string winTemp = Path.Combine(Environment.GetEnvironmentVariable("WINDIR") ?? String.Empty, "Temp");
				if (Directory.Exists(winTemp)) // Empty directory does not exist on all systems
				{
					Console.WriteLine("");
					Console.WriteLine("");
					WriteBanner($"Processing: \"{winTemp}\"");

					processor.ClearFolder(new DirectoryInfo(winTemp));
					Console.ForegroundColor = ConsoleColor.DarkYellow;
				}

				Console.WriteLine("");
				Console.WriteLine("");
				WriteBanner("Results:");

				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("\t{0} folders skipped", processor.NumFoldersSkipped);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("");
				Console.WriteLine("\t{0} folders and/or files deleted", processor.NumFoldersDeleted + processor.NumFilesDeleted);

				if (processor.NumItemsNotDeleted > 0)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("");
					Console.WriteLine("\t{0} folders and/or files could not be deleted", processor.NumItemsNotDeleted);
				}

				Console.WriteLine("");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(HORIZ_LINE);
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

		//----------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Displays a banner with the specified text, enclosed between horizontal lines.
		/// </summary>
		/// <remarks>
		/// The banner is displayed in white text on the console, with a horizontal line above and below the specified text.
		/// </remarks>
		/// <param name="text">The text to display within the banner. Cannot be null or empty.</param>
		//----------------------------------------------------------------------------------------------------------------------
		private static void WriteBanner(string text)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(HORIZ_LINE);
			Console.WriteLine(text);
			Console.WriteLine(HORIZ_LINE);
			Console.WriteLine("");
		}

		//----------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Does the exit.
		/// </summary>
		//----------------------------------------------------------------------------------------------------------------------
		private static void DoExit()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\n\nPress any key to continue...");
			Console.ResetColor();
			Console.ReadKey();
		}
	}
}