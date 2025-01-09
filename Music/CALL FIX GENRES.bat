@echo off
cd /d "%~dp0"
call .\spotdl-env\Scripts\activate.bat
setlocal enabledelayedexpansion

echo Available folders to fix:
echo -----------------------
set i=1
for /d %%D in (".\*") do (
    echo !i!. %%~nxD
    set "folder[!i!]=%%~nxD"
    set "path[!i!]=%%~fD"
    set /a i+=1
)
echo.
set /p choice="Enter the number of the folder to fix (or 0 to exit): "
if "%choice%"=="0" goto :end

.\spotdl-env\Scripts\python.exe fix_genres.py "!path[%choice%]!"
echo.
:end
echo Press any key to exit...
pause > nul