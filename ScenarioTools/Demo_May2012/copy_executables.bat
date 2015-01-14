@echo off
REM Copy Scenario Manager and Scenario Analyzer executables to .\bin folder
if not exist D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioManager\bin\Debug goto nocopy

if exist bin goto binExists

mkdir bin
goto binIsNew

:binExists

echo Deleting old executable files, if any exist...
rm bin\*.exe
rm bin\*.dll

:binIsNew

echo Copying executable files...

copy D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioManager\bin\Debug\ScenarioManager.exe bin
copy D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioManager\bin\Debug\*.dll bin

copy D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioAnalyzer\bin\Debug\ScenarioAnalyzer.exe bin
copy D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioAnalyzer\bin\Debug\Gui.dll bin
copy D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioAnalyzer\bin\Debug\libhpdf.dll bin

copy "C:\Program Files (x86)\Microsoft Chart Controls\Assemblies\System.Windows.Forms.DataVisualization.dll" bin

copy C:\WRDAPP\MF2005.1_9\bin\mf2005.exe bin
copy C:\WRDAPP\MF2005.1_9\bin\mf2005dbl.exe bin
copy D:\Projects\Florida-Tools\ScenarioTools\test\ExampleProblem\SWRSample08\mf2005-SWR.1.8.31.exe bin

goto end

:nocopy
echo No files copied

:end
