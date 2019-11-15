# DeleteTempFiles

This mini-application is used for deleting all files and folders inside the user's `%TEMP%` folder except for those specified as being excluded.

### Options

If you want to specify that certain files or folders should be excluded, i.e., **NOT** deleted, create a shortcut to this application or run it directly from the command-line passing in a **comma-delimited** list of files and/or folders as an argument to the executable.

For example:

    "%ProgramFiles(x86)%\SokoolTools\DeleteTempFiles\DeleteTempFiles.exe" "JetBrains,ReSharperCache,SymbolCache,.com_ibm_tools_attach,notes256C9A"
