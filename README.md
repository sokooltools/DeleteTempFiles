# DeleteTempFiles

This mini-applet is used for deleting all files and folders inside the user's `%TEMP%` folder except for those specified as excluded.

### Options

Create a shortcut to this application (or run it directly from the command-line) if you want to specify that certain files or folders should be excluded, i.e., **NOT** deleted. This is accomplished by passing in a **comma-delimited** list of files and/or folders as an argument.

For example:

    "%ProgramFiles(x86)%\DevTools\DeleteTempFiles\DeleteTempFiles.exe" "JetBrains,ReSharperCache,SymbolCache,.com_ibm_tools_attach,notes256C9A"
