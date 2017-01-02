@echo off
cls
set arg1=%1

cd .nuget
nuget.exe "install" "FAKE" "-OutputDirectory" "..\tools" "-ExcludeVersion"
cd ..

"tools\FAKE\tools\Fake.exe" "build.fsx" "version="%arg1%
