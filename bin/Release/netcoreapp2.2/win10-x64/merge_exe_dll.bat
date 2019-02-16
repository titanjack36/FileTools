@echo off
ILMerge.exe /target:winexe /targetplatform:v2,C:/Windows/Microsoft.NET/Framework64/v2.0.50727 /out:"%cd%\finish.exe" "%cd%\FileTools.exe" "%cd%\hostfxr.dll" "%cd%\hostpolicy.dll" "%cd%\FileTools.dll" "%cd%\FileTools.deps.json" "%cd%\FileTools.runtimeconfig.dev.json" "%cd%\FileTools.runtimeconfig.json"
pause