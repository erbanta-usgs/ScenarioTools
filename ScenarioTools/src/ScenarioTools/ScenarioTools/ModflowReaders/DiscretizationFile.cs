using System;
using System.IO;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.Util;

namespace ScenarioTools.ModflowReaders
{
    public class DiscretizationFile
    {
        // These variables are read from the file.
        int nlay, nrow, ncol, nper, itmuni, lenuni;
        int[] laycbd;
        double[] delr, delc;
        double[,] top;
        double[][,] botm;
        StressPeriod[] stressPeriods;
        ModflowTimeUnit modflowTimeUnit;

        // These variables are derived or added.
        ModelLayer[] modelLayers;
        Point2D anchorPoint;
        double rotationAngle;
        DateTime anchorDate;
        double[] xBounds, yBounds;

        DiscretizationFile(int nlay, int nrow, int ncol, int nper, int itmuni, int lenuni, 
                           int[] laycbd, double[] delr, double[] delc, double[,] top, 
                           double[][,] botm, StressPeriod[] stressPeriods)
        {
            // Store all values.
            this.nlay = nlay;
            this.nrow = nrow;
            this.ncol = ncol;
            this.nper = nper;
            this.itmuni = itmuni;
            this.lenuni = lenuni;
            this.laycbd = laycbd;
            this.delr = delr;
            this.delc = delc;
            this.top = top;
            this.botm = botm;
            this.stressPeriods = stressPeriods;

            // Make model layer objects.
            modelLayers = new ModelLayer[nlay];
            int bottomIndex = 0;
            for (int i = 0; i < nlay; i++)
            {
                // Get the upper bound for this layer.
                double[,] upperBound;
                if (i == 0)
                    upperBound = top;
                else
                    upperBound = botm[bottomIndex - 1];

                // If the layer does not have a confining bed (laycbd==0), make a model layer 
                // with just the top and bottom layers.
                if (laycbd[i] == 0)
                {
                    modelLayers[i] = new ModelLayer(this, upperBound, botm[bottomIndex], null);
                    bottomIndex++;
                }

                // If the layer has a confining bed (laycbd!=0), make a model layer with
                // the top, bottom, and confining bed layers.
                else
                {
                    modelLayers[i] = new ModelLayer(this, upperBound, botm[bottomIndex], botm[bottomIndex + 1]);
                    bottomIndex += 2;
                }
            }
        }
        public int getNlay()
        {
            return nlay;
        }
        public int getNrow()
        {
            return nrow;
        }
        public int getNcol()
        {
            return ncol;
        }
        public int getNper()
        {
            return nper;
        }
        public int getItmuni()
        {
            return itmuni;
        }
        public int getLenuni()
        {
            return lenuni;
        }
        public int getLaycbd(int index)
        {
            return laycbd[index];
        }
        public ModflowTimeUnit ModflowTimeUnit
        {
            get
            {
                return modflowTimeUnit;
            }
            private set
            {
                modflowTimeUnit = value;
            }
        }

        public TimeSpan GetSimulationTimeSpan()
        {
            TimeSpan simTimeSpan = new TimeSpan(0);
            for (int i = 0; i < nper; i++)
            {
                simTimeSpan = simTimeSpan + stressPeriods[i].getTimeSpan();
            }
            return simTimeSpan;
        }

        public StressPeriod getStressPeriod(int index)
        {
            return stressPeriods[index];
        }

        public int GetStressPeriodIndex(DateTime dateTime, DateTime simulationStartTime)
        {
            int index = -1;
            DateTime periodStartTime = simulationStartTime;
            DateTime periodEndTime;
            for (int i = 0; i < getNper(); i++)
            {
                periodEndTime = periodStartTime + stressPeriods[i].getTimeSpan();
                if (dateTime >= periodStartTime && dateTime <= periodEndTime)
                {
                    index = i;
                    break;
                }
                periodStartTime = periodEndTime;
            }
            return index;
        }

        public static DiscretizationFile fromFile(string file, bool freeFormat, NamefileInfo nfi)
        {
            StreamReader sr = null;
            try
            {
                // Open the file for reading.
                sr = File.OpenText(file);

                // Register the reader with the unit number.
                //Unit.registerReader(sr, unitNumber);

                // Discard the comment lines.
                string currentLine = sr.ReadLine();
                while (currentLine.StartsWith("#"))
                {
                    currentLine = sr.ReadLine();
                }

                // The current line is now the NLAY NROW NCOL NPER ITMUNI LENUNI line. Get the values from this line (element one of the file).
                String[] split = StringUtil.ParseLine(currentLine);
                int nlay = int.Parse(split[0]);
                int nrow = int.Parse(split[1]);
                int ncol = int.Parse(split[2]);
                int nper = int.Parse(split[3]);
                int itmuni = int.Parse(split[4]);
                int lenuni = int.Parse(split[5]);
                ModflowTimeUnit modflowTimeUnit = TimeHelper.GetModflowTimeUnit(itmuni);

                // Read element two of the file.
                // LAYCBD(NLAY)
                int[] laycbd = new int[nlay];
                split = StringUtil.ParseLine(sr.ReadLine());
                for (int i = 0; i < laycbd.Length; i++)
                    laycbd[i] = int.Parse(split[i]);

                // Item 3:
                // DELR(NCOL) - U1DREL
                double[] delr = ModflowHelpers.U1drel(sr, ncol, nfi);

                // Item 4: 
                // DELC(NROW) - U1DREL
                double[] delc = ModflowHelpers.U1drel(sr, nrow, nfi);

                // Read element five of the file.
                // Top(NCOL,NROW) - U2DREL
                double[,] top = ModflowHelpers.U2drel(sr, nrow, ncol, nfi);

                // Read element six of the file.
                // BOTM(NCOL,NROW) - U2DREL
                // Item 6 is repeated for each model layer and Quasi-3D confining bed in the grid. 
                // These layer variables are read in sequence going down from the top of the system. 
                // Thus, the number of BOTM arrays must be NLAY plus the number of Quasi-3D confining beds.
                int numQuasi3DConfiningBeds = getNumQuasi3DConfiningBeds(laycbd);
                int numBottomArrays = nlay + numQuasi3DConfiningBeds;
                double[][,] botm = new double[numBottomArrays][,];
                for (int i = 0; i < botm.Length; i++)
                {
                    botm[i] = ModflowHelpers.U2drel(sr, nrow, ncol, nfi);
                }

                // Read element seven of the file.
                StressPeriod[] stressPeriods = new StressPeriod[nper];
                TimeSpan timeSpan;
                for (int i = 0; i < nper; i++)
                {
                    stressPeriods[i] = StressPeriod.parse(sr.ReadLine());
                    stressPeriods[i].setTimeSpan(modflowTimeUnit);
                }
                // Close the file.
                sr.Close();

                // Return the discretization file.
                // FOR EACH STRESS PERIOD
                // PERLEN NSTP TSMULT Ss/tr
                DiscretizationFile disFile = new DiscretizationFile(nlay, nrow, ncol, nper, itmuni, 
                                                                    lenuni, laycbd, delr, delc, 
                                                                    top, botm, stressPeriods);
                disFile.modflowTimeUnit = TimeHelper.GetModflowTimeUnit(itmuni);
                return disFile;
            }
            catch (Exception e)
            {
                if (sr != null)
                {
                    try
                    {
                        sr.Close();
                    }
                    catch (Exception e2) { }
                }
                return null;
            }
        }
        private static int getNumQuasi3DConfiningBeds(int[] laycbd)
        {
            // LAYCBD—is a flag, with one value for each model layer, that indicates whether or not a 
            // layer has a Quasi-3D confining bed below it. 0 indicates no confining bed, and not zero 
            // indicates a confining bed. LAYCBD for the bottom layer must be 0.

            // Make a counter for the number of Quasi-3D confining beds.
            int numQuasi3DConfiningBeds = 0;

            // Count the number of confining beds.
            for (int i = 0; i < laycbd.Length; i++)
                if (laycbd[i] != 0)
                    numQuasi3DConfiningBeds++;

            // Return the number of confining beds.
            return numQuasi3DConfiningBeds;
        }

        /// <summary>
        /// Return cell top elevation
        /// </summary>
        /// <param name="layer">1-based layer index</param>
        /// <param name="row">1-based row index</param>
        /// <param name="column">1-based column index</param>
        /// <returns>Cell top elevation</returns>
        public double GetCellTopElevation(int layer, int row, int column)
        {
            try
            {
                // Convert Modflow (1-based) indices to 0-based indices
                int zeroBasedLayer = layer - 1;
                //int x = column - 1;
                //int y = row - 1;
                //double[,] top = this.modelLayers[zeroBasedLayer].getTop();
                //return top[x, y];
                ModelLayer modelLayer = this.modelLayers[zeroBasedLayer];
                return modelLayer.getTop(row, column);
            }
            catch
            {
            }
            return double.NaN;
        }

        /// <summary>
        /// Return cell bottom elevation
        /// </summary>
        /// <param name="layer">1-based layer index</param>
        /// <param name="row">1-based row index</param>
        /// <param name="column">1-based column index</param>
        /// <returns>Cell bottom elevation</returns>
        public double GetCellBottomElevation(int layer, int row, int column)
        {
            try
            {
                // Convert Modflow (1-based) layer index to 0-based index
                int zeroBasedLayer = layer - 1;
                //int x = column - 1;
                //int y = row - 1;
                //double[,] bottom = this.modelLayers[zeroBasedLayer].getBottom();
                //return bottom[x, y];
                ModelLayer modelLayer = this.modelLayers[zeroBasedLayer];
                return modelLayer.getBottom(row, column);
            }
            catch
            {
                return double.NaN;
            }
        }

        public double GetCellHeight(int layer, int row, int column)
        {
            //double cellTop = GetCellTopElevation(layer, row, column);
            //double cellBottom = GetCellBottomElevation(layer, row, column);
            //if (cellTop != double.NaN && cellBottom != double.NaN)
            //{
            //    double height = cellTop - cellBottom;
            //    if (height < 0.0)
            //    {
            //        height = 0.0;
            //    }
            //    return height;
            //}
            //else
            //{
            //    return double.NaN;
            //}
            int zeroBasedLayer = layer - 1;
            ModelLayer modelLayer = this.modelLayers[zeroBasedLayer];
            return modelLayer.GetCellHeight(row, column);
        }

        public double[] Delc
        {
            get
            {
                return delc;
            }
        }
        public double[] Delr
        {
            get
            {
                return delr;
            }
        }
        /// <summary>
        /// Returns an array of DateTime dimensioned [NPER+1].  The simulation start time is put into element [0].
        /// </summary>
        /// <param name="SimulationStartTime"></param>
        /// <returns></returns>
        public DateTime[] GetStressPeriodEndTimes(DateTime SimulationStartTime)
        {
            int nper = getNper();
            DateTime[] stressPeriodEndTimes = new DateTime[nper + 1];
            stressPeriodEndTimes[0] = SimulationStartTime;
            for (int i = 0; i < nper; i++)
            {
                stressPeriodEndTimes[i + 1] = stressPeriodEndTimes[i] + getStressPeriod(i).getTimeSpan();
            }
            return stressPeriodEndTimes;
        }

        /*
        public bool hasTransientStressPeriod() {
		// Return true if any of the stress periods are transient.
		for (StressPeriod stressPeriod : stressPeriods)
			if (stressPeriod.isTransient())
				return true;

		// None of the stress periods are transient. Return false.
		return false;
	}

        public FileType getFileType()
        {
            return FileType.DIS;
        }
        public void printInfo() {
		// Print the number of columns, rows, and layers.
		Console.WriteLine("discretization file");
		Console.WriteLine("===================");
		Console.WriteLine("ncol: " + ncol);
		Console.WriteLine("nrow: " + nrow);
		Console.WriteLine("nlay: " + nlay);

		// Print all of the elevation layers (top, all bottom, and all
		// confining beds).
		Console.WriteLine("\ntop:");
		printFloatArray(top, System.out);

		/*for (int i = 0; i < nlay; i++) {
			Console.WriteLine("\nbotm " + (i + 1) + ":");
			printFloatArray(modelLayers[i].getBottom(), System.out);

			if (modelLayers[i].hasConfiningBed()) {
				Console.WriteLine("\nconfining bed " + (i + 1) + ":");
				printFloatArray(modelLayers[i].getConfiningBed(), System.out);
			}
		}
         * */
    }
    /*
        static void printFloatArray(float[][] a, PrintStream s)
        {
            int width = a.Length;
            int height = a[0].Length;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                    s.print(a[i][j] + " ");
                s.println();
            }
        }

        // These methods are from the earlier version of the discretization file
        // class.
        public void setAnchorDate(Date anchorDate) {
		// Set the anchor date.
		this.anchorDate = anchorDate;
		
		Console.Write("Calculating stress period time intervals... ");
		
		// Make a calendar to assist in creation of the time spans.
		GregorianCalendar c = new GregorianCalendar();
		
		// Recompute the stress period time spans.
		Date startDate = anchorDate;
		for (int i = 0; i < stressPeriods.length; i++) {
			// Set the calendar to the start date.
			c.setTime(startDate);
			
			// Advance the calendar according to the time unit and the stress period length.
			c.add(getItmuniFieldNumber(), (int)stressPeriods[i].getPerlen());
			Date endDate = c.getTime();
			
			stressPeriods[i].setTimeSpan(new TimeSpan(startDate, endDate));
			
			//Console.WriteLine(startDate.getTime() + " : " + endDate.getTime());
			
			// The next start date is the current end date.
			startDate = endDate;
		}
		
		Console.WriteLine("done.");
	}
        private int getItmuniFieldNumber()
        {
            //// http://water.usgs.gov/nrp/gwsoftware/modflow2000/MFDOC/dis.htm
            /// 0 - undefined 
            /// 1 - seconds 
            /// 2 - minutes 
            /// 3 - hours 
            /// 4 - days 
            /// 5 - years 

            switch (itmuni)
            {
                case 1:
                    return GregorianCalendar.SECOND;
                case 2:
                    return GregorianCalendar.MINUTE;
                case 3:
                    return GregorianCalendar.HOUR;
                case 4:
                    return GregorianCalendar.DATE;
                case 5:
                    return GregorianCalendar.YEAR;
            }

            return GregorianCalendar.DATE;
        }
        public void setAnchorPoint(Point2D anchorPoint)
        {
            // Set the anchor point.
            this.anchorPoint = anchorPoint;

            // The x and y bounds, if present, are invalid.
            xBounds = null;
            yBounds = null;
        }
        public double[] getXBounds()
        {
            // If the anchor point is null, the bounds cannot be computed.
            if (anchorPoint == null)
                return null;

            // If the x-bounds array is null, compute it.
            if (xBounds == null)
                calculateXBounds();

            return xBounds;
        }
        private void calculateXBounds()
        {
            xBounds = new double[delr.Length + 1];
            xBounds[0] = anchorPoint.getX();
            for (int i = 0; i < delr.Length; i++)
                xBounds[i + 1] = xBounds[i] + delr[i];
        }
        public double[] getYBounds()
        {
            // If the anchor point is null, the bounds cannot be computed.
            if (anchorPoint == null)
                return null;

            // If the y-bounds array is null, compute it.
            if (yBounds == null)
                calculateYBounds();

            // Return the array.
            return yBounds;
        }
        private void calculateYBounds()
        {
            yBounds = new double[delc.Length + 1];
            yBounds[yBounds.Length - 1] = anchorPoint.getY();
            for (int i = delc.Length - 1; i >= 0; i--)
                yBounds[i] = yBounds[i + 1] - delc[i];
        }
        public int getNumStressPeriods()
        {
            return stressPeriods.Length;
        }
        public float[] getDelr()
        {
            return delr;
        }
        public float[] getDelc()
        {
            return delc;
        }
        public ModelLayer getModelLayer(int index)
        {
            // If the index is invalid, return null.
            if (index < 0 || index >= modelLayers.Length)
                return null;
            // Otherwise, return the layer at the specified index.
            else
                return modelLayers[index];
        }
        //
        // Maps an x coordinate to the x cell index.
        //
        public int getXCellIndex(double xCoordinate)
        {
            // If the anchor point has not been set, the value cannot be computed.
            //if (anchorPoint == null)
            //return -1;

            // If the x-bounds array is null, compute it.
            if (xBounds == null)
                calculateXBounds();

            // Calculate and return the index.
            return getCellIndex(xCoordinate, xBounds);
        }
        //
        // Maps a y coordinate to the y cell index.
        //
        public int getYCellIndex(double yCoordinate)
        {
            // If the anchor point has not been set, the value cannot be computed.
            if (anchorPoint == null)
                return -1;

            // If the y-bounds array is null, compute it.
            if (yBounds == null)
                calculateYBounds();

            // Calculate and return the index.
            return getCellIndex(yCoordinate, yBounds);
        }
        private static int getCellIndex(double coordinate, double[] bounds)
        {
            // If the coordinate is outside the range, return -1.
            if (coordinate < bounds[0] || coordinate > bounds[bounds.Length - 1])
            {
                return -1;
            }

            // We know that the bounds array is monotonically increasing, so we can do a binary search to save some time.
            // This is a modified binary search that will find the largest index for which the value at that index is less than the search value.
            int low = 0;
            int high = bounds.Length - 1;
            int mid;
            while (low <= high)
            {
                mid = (low + high) / 2;

                // if midpoint is too low
                if (bounds[mid] < coordinate)
                    // if point is contained between mid index and next index, return mid
                    if (bounds[mid + 1] >= coordinate)
                        return mid;
                    // otherwise, move low to mid + 1
                    else
                        low = mid + 1;
                // if midpoint is too high
                else if (bounds[mid] > coordinate)
                    // if point is contained between mid index and previous index, return mid - 1
                    if (bounds[mid - 1] <= coordinate)
                        return mid - 1;
                    // otherwise, move high to mid - 1
                    else
                        high = mid - 1;
                else
                    return mid;
            }

            return -1;
        }
        public void setRotationAngle(double rotationAngle)
        {
            this.rotationAngle = rotationAngle;
        }
        public double getRotationAngle()
        {
            return rotationAngle;
        }
        public OrthoGrid2D getGrid()
        {
            return new OrthoGrid2D(getXBounds(), getYBounds(), getRotationAngle());
        }
*/
}
