using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DeleteTempFiles
{
	internal static class Program
	{
		private static int _iFilesDeleted;
		private static int _iFoldersSkipped;
		private static int _iFoldersDeleted;
		private static int _iFilesFoldersNotDeleted;

		private const string SKIPPED = "            [Skipped]";
		private const string DELETED = "            [Deleted]";
		private const string COULD_NOT_DELETE = "   [Could not delete]";

		[DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool PathCompactPathEx(StringBuilder pszOut, string pszSrc, int cchMax, int dwFlags);

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The list of all folders which will be excluded (skipped) during deletion.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static List<string> _excludedFolders;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The main entry method along with any specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void Main(string[] args)
		{
			try
			{
				Console.Title = "Deleting Temp Files";
				Console.SetBufferSize(150, 800);
				Console.SetWindowSize(80, 40);

				if (args.Length > 0)
					_excludedFolders = args[0].Trim('"').Split(';', ',', '|').ToList();

				// TODO: Read the excluded folder values from a file.

				//string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

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
					ClearFolder(new DirectoryInfo(tempPath)); // Execute ClearFolder() on the IE's cache folder
				}

				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("");
				Console.WriteLine("{0} folders skipped.", _iFoldersSkipped);

				if (_iFilesFoldersNotDeleted > 0)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("");
					Console.WriteLine("{0} folders and/or files could not be deleted.", _iFilesFoldersNotDeleted);
				}

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("");
				Console.WriteLine("{0} folders and/or files deleted.", _iFoldersDeleted + _iFilesDeleted);
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

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Clears the folder.
		/// </summary>
		/// <param name="dirInfo">The directory info object.</param>
		//------------------------------------------------------------------------------------------------------------------------
		private static void ClearFolder(DirectoryInfo dirInfo)
		{
			Console.ResetColor();

			// First delete all the files in the folder.
			foreach (FileInfo fiCurrFile in dirInfo.GetFiles())
			{
				try
				{
					Console.ResetColor();
					Console.Write("Deleting File: {0}", TruncatePath(fiCurrFile.Name, 43).PadRight(43));
					fiCurrFile.Delete();
					//FileSystem.DeleteFile(fiCurrFile.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(DELETED);
					_iFilesDeleted++;
				}
				catch (Exception)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(COULD_NOT_DELETE);
					Console.ResetColor();
					_iFilesFoldersNotDeleted++;
				}
			}

			foreach (DirectoryInfo diSubFolder in dirInfo.GetDirectories())
			{
				// Next delete each sub-folder in the folder.
				if (_excludedFolders.All(x => x != Path.GetFileName(diSubFolder.FullName)))
				{
					ClearFolder(diSubFolder); // Call recursively for all subfolders
				}
				try
				{
					Console.ResetColor();
					Console.Write("Deleting SDir: {0}", TruncatePath(diSubFolder.FullName, 43).PadRight(43));

					if (_excludedFolders.All(x => x != Path.GetFileName(diSubFolder.FullName)))
					{
						diSubFolder.Delete(false);
						//FileSystem.DeleteDirectory(diSubFolder.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine(DELETED);
						_iFoldersDeleted++;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						Console.WriteLine(SKIPPED);
						_iFoldersSkipped++;
					}
				}
				catch (Exception)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(COULD_NOT_DELETE);
					Console.ResetColor();
					_iFilesFoldersNotDeleted++;
				}
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Truncates the specified path if it exceeds the specified length.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="maxLength">The length.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static string TruncatePath(string path, int maxLength)
		{
			var sb = new StringBuilder(260);
			PathCompactPathEx(sb, path, maxLength + 1, 0);
			return sb.ToString();
		}
	}
}