
make it so they are downloaded as flac:
@echo off
setlocal enabledelayedexpansion
:: Base directory is the current folder
set "BASE_DIR=%~dp0"
echo Spotify Playlist Sync Tool
echo ------------------------
echo.
echo 1. First-time sync (Create new sync file)
echo 2. Update existing sync file
echo 3. Exit
echo.
set /p choice="Enter your choice (1-3): "
if "%choice%"=="1" (
    echo.
    set /p "playlist_url=Enter Spotify playlist URL: "
    set /p "playlist_name=Enter playlist name (This will be the folder name): "
    :: Remove quotes if present in the URL
    set playlist_url=!playlist_url:"=!
    :: Create playlist directory if it doesn't exist
    if not exist "%BASE_DIR%!playlist_name!" mkdir "%BASE_DIR%!playlist_name!"
    echo.
    echo Creating new sync file and downloading playlist...
    call spotdl sync "!playlist_url!" --save-file "%BASE_DIR%!playlist_name!\!playlist_name!.spotdl" --output "%BASE_DIR%!playlist_name!"
    echo.
    echo Sync file created: !playlist_name!.spotdl
    echo Music downloaded to: %BASE_DIR%!playlist_name!
) else if "%choice%"=="2" (
    echo.
    echo Available playlists:
    echo -------------------
    for /d %%D in ("%BASE_DIR%*") do (
        if exist "%%D\*.spotdl" (
            echo %%~nxD
        )
    )
    echo.
    set /p "playlist_name=Enter playlist name: "
    if exist "%BASE_DIR%!playlist_name!\!playlist_name!.spotdl" (
        echo.
        echo Updating playlist...
        call spotdl sync "%BASE_DIR%!playlist_name!\!playlist_name!.spotdl" --output "%BASE_DIR%!playlist_name!"
    ) else (
        echo.
        echo Error: Sync file not found for playlist !playlist_name!
    )
) else if "%choice%"=="3" (
    echo Exiting...
    goto :end
) else (
    echo Invalid choice!
)
:end
echo.
echo Press any key to exit...
pause > nul