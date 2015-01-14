using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;

using OSGeo.GDAL; // supports raster formats
using OSGeo.OGR;  // Supports vector formats.  May not be needed?
using OSGeo.OSR;  // Spatial reference and coordinate transformation.

namespace ScenarioTools
{
    public static class GdalHelper
    {
        public static Bitmap ToBitmap(OSGeo.GDAL.Dataset dataset, int iOverview)
        {
            if (GlobalStaticVariables.GlobalBackgroundWorker != null)
            {
                GlobalStaticVariables.GlobalBackgroundWorker.RunWorkerAsync(dataset);
                while (GlobalStaticVariables.GlobalBackgroundWorker.IsBusy)
                {
                    Thread.Sleep(1);
                    GlobalStaticVariables.DoEvents();
                }
                while (GlobalStaticVariables.GlobalBitmap == null)
                {
                    Thread.Sleep(1);
                    GlobalStaticVariables.DoEvents();
                }
                return GlobalStaticVariables.GlobalBitmap;
            }
            else
            {
                BackgroundWorker nullBackgroundWorker = null;
                return GetBitmapBuffered(dataset, iOverview, nullBackgroundWorker);
            }
        }

        public static Bitmap GetBitmapBuffered(Dataset ds, int iOverview, BackgroundWorker worker) 
        {
            Bitmap bitmap = null;
            // Get the GDAL Band objects from the Dataset
            Band redBand = ds.GetRasterBand(1);

            if (redBand.GetRasterColorInterpretation() == ColorInterp.GCI_PaletteIndex)
            {
                bitmap = GetBitmapPaletteBuffered(ds, iOverview, worker);
                GlobalStaticVariables.GlobalBitmap = bitmap;
                return bitmap;
            }

            if (redBand.GetRasterColorInterpretation() == ColorInterp.GCI_GrayIndex)
            {
                bitmap = GetBitmapGrayBuffered(ds, iOverview, worker);
                GlobalStaticVariables.GlobalBitmap = bitmap;
                return bitmap;
            }

            if (redBand.GetRasterColorInterpretation() != ColorInterp.GCI_RedBand)
            {
                Console.WriteLine("Non RGB images are not supported by this sample! ColorInterp = " +
                    redBand.GetRasterColorInterpretation().ToString());
                return null;
            }

            if (ds.RasterCount < 3)
            {
                Console.WriteLine("The number of the raster bands is not enough to run this sample");
                return null;
            }

            if (iOverview >= 0 && redBand.GetOverviewCount() > iOverview)
                redBand = redBand.GetOverview(iOverview);

            Band greenBand = ds.GetRasterBand(2);

            if (greenBand.GetRasterColorInterpretation() != ColorInterp.GCI_GreenBand)
            {
                Console.WriteLine("Non RGB images are not supported by this sample! ColorInterp = " +
                    greenBand.GetRasterColorInterpretation().ToString());
                return null;
            }

            if (iOverview >= 0 && greenBand.GetOverviewCount() > iOverview)
                greenBand = greenBand.GetOverview(iOverview);

            Band blueBand = ds.GetRasterBand(3);

            if (blueBand.GetRasterColorInterpretation() != ColorInterp.GCI_BlueBand)
            {
                Console.WriteLine("Non RGB images are not supported by this sample! ColorInterp = " +
                    blueBand.GetRasterColorInterpretation().ToString());
                return null;
            }

            if (worker != null)
            {
                worker.ReportProgress(80);
            }

            if (iOverview >= 0 && blueBand.GetOverviewCount() > iOverview)
                blueBand = blueBand.GetOverview(iOverview);

            // Get the width and height of the raster
            int width = redBand.XSize;
            int height = redBand.YSize;

            // Create a Bitmap to store the GDAL image in
            bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            DateTime start = DateTime.Now;

            byte[] r = new byte[width * height];
            byte[] g = new byte[width * height];
            byte[] b = new byte[width * height];

            redBand.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            greenBand.ReadRaster(0, 0, width, height, g, width, height, 0, 0);
            blueBand.ReadRaster(0, 0, width, height, b, width, height, 0, 0);
            TimeSpan renderTime = DateTime.Now - start;
            Console.WriteLine("SaveBitmapBuffered fetch time: " + renderTime.TotalMilliseconds + " ms");

            int i, j;
            int progressIncrement = width / 100;
            if (progressIncrement < 1)
            {
                progressIncrement = 1;
            }
            int progress = 0;
            int progressCounter = 0;
            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    Color newColor = Color.FromArgb(Convert.ToInt32(r[i + j * width]), 
                                                    Convert.ToInt32(g[i + j * width]), 
                                                    Convert.ToInt32(b[i + j * width]));
                    bitmap.SetPixel(i, j, newColor);
                }
                progressCounter++;
                if (worker != null)
                {
                    if (progressCounter == progressIncrement && progress < 100)
                    {
                        progress++;
                        worker.ReportProgress(progress);
                        progressCounter = 0;
                    }
                }
            }
            if (worker != null)
            {
                worker.ReportProgress(100);
            }
            GlobalStaticVariables.GlobalBitmap = bitmap;
            return bitmap;
        }

        private static Bitmap GetBitmapPaletteBuffered(Dataset ds, int iOverview, BackgroundWorker worker)
        {
            // Get the GDAL Band objects from the Dataset
            Band band = ds.GetRasterBand(1);
            if (iOverview >= 0 && band.GetOverviewCount() > iOverview)
                band = band.GetOverview(iOverview);

            ColorTable ct = band.GetRasterColorTable();
            if (ct == null)
            {
                Console.WriteLine("   Band has no color table!");
                return null;
            }

            if (ct.GetPaletteInterpretation() != PaletteInterp.GPI_RGB)
            {
                Console.WriteLine("   Only RGB palette interp is supported by this sample!");
                return null;
            }

            // Get the width and height of the Dataset
            int width = band.XSize;
            int height = band.YSize;

            // Create a Bitmap to store the GDAL image in
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            DateTime start = DateTime.Now;

            byte[] r = new byte[width * height];

            band.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            TimeSpan renderTime = DateTime.Now - start;
            Console.WriteLine("SaveBitmapBuffered fetch time: " + renderTime.TotalMilliseconds + " ms");

            int i, j;
            int progressIncrement = width / 100;
            if (progressIncrement < 1)
            {
                progressIncrement = 1;
            }
            int progress = 0;
            int progressCounter = 0;
            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    ColorEntry entry = ct.GetColorEntry(r[i + j * width]);
                    Color newColor = Color.FromArgb(Convert.ToInt32(entry.c1), Convert.ToInt32(entry.c2), Convert.ToInt32(entry.c3));
                    bitmap.SetPixel(i, j, newColor);
                }
                progressCounter++;
                if (worker != null)
                {
                    if (progressCounter == progressIncrement && progress < 100)
                    {
                        progress++;
                        worker.ReportProgress(progress);
                        progressCounter = 0;
                    }
                }
            }

            return bitmap;
        }

        private static Bitmap GetBitmapGrayBuffered(Dataset ds, int iOverview, BackgroundWorker worker)
        {
            // Get the GDAL Band objects from the Dataset
            Band band = ds.GetRasterBand(1);
            if (iOverview >= 0 && band.GetOverviewCount() > iOverview)
                band = band.GetOverview(iOverview);

            // Get the width and height of the Dataset
            int width = band.XSize;
            int height = band.YSize;

            // Create a Bitmap to store the GDAL image in
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            DateTime start = DateTime.Now;

            byte[] r = new byte[width * height];

            band.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            TimeSpan renderTime = DateTime.Now - start;
            Console.WriteLine("SaveBitmapBuffered fetch time: " + renderTime.TotalMilliseconds + " ms");

            int i, j;
            int progressIncrement = width / 100;
            if (progressIncrement < 1)
            {
                progressIncrement = 1;
            }
            int progress = 0;
            int progressCounter = 0;
            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    Color newColor = Color.FromArgb(Convert.ToInt32(r[i + j * width]), Convert.ToInt32(r[i + j * width]), Convert.ToInt32(r[i + j * width]));
                    bitmap.SetPixel(i, j, newColor);
                }
                progressCounter++;
                if (worker != null)
                {
                    if (progressCounter == progressIncrement && progress < 100)
                    {
                        progress++;
                        worker.ReportProgress(progress);
                        progressCounter = 0;
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// GDALInfoGetPosition
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="geoX">X-coordinate</param>
        /// <param name="geoY">Y-coordinate</param>
        /// Examples using original signature:
        /// Console.WriteLine( "Corner Coordinates:" );
        /// Console.WriteLine("  Upper Left (" + GDALInfoGetPosition( ds, 0.0, 0.0) + ")");
        /// Console.WriteLine("  Lower Left (" + GDALInfoGetPosition( ds, 0.0, ds.RasterYSize) + ")");
        /// Console.WriteLine("  Upper Right (" + GDALInfoGetPosition( ds, ds.RasterXSize, 0.0) + ")");
        /// Console.WriteLine("  Lower Right (" + GDALInfoGetPosition( ds, ds.RasterXSize, ds.RasterYSize) + ")");
        /// Console.WriteLine("  Center (" + GDALInfoGetPosition( ds, ds.RasterXSize / 2, ds.RasterYSize / 2) + ")");
        /// Original signature: private static string GDALInfoGetPosition(Dataset ds, double x, double y)
        public static void GDALInfoGetPosition(Dataset ds, double x, double y, ref double geoX, ref double geoY)
        {
        #region Copyright
        /******************************************************************************
        * Copyright (c) 2007, Tamas Szekeres
        *
        * Permission is hereby granted, free of charge, to any person obtaining a
        * copy of this software and associated documentation files (the "Software"),
        * to deal in the Software without restriction, including without limitation
        * the rights to use, copy, modify, merge, publish, distribute, sublicense,
        * and/or sell copies of the Software, and to permit persons to whom the
        * Software is furnished to do so, subject to the following conditions:
        *
        * The above copyright notice and this permission notice shall be included
        * in all copies or substantial portions of the Software.
        *
        * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
        * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
        * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
        * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
        * DEALINGS IN THE SOFTWARE.
        *****************************************************************************/
        #endregion Copyright
            double[] adfGeoTransform = new double[6];
            //double dfGeoX, dfGeoY;
            ds.GetGeoTransform(adfGeoTransform);

            geoX = adfGeoTransform[0] + adfGeoTransform[1] * x + adfGeoTransform[2] * y;
            geoY = adfGeoTransform[3] + adfGeoTransform[4] * x + adfGeoTransform[5] * y;

            //return dfGeoX.ToString() + ", " + dfGeoY.ToString();
            return;
        }

        public static void Warp(OSGeo.GDAL.Dataset dsSrc)
        {
            // This method is not functional
            OSGeo.GDAL.Gdal gdal = new Gdal();
            OSGeo.OGR.Ogr ogr = new Ogr();
            // Strings identifying Well Known Text format for describing coordinate systems 
            string wktOrig = dsSrc.GetProjection();
            string wktTarget = ""; // Ned TODO: Needs to be defined
            OSGeo.OSR.SpatialReference srSource = new SpatialReference(wktOrig);
            OSGeo.OSR.SpatialReference srDest = new SpatialReference(wktTarget);
            OSGeo.OSR.CoordinateTransformation ct = new CoordinateTransformation(srSource, srDest);
            OSGeo.OSR.Osr osr = new Osr();
        }

    }

}