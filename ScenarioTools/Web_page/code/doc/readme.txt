readme.txt

                      Scenario Tools - Version 1.0.0
                      
            Graphical user interfaces for managing and analyzing
                    MODFLOW groundwater-model scenarios
      
NOTE: Any use of trade, product or firm names is for descriptive purposes 
      only and does not imply endorsement by the U.S. Government.

Scenario Tools (Banta, 2014) is a suite of two graphical user interface 
(GUI) applications for managing and analyzing groundwater-model scenarios: 
Scenario Manager and Scenario Analyzer.

Executable files, example files, and a help facility are included.  Please 
see file release.txt for release notes.


                            TABLE OF CONTENTS
                            
            A. Distribution file
            B. Installing
            C. Preparing examples
            D. Source code and executables
            E. References


A. Distribution file

The Scenario Tools applications are distributed for use under the Microsoft 
Windows operating system in an installation setup file named:

    ScenarioTools_1_0_0_Setup.exe
     

B. Installing

Installation of Scenario Tools requires administrator privileges.  When 
executed, the setup file installs the applications, creates Start menu 
shortcuts, and, optionally, makes file associations for the extensions 
identifying Scenario Manager (.smgx) and Scenario Analyzer (.sa) files, and 
creates desktop icons.  These shortcuts and icons can be used to start the 
corresponding application.

The Scenario Tools applications require that Microsoft .NET Framework 4.0 
(Client Profile) be installed on the user's computer.  If .NET Framework 4.0 
has not been installed, the setup file will install it.

The following folder structure will be created in the installation folder:   

Scenario Tools
    bin                 (Executable files for Scenario Tools)
    examples            (Examples shown in Help facility)
        500m_example    (Input files needed for Scenario Manager example)
        bin             (Executable file used by Scenario Manager example)
        GIS             (Shapefiles needed for examples)
        Images          (Georeference image file needed for examples)
        SeawatExample   (Input files needed for Scenario Analyzer example)
        TimeSeriesData  (Time-series data files needed for examples)
    uninstall           (Files needed for uninstalling Scenario Tools)

It is recommended that no user files are kept in the Scenario Tools 
installation folder structure.


C. Preparing examples

Before using the examples, the examples folder needs to be copied to a 
location where the user has write privileges.  After installation is 
complete, please copy the entire examples folder to a location on your 
computer where the user has write privileges.

Some model-output files need to be generated before Scenario Analyzer can be 
used with the SeawatExample model.  To generate those output files, run the 
batch file PrepareExamples.bat in the copy of the examples folder, where the 
user should have write privileges.  PrepareExamples.bat may take several 
minutes to complete the model run.


D. Source code and executables

The Scenario Tools applications are compiled from source code written in the 
C# programming language.  It was compiled using Microsoft Visual Studio 
2010.  

The applications also use Dynamic Link Library (DLL) files from a number of 
sources.  Without the availability of these libraries, timely development of 
the applications described in this report would not have been possible:

o Puma Framework (http://water.usgs.gov/ogw/modpath/moe.html)
o FW-Tools (http://fwtools.maptools.org/)
o Geospatial Data Abstraction Library (http://www.gdal.org/)
o GeoAPI.NET (http://geoapi.codeplex.com/)
o Haru free PDF library (http://libharu.org/)
o NetTopologySuite (http://code.google.com/p/nettopologysuite/)
o SharpMap (http://sharpmap.codeplex.com/)
o SharpZip (http://www.icsharpcode.net/opensource/sharpziplib/)
o Software Productions (for Undo and Redo; no longer available online)

Source code may be obtained from the author:

    Edward (Ned) Banta
    erbanta@usgs.gov

However, the USGS cannot provide assistance for versions of the Scenario 
Tools applications other than the one provided for download on the USGS 
water-software web page (http://water.usgs.gov/software/).


E. References

Banta, E.R., 2014, Two graphical user interfaces for managing and analyzing 
MODFLOW groundwater-model scenarios: U.S. Geological Survey Techniques and 
Methods, book 6, chap. A50, 38 p. Available online at: http://dx.doi.org/10.3133/tm6a50.

Harbaugh, A.W., 2005, MODFLOW-2005, the U.S. Geological Survey modular 
ground-water model--The Ground-Water Flow Process: U.S. Geological Survey 
Techniques and Methods, book 6, chap. A16, variously paginated.  (Also 
available at http://pubs.usgs.gov/tm/2005/tm6A16/PDF/TM6A16.pdf.)
