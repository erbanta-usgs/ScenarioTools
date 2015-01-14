@echo off

REM define FWTools environment
call "C:\Program Files\FWTools2.4.7\setfw.bat"

if exist output.tif del Biscayne_area_land_cover.tif

REM Generate GeoTiff with original (30 meter) resolution
REM gdalwarp  -t_srs ESRI::bbgrid_poly.prj  75243822.tif Biscayne_area_land_cover.tif

REM Generate GeoTiff with 100 meter resolution
gdalwarp  -t_srs ESRI::bbgrid_poly.prj  -tr 100.0 100.0 75243822.tif Biscayne_area_land_cover.tif


