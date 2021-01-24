@echo off

if [%1]==[] (
set D=%~dp0
) ELSE (
SET D=%1
)

IF exist "%windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe" (
    set framework=%windir%\Microsoft.NET\Framework64\v4.0.30319\
	echo x64
) ELSE (
    set framework=%windir%\Microsoft.NET\Framework\v4.0.30319\
	echo x86
)

if not exist "%framework%\ngen.exe" goto :error

"%framework%\ngen.exe" install %D%\Paya.Automation.Editor.exe

for %%f in (%D%\*.dll) do "%framework%\ngen.exe" install "%%f"

goto :end
:error
echo The ngen not found
:end

VER

