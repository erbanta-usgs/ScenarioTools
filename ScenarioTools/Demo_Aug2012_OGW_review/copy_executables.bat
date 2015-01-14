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
REM copy D:\Projects\Florida-Tools\ScenarioTools\src\ScenarioAnalyzer\bin\Debug\libhpdf.dll bin

REM Code to copy required DLL based on OS version under which this batch file is run
ver | findstr /i "5\.1\." > nul
IF %ERRORLEVEL% EQU 0 goto ver_XP
ver | findstr /i "6\.1\." > nul
IF %ERRORLEVEL% EQU 0 goto ver_Win7

:ver_XP
REM The following DLL is needed when Scenario Analyzer is run under Windows XP
echo Copying DLL needed for MS Chart Controls (Windows XP)
copy "C:\Program Files\Microsoft Chart Controls\Assemblies\System.Windows.Forms.DataVisualization.dll" bin
goto endVer

:ver_Win7
REM The following DLL is needed when Scenario Analyzer is run under Windows 7
echo Copying DLL needed for MS Chart Controls (Windows 7)
copy "C:\Program Files (x86)\Microsoft Chart Controls\Assemblies\System.Windows.Forms.DataVisualization.dll" bin

:endVer

copy C:\WRDAPP\MF2005.1_9\bin\mf2005.exe bin
copy C:\WRDAPP\MF2005.1_9\bin\mf2005dbl.exe bin
copy D:\Projects\Florida-Tools\ScenarioTools\test\ExampleProblem\SWRSample08\mf2005-SWR.1.8.31.exe bin
copy D:\Projects\Florida-Tools\ScenarioTools\test\demo_120730b\bin\MF2005_VS2008.exe bin

goto end

:nocopy
echo No files copied

:end
