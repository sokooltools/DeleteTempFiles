using System.Runtime.InteropServices;
using System.Text;

namespace DeleteTempFiles
{
	internal static class NativeMethods
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool PathCompactPathEx(StringBuilder pszOut, string pszSrc, int cchMax, int dwFlags);
	}
}
