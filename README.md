# DeleteTempFiles

This MS Windows mini-application is used for deleting all files and folders inside the 
user's `%TEMP%` folder except for those specified as being excluded.

### Options

If you want to specify that certain files or folders should be excluded, i.e., **NOT** 
be deleted, create a shortcut to this application (or run it directly from the 
command-line), passing in a **comma-delimited** list of files and/or folders as an 
argument to the executable.

For example:

    "%ProgramFiles(x86)%\SokoolTools\DeleteTempFiles\DeleteTempFiles.exe" "JetBrains,ReSharperCache,SymbolCache,.com_ibm_tools_attach,notes256C9A"

<hr>

Running the application immediately begins deleting folders and/or files from both the user's `%TEMP%` folder (e.g. `'C:\Users\[Username]\AppData\Local\Temp'`)
and system `%TEMP%` folder (e.g. `'C:\Windows\Temp'`), meanwhile displaying the results in the application's console window 
such as this:

![Image1](Images/image1.png "Deleting Temp Files")

Notice at the bottom of the output window there is:
  1. A summary of how many folders got 'skipped' (because of an exclusion argument being specified);
  2. The number of folders and/or files that could not be deleted (most likely because 
     they were open at the time by some other process running on the PC); 
  3. The number of folders and/or files actually deleted from the %TEMP% directory.
