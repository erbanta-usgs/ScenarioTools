@echo off
REM Set up demo directory
echo Start of make_demo...
if "%1"=="" goto usage
set dirnam=%1

set modeldir=D:\Projects\Florida-Tools\ScenarioTools\test\ExampleProblem\SWRSample08

if not exist %modeldir% goto end

set datadir=D:\Projects\Florida-Tools\ScenarioTools\test\Scenario_manager_well_data
set imagedir=D:\Projects\Florida-Tools\ScenarioTools\test\ImageData\LandCover_Biscayne

if exist %dirnam% goto usage

mkdir %dirnam%

cd %dirnam%

  REM Copy executables to bin directory
  echo.
  echo Copying executable files...
  call ..\copy_executables
    
  REM Copy documentation
  REM mkdir doc
  REM echo.
  REM echo Copying documentation...
  
  REM copy ..\doc\ScenarioAnalyzerExercises_120524-d.doc doc\ScenarioAnalyzerExercises_120524.doc
  REM copy ..\doc\ScenarioManagerExercises_120524-d.doc  doc\ScenarioManagerExercises_120524.doc
    
  REM Copy shapefiles  
  mkdir GIS
  echo.
  echo Copying shapefiles...
    
  copy %datadir%\bbgrid_poly.* GIS
  copy %datadir%\county_wells_NAD_27_filtered_edit.* GIS
  copy %datadir%\water_poly.* GIS
    
  REM Time-series data files
  mkdir TimeSeriesData
  echo.
  echo Copying time-series data...
    
  copy %datadir%\pumpwell2.smp TimeSeriesData
  copy %datadir%\sea_level.smp TimeSeriesData
  
  REM Copy image file
  mkdir Images
  echo.
  echo Copying background image...
  
  copy %imagedir%\Biscayne_area_land_cover.tif Images
  
  REM Set up example model folders

  REM Set up SWRSample08 example
  
  echo Setting up SWRSample08 example...
  mkdir SWRSample08
  cd SWRSample08
    mkdir GroupFiles
    mkdir init
    mkdir NonPumpingResults
    mkdir PumpingResults
    mkdir Results

    REM Copy model files
    
    copy %modeldir%\Sample01-obs.smp .
    copy %modeldir%\PW-01_Heads.smp .
    copy %modeldir%\PW-01_heads.USGS.csv .
    copy %modeldir%\SWRSample08.bas .
    copy %modeldir%\SWRSample08.dis .
    copy %modeldir%\SWRSample08.drn .
    copy %modeldir%\SWRSample08.evt .
    copy %modeldir%\SWRSample08.lpf .
    copy %modeldir%\SWRSample08.nam .
    copy %modeldir%\SWRSample08_nopump.nam .
    copy %modeldir%\SWRSample08.oc .
    copy %modeldir%\SWRSample08.pcg .
    copy %modeldir%\SWRSample08.rch .
    copy %modeldir%\SWRSample08.riv .
    copy %modeldir%\SWRSample08.swr .
    copy %modeldir%\SWRSample08.uzf .
    copy %modeldir%\SWRSample08.zone .
    copy %modeldir%\SWRSample08_SS_deep.wel .
    
    copy %modeldir%\run_pump.bat run_pump.bat
    copy %modeldir%\run_nopump.bat run_nopump.bat

    copy %modeldir%\GroupFiles\SWRSample08_Basins.csv GroupFiles    
    copy %modeldir%\init\*.ref init
    
    REM Run MODFLOW-2005
    
    echo.
    echo Running MODFLOW to generate output files for pumping case...
    ..\bin\mf2005-SWR.1.8.31.exe SWRSample08.nam
    echo.
    echo Running MODFLOW to generate output files for no-pumping case...
    echo.
    ..\bin\mf2005-SWR.1.8.31.exe SWRSample08_nopump.nam
    echo.

    cd ..
    
  REM Set up 500m_example example
  
  echo Setting up 500m_example...
  echo.
  mkdir 500m_example  
  set exdir=D:\Projects\Florida-Tools\ScenarioTools\test\ExampleProblem\3_layer_500m_example
  cd 500m_example
    copy %exdir%\3_layer_example_500m.bas .
    copy %exdir%\3_layer_example_500m.chd .
    copy %exdir%\3_layer_example_500m.dis .
    copy %exdir%\3_layer_example_500m.evt .
    copy %exdir%\3_layer_example_500m.lpf .
    copy %exdir%\3_layer_example_500m.nam .
    copy %exdir%\3_layer_example_500m.oc .
    copy %exdir%\3_layer_example_500m.pcg .
    copy %exdir%\3_layer_example_500m.rch .
    copy %exdir%\3_layer_example_500m.riv .
    copy %exdir%\3_layer_example_500m.wel .
    copy %exdir%\3_layer_HK.ref .
    copy %exdir%\3_layer_example_500m_arrays.rch .
    
    cd ..
    
  cd ..

echo.
echo Finished making demo folder "%dirnam%"
echo.

goto end

:usage
echo.
echo Usage: make_demo Directory-name 
echo Directory-name must not exist
echo.

REM
:end
