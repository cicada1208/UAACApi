@ECHO ON

IF EXIST "e:\build_env2019.bat" (
call e:\build_env2019.bat
)

IF EXIST "%Build_BinariesDirectory%\WpfiEMR" (
    ECHO "Clean up Build_BinariesDirectory"
    DEL /Q/F/S %Build_BinariesDirectory%\*
)

echo "WE ARE GOING TO SOURCE DIRECTORY %BUILD_SOURCESDIRECTORY%"
cd %BUILD_SOURCESDIRECTORY%

dotnet build
dotnet publish -o %Build_BinariesDirectory%

echo "Build completed. Message from build script"
