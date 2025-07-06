dotnet build -c Release
copy %~dp0bin\Release\netstandard2.1\UltrawideHUD.dll %appdata%\r2modmanPlus-local\PEAK\profiles\Modding\BepInEx\scripts /y
copy %~dp0bin\Release\netstandard2.1\UltrawideHUD.pdb %appdata%\r2modmanPlus-local\PEAK\profiles\Modding\BepInEx\scripts /y
copy %~dp0bin\Release\netstandard2.1\UltrawideHUD.deps.json %appdata%\r2modmanPlus-local\PEAK\profiles\Modding\BepInEx\scripts /y