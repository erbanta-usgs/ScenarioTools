﻿using System;
using System.Collections.Generic;
using System.Text;

using ScenarioTools.Geometry;
using ScenarioTools.Numerical;

namespace ScenarioTools.Graphics
{
    public class ContourMaker
    {
        private Extent extent;

        const int MIN_X_STEPS = 2;
        const int MIN_Y_STEPS = 2;
        const int MAX_X_STEPS = 100;
        const int MAX_Y_STEPS = 100;

        const string infoStrX = "string x";
        const string infoStrY = "string y";

        // Below, constant data members:
        const bool SHOW_NUMBERS = true;
        const int BLANK = 32, OPEN_SUITE = (int)'{', CLOSE_SUITE = (int)'}', BETWEEN_ARGS = (int)',',
            N_CONTOURS = 10,
            PLOT_MARGIN = 20,
            WEE_BIT = 3,
            NUMBER_LENGTH = 3;
        const float Z_MAX_MAX = float.MaxValue, Z_MIN_MIN = -float.MaxValue;
        const string EOL = "\n";

        // Below, data members which store the grid steps, the z values, the interpolation flag, the dimensions of the contour plot and the increments in the grid.
        int gridWidth, gridHeight;
        float[][] grid;

        bool logInterpolation = false;
        //Dimension d;
        //int imageWidth, imageHeight;
        double deltaX, deltaY;

        // Below, data members, most of which are adapted from
        // Fortran variables in Snyder's code:
        //const int ncv = N_CONTOURS;
        int[] l1 = new int[4];
        int[] l2 = new int[4];
        int[] ij = new int[2];
        int[] i1 = new int[2];
        int[] i2 = new int[2];
        int[] i3 = new int[6];
        int ibkey, icur, jcur, ii, jj, elle, ix, iedge, iflag, ni, ks;
        int cntrIndex, prevIndex;
        int idir, nxidir, k;
        float z1, z2, cval, zMax, zMin;
        double[] intersect = new double[4];
        double[] xy = new double[2];
        double[] prevXY = new double[2];
        //float[] cv = new float[ncv];
        float[] cv;
        bool jump;

        public ContourMaker()
        {
            
        }

        int sign(int a, int b)
        {
            a = Math.Abs(a);
            if (b < 0)
            {
                return -a;
            }
            else
            {
                return a;
            }
        }

        private void flagInvalidData()
        {
            if (cv.Length < 2)
            {
                cv = new float[2];
            }
            cv[0] = (float)0.0;
            cv[1] = (float)0.0;
        }

        private void findZMinMax()
        {
            zMin = float.NaN;
            zMax = float.NaN;
            for (int j = 0; j < gridHeight; j++)
            {
                for (int i = 0; i < gridWidth; i++)
                {
                    zMin = MathUtil.NanAwareMin(zMin, (float)grid[i][j]);
                    zMax = MathUtil.NanAwareMax(zMax, (float)grid[i][j]);
                }
            }
            if (zMin == zMax)
            {
                flagInvalidData();
            }

            Console.WriteLine("data range: " + zMin + " -- " + zMax);
        }

        public void AssignContourValues(float[] contourValues)
        {
            // Store the contour values.
            this.cv = new float[contourValues.Length];
            for (int i = 0; i < contourValues.Length; i++)
            {
                cv[i] = (float)contourValues[i];
            }
        }
        public void AssignContourValues(float contourStart, float contourInterval)
        {
            int i;
            double delta;

            if ((logInterpolation) && (zMin <= 0.0))
            {
                //InvalidData();
            }
            if (logInterpolation)
            {
                double temp = Math.Log(zMin);

                delta = (Math.Log(zMax) - temp) / cv.Length;
                for (i = 0; i < cv.Length; i++) cv[i] = (float)Math.Exp(temp + (i + 1) * delta);
            }
            else
            {
                // Determine the number of contour crosses that will occur between the minimum with the specified start and interval.
                double distFromMinToStart = contourInterval - zMin;

                int nContours = (int)Math.Floor((zMax - zMin) / contourInterval) + 1;

                if (nContours < 1)
                {
                    nContours = 1;
                }
                else if (nContours > 1000) {
                    nContours = 1000;
                }

                // If the starting contour is less than the minimum value, shift it so that it is the first valid contour value in the range.
                if (contourStart < zMin)
                {
                    Console.WriteLine("The minimum value is " + zMin);
                    int numContourIntervals = (int)Math.Ceiling((zMin - contourStart) / contourInterval);
                    Console.WriteLine("Num times: " + numContourIntervals);
                    Console.WriteLine("Interval: " + contourInterval);

                    contourStart += numContourIntervals * contourInterval;

                    Console.WriteLine("Adjusted the contour start value to " + contourStart);
                }

                // If the starting contour value misses some contours below, shift it so that it is the first valid contour in the range.
                if (contourStart - zMin >= contourInterval)
                {
                    Console.WriteLine("The minimum value is " + zMin);


                    int numContourIntervals = (int)Math.Floor((contourStart - zMin) / contourInterval);
                    contourStart -= numContourIntervals * contourInterval;

                    Console.WriteLine("Adjusted the contour start value down to " + contourStart);
                }

                // Make a new array for the contours.
                this.cv = new float[nContours];

                for (int j = 0; j < nContours; j++)
                {
                    cv[j] = (float)(contourStart + j * contourInterval);
                    Console.WriteLine("cv [" + j + "] = " + cv[j]);
                }
            }
        }

        //-------------------------------------------------------
        // "SetMeasurements" determines the dimensions of
        // the contour plot and the increments in the grid.
        //-------------------------------------------------------
        void SetMeasurements()
        {
            deltaX = 1.0f; // imageWidth / (gridWidth - 1.0);// d.width / (gridWidth - 1.0);
            deltaY = 1.0f; // imageHeight / (gridHeight - 1.0); // d.height / (gridHeight - 1.0);
        }

        //-------------------------------------------------------
        // "DrawKernel" is the guts of drawing and is called
        // directly or indirectly by "ContourPlotKernel" in order
        // to draw a segment of a contour or to set the pen
        // position "prevXY". Its action depends on "iflag":
        //
        // iflag == 1 means Continue a contour
        // iflag == 2 means Start a contour at a boundary
        // iflag == 3 means Start a contour not at a boundary
        // iflag == 4 means Finish contour at a boundary
        // iflag == 5 means Finish closed contour not at boundary
        // iflag == 6 means Set pen position
        //
        // If the constant "SHOW_NUMBERS" is true then when
        // completing a contour ("iflag" == 4 or 5) the contour
        // index is drawn adjacent to where the contour ends.
        //-------------------------------------------------------
        void DrawKernel(PointList g)
        {
            float prevU, prevV, u, v;

            if ((iflag == 1) || (iflag == 4) || (iflag == 5))
            {
                if (cntrIndex != prevIndex)
                { // Must change colour
                    prevIndex = cntrIndex;
                }

                //double prevXY0 = 1.0;
                //double prevXY1 = 1.0;
                //double xy0 = gridWidth;
                //double xy1 = gridHeight;

                prevU = extentWest + (float)(prevXY[0] - 0.5f) * cellAddX;
                prevV = extentNorth + (float)(prevXY[1] - 0.5f) * cellAddY;
                u = extentWest + (float)(xy[0] - 0.5f) * cellAddX;
                v = extentNorth + (float)(xy[1] - 0.5f) * cellAddY;

                yMinVal = MathUtil.NanAwareMin(yMinVal, prevV);
                yMaxVal = MathUtil.NanAwareMax(yMaxVal, prevV);

                g.DrawLine(u, v, prevU, prevV, cv[cntrIndex]);

                if ((SHOW_NUMBERS) && ((iflag == 4) || (iflag == 5)))
                {
                    //if (u == 0) u = u - WEE_BIT;
                    //else if (u == imageWidth) u = u + PLOT_MARGIN / 2;
                    //else if (v == 0) v = v - PLOT_MARGIN / 2;
                    //else if (v == imageHeight) v = v + WEE_BIT;
                    // g.drawString(cntrIndex + "", PLOT_MARGIN + v, PLOT_MARGIN + u);
                }
            }
            prevXY[0] = xy[0];
            prevXY[1] = xy[1];
        }

        public float yMinVal = float.NaN;
        public float yMaxVal = float.NaN;

        //-------------------------------------------------------
        // "DetectBoundary"
        //-------------------------------------------------------
        void DetectBoundary()
        {
            ix = 1;
            if (ij[1 - elle] != 1)
            {
                ii = ij[0] - i1[1 - elle];
                jj = ij[1] - i1[elle];
                if (grid[ii - 1][jj - 1] <= Z_MAX_MAX)
                {
                    ii = ij[0] + i2[elle];
                    jj = ij[1] + i2[1 - elle];
                    if (grid[ii - 1][jj - 1] < Z_MAX_MAX) ix = 0;
                }
                if (ij[1 - elle] >= l1[1 - elle])
                {
                    ix = ix + 2;
                    return;
                }
            }
            ii = ij[0] + i1[1 - elle];
            jj = ij[1] + i1[elle];
            if (grid[ii - 1][jj - 1] > Z_MAX_MAX)
            {
                ix = ix + 2;
                return;
            }
            if (grid[ij[0]][ij[1]] >= Z_MAX_MAX) ix = ix + 2;
        }

        //-------------------------------------------------------
        // "Routine_label_020" corresponds to a block of code
        // starting at label 20 in Synder's subroutine "GCONTR".
        //-------------------------------------------------------
        bool Routine_label_020()
        {
            l2[0] = ij[0];
            l2[1] = ij[1];
            l2[2] = -ij[0];
            l2[3] = -ij[1];
            idir = 0;
            nxidir = 1;
            k = 1;
            ij[0] = Math.Abs(ij[0]);
            ij[1] = Math.Abs(ij[1]);
            if (grid[ij[0] - 1][ij[1] - 1] > Z_MAX_MAX)
            {
                elle = idir % 2;
                ij[elle] = sign(ij[elle], l1[k - 1]);
                return true;
            }
            elle = 0;
            return false;
        }

        //-------------------------------------------------------
        // "Routine_label_050" corresponds to a block of code
        // starting at label 50 in Synder's subroutine "GCONTR".
        //-------------------------------------------------------
        bool Routine_label_050()
        {
            while (true)
            {
                if (ij[elle] >= l1[elle])
                {
                    if (++elle <= 1) continue;
                    elle = idir % 2;
                    ij[elle] = sign(ij[elle], l1[k - 1]);
                    if (Routine_label_150()) return true;
                    continue;
                }
                ii = ij[0] + i1[elle];
                jj = ij[1] + i1[1 - elle];
                if (grid[ii - 1][jj - 1] > Z_MAX_MAX)
                {
                    if (++elle <= 1) continue;
                    elle = idir % 2;
                    ij[elle] = sign(ij[elle], l1[k - 1]);
                    if (Routine_label_150()) return true;
                    continue;
                }
                break;
            }
            jump = false;
            return false;
        }

        //-------------------------------------------------------
        // "Routine_label_150" corresponds to a block of code
        // starting at label 150 in Synder's subroutine "GCONTR".
        //-------------------------------------------------------
        bool Routine_label_150()
        {
            while (true)
            {
                //------------------------------------------------
                // Lines from z[ij[0]-1][ij[1]-1]
                //	   to z[ij[0]  ][ij[1]-1]
                //	  and z[ij[0]-1][ij[1]]
                // are not satisfactory. Continue the spiral.
                //------------------------------------------------
                if (ij[elle] < l1[k - 1])
                {
                    ij[elle]++;
                    if (ij[elle] > l2[k - 1])
                    {
                        l2[k - 1] = ij[elle];
                        idir = nxidir;
                        nxidir = idir + 1;
                        k = nxidir;
                        if (nxidir > 3) nxidir = 0;
                    }
                    ij[0] = Math.Abs(ij[0]);
                    ij[1] = Math.Abs(ij[1]);
                    if (grid[ij[0] - 1][ij[1] - 1] > Z_MAX_MAX)
                    {
                        elle = idir % 2;
                        ij[elle] = sign(ij[elle], l1[k - 1]);
                        continue;
                    }
                    elle = 0;
                    return false;
                }
                if (idir != nxidir)
                {
                    nxidir++;
                    ij[elle] = l1[k - 1];
                    k = nxidir;
                    elle = 1 - elle;
                    ij[elle] = l2[k - 1];
                    if (nxidir > 3) nxidir = 0;
                    continue;
                }

                if (ibkey != 0) return true;
                ibkey = 1;
                ij[0] = icur;
                ij[1] = jcur;
                if (Routine_label_020()) continue;
                return false;
            }
        }

        //-------------------------------------------------------
        // "Routine_label_200" corresponds to a block of code
        // starting at label 200 in Synder's subroutine "GCONTR".
        // It has return values 0, 1 or 2.
        //-------------------------------------------------------
        short Routine_label_200(PointList g, bool[] workSpace)
        {
            while (true)
            {
                xy[elle] = 1.0 * ij[elle] + intersect[iedge - 1];
                xy[1 - elle] = 1.0 * ij[1 - elle];
                workSpace[2 * (gridWidth * (gridHeight * cntrIndex + ij[1] - 1)
                    + ij[0] - 1) + elle] = true;
                DrawKernel(g);
                if (iflag >= 4)
                {
                    icur = ij[0];
                    jcur = ij[1];
                    return 1;
                }
                ContinueContour();
                if (!workSpace[2 * (gridWidth * (gridHeight * cntrIndex
                    + ij[1] - 1) + ij[0] - 1) + elle]) return 2;
                iflag = 5;		// 5. Finish a closed contour
                iedge = ks + 2;
                if (iedge > 4) iedge = iedge - 4;
                intersect[iedge - 1] = intersect[ks - 1];
            }
        }

        //-------------------------------------------------------
        // "CrossedByContour" is true iff the current segment in
        // the grid is crossed by one of the contour values and
        // has not already been processed for that value.
        //-------------------------------------------------------
        bool CrossedByContour(bool[] workSpace)
        {
            ii = ij[0] + i1[elle];
            jj = ij[1] + i1[1 - elle];
            z1 = grid[ij[0] - 1][ij[1] - 1];
            z2 = grid[ii - 1][jj - 1];
            for (cntrIndex = 0; cntrIndex < cv.Length; cntrIndex++)
            {
                int i = 2 * (gridWidth * (gridHeight * cntrIndex + ij[1] - 1) + ij[0] - 1) + elle;

                if (i < workSpace.Length)
                {
                    if (!workSpace[i])
                    {
                        float x = cv[cntrIndex];
                        if ((x > Math.Min(z1, z2)) && (x <= Math.Max(z1, z2)))
                        {
                            workSpace[i] = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //-------------------------------------------------------
        // "ContinueContour" continues tracing a contour. Edges
        // are numbered clockwise, the bottom edge being # 1.
        //-------------------------------------------------------
        void ContinueContour()
        {
            short local_k;

            ni = 1;
            if (iedge >= 3)
            {
                ij[0] = ij[0] - i3[iedge - 1];
                ij[1] = ij[1] - i3[iedge + 1];
            }
            for (local_k = 1; local_k < 5; local_k++)
                if (local_k != iedge)
                {
                    ii = ij[0] + i3[local_k - 1];
                    jj = ij[1] + i3[local_k];
                    z1 = grid[ii - 1][jj - 1];
                    ii = ij[0] + i3[local_k];
                    jj = ij[1] + i3[local_k + 1];
                    z2 = grid[ii - 1][jj - 1];
                    if ((cval > Math.Min(z1, z2) && (cval <= Math.Max(z1, z2))))
                    {
                        if ((local_k == 1) || (local_k == 4))
                        {
                            float zz = z2;

                            z2 = z1;
                            z1 = zz;
                        }
                        intersect[local_k - 1] = (cval - z1) / (z2 - z1);
                        ni++;
                        ks = local_k;
                    }
                }
            if (ni != 2)
            {
                //-------------------------------------------------
                // The contour crosses all 4 edges of cell being
                // examined. Choose lines top-to-left & bottom-to-
                // right if interpolation point on top edge is
                // less than interpolation point on bottom edge.
                // Otherwise, choose the other pair. This method
                // produces the same results if axes are reversed.
                // The contour may close at any edge, but must not
                // cross itself inside any cell.
                //-------------------------------------------------
                ks = 5 - iedge;
                if (intersect[2] >= intersect[0])
                {
                    ks = 3 - iedge;
                    if (ks <= 0) ks = ks + 4;
                }
            }
            //----------------------------------------------------
            // Determine whether the contour will close or run
            // into a boundary at edge ks of the current cell.
            //----------------------------------------------------
            elle = ks - 1;
            iflag = 1;		// 1. Continue a contour
            jump = true;
            if (ks >= 3)
            {
                ij[0] = ij[0] + i3[ks - 1];
                ij[1] = ij[1] + i3[ks + 1];
                elle = ks - 3;
            }
        }



        //-------------------------------------------------------
        // "ContourPlotKernel" is the guts of this class and
        // corresponds to Synder's subroutine "GCONTR".
        //-------------------------------------------------------
        void ContourPlotKernel(PointList pointList, bool[] workSpace)
        {
            short val_label_200;

            l1[0] = gridWidth; l1[1] = gridHeight;
            l1[2] = -1; l1[3] = -1;
            i1[0] = 1; i1[1] = 0;
            i2[0] = 1; i2[1] = -1;
            i3[0] = 1; i3[1] = 0; i3[2] = 0;
            i3[3] = 1; i3[4] = 1; i3[5] = 0;
            prevXY[0] = 0.0; prevXY[1] = 0.0;
            xy[0] = 1.0; xy[1] = 1.0;
            cntrIndex = 0;
            prevIndex = -1;
            iflag = 6;
            DrawKernel(pointList);
            icur = Math.Max(1, Math.Min((int)Math.Floor(xy[0]), gridWidth));
            jcur = Math.Max(1, Math.Min((int)Math.Floor(xy[1]), gridHeight));
            ibkey = 0;
            ij[0] = icur;
            ij[1] = jcur;
            if (Routine_label_020() && Routine_label_150())
            {
                return;
            }
            if (Routine_label_050())
            {
                return;
            }
            while (true)
            {
                DetectBoundary();
                if (jump)
                {
                    if (ix != 0) iflag = 4; // Finish contour at boundary
                    iedge = ks + 2;
                    if (iedge > 4) iedge = iedge - 4;
                    intersect[iedge - 1] = intersect[ks - 1];
                    val_label_200 = Routine_label_200(pointList, workSpace);
                    if (val_label_200 == 1)
                    {
                        if (Routine_label_020() && Routine_label_150())
                        {
                            return;
                        }
                        if (Routine_label_050())
                        {
                            return;
                        }
                        continue;
                    }
                    if (val_label_200 == 2)
                    {
                        continue;
                    }
                    return;
                }
                if ((ix != 3) && (ix + ibkey != 0) && CrossedByContour(workSpace))
                {
                    //
                    // An acceptable line segment has been found.
                    // Follow contour until it hits a
                    // boundary or closes.
                    //
                    iedge = elle + 1;
                    cval = cv[cntrIndex];
                    if (ix != 1) iedge = iedge + 2;
                    iflag = 2 + ibkey;
                    intersect[iedge - 1] = (cval - z1) / (z2 - z1);
                    val_label_200 = Routine_label_200(pointList, workSpace);
                    if (val_label_200 == 1)
                    {
                        if (Routine_label_020() && Routine_label_150())
                        {
                            return;
                        }
                        if (Routine_label_050())
                        {
                            return;
                        }
                        continue;
                    }
                    if (val_label_200 == 2) continue;
                    return;
                }
                if (++elle > 1)
                {
                    elle = idir % 2;
                    ij[elle] = sign(ij[elle], l1[k - 1]);
                    if (Routine_label_150())
                    {
                        return;
                    }
                }
                if (Routine_label_050())
                {
                    return;
                }
            }
        }

        public void MakeContours(PointList g)
        {
            if (grid != null)
            {
                int workLength = 2 * gridWidth * gridHeight * cv.Length;
                bool[] workSpace; // Allocate below if data valid

                SetMeasurements();

                workSpace = new bool[workLength];
                ContourPlotKernel(g, workSpace);
            }
        }

        public void SetGrid(float[][] z)
        {
            if (z != null)
            {
                // Store a reference to the grid.
                this.grid = z;

                MakeMatrixRectangular();
                findZMinMax();

                Random r = new Random();

                // Replace all NaNs with 0.0.
                int nCols = z.Length;
                int nRows = z[0].Length;
                for (int j = 0; j < nRows; j++)
                {
                    for (int i = 0; i < nCols; i++)
                    {
                        if (float.IsNaN(z[i][j]))
                        {
                            z[i][j] = 0.0f;// (float)r.NextDouble() * 30.0f;
                        }
                    }
                }

                if (zMax > Z_MAX_MAX) zMax = Z_MAX_MAX;
                if (zMin < Z_MIN_MIN) zMin = Z_MIN_MIN;
                float range = zMax - zMin;
                if (range == 0.0f)
                {
                    range = 1.0f;
                }
            }
        }

        //-------------------------------------------------------
        // "AddRow" appends a new empty row to the end of "z"
        //-------------------------------------------------------
        public void AddRow()
        {
            int leng = grid.Length;
            float[][] temp;

            if (leng >= MAX_X_STEPS)
            {
                throw new Exception();
            }
            temp = new float[leng + 1][];
            Array.Copy(grid, 0, temp, 0, leng);
            grid = temp;
        }

        //-------------------------------------------------------
        // "AddColumn" appends "val" to end of last row in "z"
        //-------------------------------------------------------
        public void AddColumn(float val)
        {
            int i = grid.Length - 1;
            int leng = grid[i].Length;
            float[] temp;

            if (leng >= MAX_Y_STEPS)
            {
                throw new Exception();
            }
            temp = new float[leng + 1];
            Array.Copy(grid[i], 0, temp, 0, leng);
            temp[leng] = val;
            grid[i] = temp;
        }

        //-------------------------------------------------------
        // "MakeMatrixRectangular" appends zero(s) to the end of
        // any row of "z" which is shorter than the longest row.
        //-------------------------------------------------------
        public void MakeMatrixRectangular()
        {
            int i, y, leng;

            gridWidth = grid.Length;
            gridHeight = MIN_Y_STEPS;
            for (i = 0; i < gridWidth; i++)
            {
                y = grid[i].Length;
                if (gridHeight < y) gridHeight = y;
            }

            for (i = 0; i < gridWidth; i++)
            {
                leng = grid[i].Length;
                if (leng < gridHeight)
                {
                    float[] temp = new float[gridHeight];

                    Array.Copy(grid[i], 0, temp, 0, leng);
                    while (leng < gridHeight)
                    {
                        temp[leng++] = 0;
                    }
                    grid[i] = temp;
                }
            }

            //Console.WriteLine("Grid size is: " + gridWidth + ", " + gridHeight);
        }

        //-------------------------------------------------------
        // "ReturnZedMatrix" returns a string containing the
        // values in "z" for display in the results area.
        //-------------------------------------------------------
        public String ReturnZedMatrix()
        {
            String s, oneValue;
            int i, j;

            s = infoStrX + gridWidth + EOL + infoStrY + gridHeight + EOL;
            for (i = 0; i < gridWidth; i++)
            {
                for (j = 0; j < gridHeight; j++)
                {
                    oneValue = grid[i][j] + "";
                    while (oneValue.Length < NUMBER_LENGTH)
                        oneValue = " " + oneValue;
                    s = s + oneValue;
                    if (j < gridHeight - 1) s = s + " ";
                }
                s = s + EOL;
            }
            return s;
        }

        internal void setZ(float[][] z)
        {
            this.grid = z;
        }

        float extentWest, extentNorth, extentSouth;
        float cellAddX, cellAddY;
        public void SetExtent(Extent extent)
        {
            // Set the extent.
            this.extent = extent;

            // Calculate the necessary values from the extent.
            extentWest = (float)extent.West;
            extentNorth = (float)extent.North;
            extentSouth = (float)extent.South;
            cellAddX = (float)((extent.East - extent.West) / grid.Length);
            cellAddY = (float)((extent.South - extent.North) / grid[0].Length);

            Console.WriteLine("cell-add-x: " + cellAddX + "; cell-add-y: " + cellAddY);
        }
    }
}
