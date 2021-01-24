on error resume next
Dim objShell
Set objShell = WScript.CreateObject("WScript.Shell")
objShell.Run objShell.CurrentDirectory & "\Enable Url Access.bat"
objShell.Run objShell.CurrentDirectory & "\ngen.bat"
