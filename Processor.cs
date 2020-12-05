using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DeleteTempFiles
{
	public sealed class Processor
	{
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number folders skipped.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public int NumFoldersSkipped { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number folders deleted.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public int NumFoldersDeleted { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number files deleted.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public int NumFilesDeleted { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number of items not deleted.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public int NumItemsNotDeleted { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The list of all folders which will be excluded (skipped) during deletion.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public List<string> ExcludedFolders { get; set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="Processor"/> class.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public Processor()
		{
			ExcludedFolders = new List<string>();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Recursively clears the specified folder of all its sub-folders and files.
		/// </summary>
		/// <param name="dirInfo">The directory info object.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public void ClearFolder(DirectoryInfo dirInfo)
		{
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
					NumFilesDeleted++;
				}
				catch (Exception)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(COULD_NOT_DELETE);
					Console.ResetColor();
					NumItemsNotDeleted++;
				}
			}
			Console.ResetColor();
			foreach (DirectoryInfo diSubFolder in dirInfo.GetDirectories())
			{
				// Next delete each sub-folder in the folder.
				if (ExcludedFolders.All(x => x != Path.GetFileName(diSubFolder.FullName)))
				{
					ClearFolder(diSubFolder); // Call recursively for all subfolders
				}
				try
				{
					Console.ResetColor();
					Console.Write("Deleting Fldr: {0}", TruncatePath(diSubFolder.FullName, 43).PadRight(43));

					if (ExcludedFolders.All(x => x != Path.GetFileName(diSubFolder.FullName)))
					{
						diSubFolder.Delete(false);
						//FileSystem.DeleteDirectory(diSubFolder.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine(DELETED);
						NumFoldersDeleted++;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						Console.WriteLine(SKIPPED);
						NumFoldersSkipped++;
					}
				}
				catch 
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(COULD_NOT_DELETE);
					Console.ResetColor();
					NumItemsNotDeleted++;
				}
			}
		}

		private const string SKIPPED = "            [Skipped]";
		private const string DELETED = "            [Deleted]";
		private const string COULD_NOT_DELETE = "   [Could not delete]";

		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool PathCompactPathEx(StringBuilder pszOut, string pszSrc, int cchMax, int dwFlags);

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
