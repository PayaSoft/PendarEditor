on error resume next
Dim objShell
Set objShell = WScript.CreateObject("WScript.Shell")
objShell.Exec "%APPDATA%\Paya\Automation\Editor\Paya.Automation.Editor.exe"

