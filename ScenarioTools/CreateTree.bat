@echo off
echo Start of CreateTree...
if "%1"=="" goto usage
set dirnam=%1
set createall=NO
if "%2"=="all" set createall=ALL
if "%2"=="ALL" set createall=ALL
if "%2"=="All" set createall=ALL

if exist %dirnam% goto usage

mkdir %dirnam%

cd %dirnam%

REM  Set up distribution directories

copy ..\readme.txt ReadMe.txt

REM ************************************
REM Bin directory for executables
REM ************************************
mkdir Bin

echo.
echo Copying DLLS and executable file for ScenarioAnalyzer...
copy ..\src\DLLs\FWTools\*.dll Bin
copy ..\src\DLLs\LibHaru\libhpdf.dll Bin
copy ..\src\DLLs\MapTools\MapTools.dll Bin

if not exist ..\src\ScenarioAnalyzer\bin\x86\Release\ScenarioAnalyzer.exe goto noX86
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\ScenarioAnalyzer.exe Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdalconst_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\ogr_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\osr_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\GeoAPI.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\HPdf.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\ICSharpCode.SharpZipLib.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\NDepend.Helpers.FileDirectoryPath.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\ScenarioTools.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\SoftwareProductions.Utilities.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\USGS.ModflowTrainingTools.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\USGS.Puma.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\USGS.Puma.Modflow.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\USGS.Puma.Modpath.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\USGS.Puma.NTS.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\USGS.Puma.UI.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\freexl.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\gdal_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\gdal19.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\gdalconst_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\geos_c.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\iconv.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\libcurl.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\libeay32.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\libexpat.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\libmysql.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\libpq.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\msvcp100.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\msvcr100.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\ogr_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\openjpeg.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\osr_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\proj.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\spatialite.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\ssleay32.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\xerces-c_2_8.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\x86\Release\gdal\x86\zlib1.dll Bin
  goto endReleaseCopy
:noX86
  copy ..\src\ScenarioAnalyzer\bin\Release\ScenarioAnalyzer.exe Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdalconst_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\ogr_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\osr_csharp.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\GeoAPI.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\HPdf.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\ICSharpCode.SharpZipLib.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\NDepend.Helpers.FileDirectoryPath.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\ScenarioTools.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\SoftwareProductions.Utilities.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\USGS.ModflowTrainingTools.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\USGS.Puma.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\USGS.Puma.Modflow.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\USGS.Puma.Modpath.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\USGS.Puma.NTS.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\USGS.Puma.UI.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\freexl.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\gdal_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\gdal19.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\gdalconst_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\geos_c.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\iconv.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\libcurl.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\libeay32.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\libexpat.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\libmysql.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\libpq.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\msvcp100.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\msvcr100.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\ogr_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\openjpeg.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\osr_wrap.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\proj.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\spatialite.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\ssleay32.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\xerces-c_2_8.dll Bin
  copy ..\src\ScenarioAnalyzer\bin\Release\gdal\x86\zlib1.dll Bin
:endReleaseCopy

echo.
echo Copying executable file for ScenarioManager...
copy ..\src\ScenarioManager\bin\Release\ScenarioManager.exe Bin

echo.
echo Copying MODFLOW and SEAWAT executables...
mkdir Test
mkdir Test\bin
copy C:\WRDAPP\MF2005.1_11\bin\mf2005.exe Test\bin
copy C:\WRDAPP\MF2005.1_11\bin\mf2005dbl.exe Test\bin
copy D:\Downloads\Seawat\swt_v4_00_05\exe\swt_v4.exe Test\bin

REM ************************************
REM Doc directory for documentation
REM ************************************

REM ************************************
REM Src directory for source code
REM ************************************

if %createall%==NO goto endcopysrc
mkdir Src
cd Src

  mkdir ScenarioManager
  echo.
  echo Copying source-code files for ScenarioManager...

  mkdir ScenarioAnalyzer
  echo.
  echo Copying source-code files for ScenarioAnalyzer...
  
  cd ..

:endcopysrc

REM ************************************
REM Test directory and subdirectories
REM ************************************

echo.
echo Copying Test directory...
cd Test

REM ************************************

  echo   Copying 500m_example...
  
  mkdir 500m_example
  copy ..\..\test\demo_120813\500m_example\3_layer_example_500m.* 500m_example
  copy ..\..\test\demo_120813\500m_example\3_layer_example_500m_arrays.rch 500m_example
  copy ..\..\test\demo_120813\500m_example\3_layer_HK.ref 500m_example

REM ************************************

  if not exist ..\..\test\ModelMuseTest\MMuseExample.cbc goto makeMMuseRun
  if not exist ..\..\test\ModelMuseTest\MMuseExample.hdd goto makeMMuseRun
  
  echo.
  echo MMuseExample output files are present -- MMuseExample runs will not be made
  echo.
  
  goto skipMMuseRun
  
  :makeMMuseRun
  
  echo Running ModelMuseTest...
  cd ..\..\test\ModelMuseTest
    call go.bat
    cd ..\..
    cd %dirnam%
    cd Test

  :skipMMuseRun
  
  echo   Copying ModelMuseTest...
  
  mkdir ModelMuseTest
  copy ..\..\test\ModelMuseTest\ModelMuseTest.bat ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.bas  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.cbc  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.dis  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.hdd  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.lpf  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.lst  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.nam  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.oc   ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.pcg  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.pval ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.rch  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.riv  ModelMuseTest
  copy ..\..\test\ModelMuseTest\MMuseExample.wel  ModelMuseTest
  copy ..\..\test\ModelMuseTest\ModelMuseGrid.dbf ModelMuseTest
  copy ..\..\test\ModelMuseTest\ModelMuseGrid.shp ModelMuseTest
  copy ..\..\test\ModelMuseTest\ModelMuseGrid.shx ModelMuseTest
  
REM ************************************

  echo   Copying GIS files...
  
  mkdir GIS
  copy ..\..\test\demo_120813\GIS\bbgrid_poly.* GIS
  copy ..\..\test\demo_120813\GIS\county_wells_NAD_27_filtered_edit.* GIS
  copy ..\..\test\Scenario_manager_well_data\water_poly.* GIS  
  copy ..\..\test\Scenario_manager_well_data\new_rch_zones.* GIS
  copy ..\..\test\ExampleProblem\SWRSample08\SwrSample08_new.shp GIS\SwrSample08_grid.shp
  copy ..\..\test\ExampleProblem\SWRSample08\SwrSample08_new.dbf GIS\SwrSample08_grid.dbf
  copy ..\..\test\ExampleProblem\SWRSample08\SwrSample08_new.shx GIS\SwrSample08_grid.shx
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.shp GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.dbf GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.fix GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.prj GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.qix GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.sbn GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.sbx GIS
  copy ..\..\test\ExampleProblem\GIS\MD_Canals_50m_v2.shx GIS

REM ************************************

  echo   Copying Images...
  
  mkdir Images
  copy ..\..\test\demo_120813\Images\Biscayne_area_land_cover.tif Images  

REM ************************************

  echo   Copying SWRSample08...

  if not exist ..\..\test\demo_120813\SWRSample08\NonPumpingResults\SWRSample08.cbb goto makeSwrRun
  if not exist ..\..\test\demo_120813\SWRSample08\NonPumpingResults\SWRSample08.hds goto makeSwrRun
  if not exist ..\..\test\demo_120813\SWRSample08\PumpingResults\SWRSample08.cbb goto makeSwrRun
  if not exist ..\..\test\demo_120813\SWRSample08\PumpingResults\SWRSample08.hds goto makeSwrRun
  
  echo.
  echo SWRSample08 output files are present -- SWRSample08 runs will not be made
  echo.
  
  goto skipSwrRun
  
  :makeSwrRun
  
  cd ..\..\test\demo_120813\SWRSample08
    copy ..\..\ExampleProblem\SWRSample08\mf2005-SWR.1.8.31.exe ..\Bin
    echo.
    echo Running SWRSample08 (with pumping)...
    call run_pump.bat
    echo.
    echo Running SWRSample08 (without pumping)...
    echo.
    call run_nopump.bat
    cd ..\..\..
    cd %dirnam%
    cd Test

  :skipSwrRun
  
  mkdir SWRSample08
  copy ..\..\test\demo_120813\SWRSample08\*.bat SWRSample08
  copy ..\..\test\demo_120813\SWRSample08\Well01-obs.smp SWRSample08
  copy ..\..\test\demo_120813\SWRSample08\SWRSample08.* SWRSample08
  copy ..\..\test\demo_120813\SWRSample08\SWRSample08_nopump.nam SWRSample08
  copy ..\..\test\demo_120813\SWRSample08\SWRSample08_SS_deep.wel SWRSample08

  cd SWRSample08
  
    mkdir GroupFiles
    copy ..\..\..\test\demo_120813\SWRSample08\GroupFiles\SWRSample08_Basins.csv GroupFiles
  
    mkdir init
    copy ..\..\..\test\demo_120813\SWRSample08\init\*.ref init
  
    mkdir NonPumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\NonPumpingResults\SWRSample08.* NonPumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\NonPumpingResults\SWRSample08flows.csv NonPumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\NonPumpingResults\SWRSample08stage.csv NonPumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\NonPumpingResults\UZFbudgetTR.txt NonPumpingResults
  
    mkdir PumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\PumpingResults\SWRSample08.* PumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\PumpingResults\SWRSample08flows.csv PumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\PumpingResults\SWRSample08stage.csv PumpingResults
    copy ..\..\..\test\demo_120813\SWRSample08\PumpingResults\UZFbudgetTR.txt PumpingResults

    cd ..
    
REM ************************************

  echo   Copying SeawatExample...
  
  mkdir SeawatExample
  
REM Copy input files  
  copy ..\..\test\SeawatExample\SeawatExample.adv        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.bas        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.bat        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.bcf        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.btn        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.chd        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.evt        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.dis        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.gcg        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.ghb        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.nam        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.oc         SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.pcg        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.rch        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.riv        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.ssm        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.vdf        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.wel        SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample.zone       SeawatExample
  copy ..\..\test\SeawatExample\SeawatExample_arrays.rch SeawatExample
  
REM Copy output files
  copy ..\..\test\SeawatExample_work\MT3D001.MAS         SeawatExample
  copy ..\..\test\SeawatExample_work\MT3D001.UCN         SeawatExample
  copy ..\..\test\SeawatExample_work\MT3D.CNF            SeawatExample
  copy ..\..\test\SeawatExample_work\SeawatExample.cbb   SeawatExample
  copy ..\..\test\SeawatExample_work\SeawatExample.cbwel SeawatExample
  copy ..\..\test\SeawatExample_work\SeawatExample.hds   SeawatExample
  copy ..\..\test\SeawatExample_work\SeawatExample.lst   SeawatExample
    
REM ************************************

  echo   Copying TimeSeriesData...
  
  mkdir TimeSeriesData
  copy ..\..\test\demo_120813\TimeSeriesData\pumpwell2.smp TimeSeriesData
  copy ..\..\test\demo_120813\TimeSeriesData\sea_level.smp TimeSeriesData
  copy ..\..\test\ExampleProblem\SMP\output_navd_all.smp TimeSeriesData
  copy ..\..\test\Scenario_manager_well_data\recharge.smp TimeSeriesData
  
  cd ..
cd ..

echo.
echo Done making folder: %dirnam%
goto end

REM ************************************
REM Usage
REM ************************************

:usage
echo.
echo Usage: CreateTree Directory-name
echo where Directory-name is of form: ScenarioTools.0_1_0
echo Directory-name must not exist
echo.
goto end
REM

:end
