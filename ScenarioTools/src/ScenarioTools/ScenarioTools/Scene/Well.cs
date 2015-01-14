using ScenarioTools.ModflowReaders;

namespace ScenarioTools.Scene
{
    public class Well
    {
        #region Properties

        public int Row { get; set; }
        public int Column { get; set; }
        public double OpenTopElevation { get; set; }
        public double OpenBottomElevation { get; set; }

        #endregion Properties

        #region Constructors

        public Well()
        {
            Row = 0;
            Column = 0;
            OpenTopElevation = double.NaN;
            OpenBottomElevation = double.NaN;
        }
        public Well(int row, int column)
            : this()
        {
            Row = row;
            Column = column;
        }
        public Well(int row, int column, double openTopElevation, double openBottomElevation) : this(row,column)
        {
            OpenTopElevation = openTopElevation;
            OpenBottomElevation = openBottomElevation;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Return number of cells intersected by open interval of Well
        /// </summary>
        /// <returns></returns>
        public int CountCells(DiscretizationFile disFile) 
        {
            int k = 0;
            for (int lay = 1; lay <= disFile.getNlay(); lay++)
            {
                if (CellOpenLength(disFile, lay) > 0.0)
                {
                    k++;
                }
            }
            return k;
        }

        public double TotalOpenLength(DiscretizationFile disFile)
        {
            double length = 0.0;
            for (int i = 1; i <= disFile.getNlay(); i++)
            {
                length = length + CellOpenLength(disFile, i);
            }
            return length;
        }

        private double CellHeight(DiscretizationFile disFile, int layer)
        {
            double cellHeight = 0.0;
            try
            {
                if (!double.IsNaN(OpenTopElevation) && !double.IsNaN(OpenBottomElevation))
                {
                    if (Row > 0 && Column > 0 && layer > 0)
                    {
                        if (Row <= disFile.getNrow() && Column <= disFile.getNcol() && layer <= disFile.getNlay())
                        {
                            cellHeight = disFile.GetCellHeight(layer, Row, Column);
                            if (cellHeight < 0.0)
                            {
                                cellHeight = 0.0;
                            }
                            return cellHeight;
                        }
                    }
                }
            }
            catch
            {
            }
            return 0.0; ;
        }

        public double CellOpenLength(DiscretizationFile disFile, int layer)
        {
            // Define top of open interval in cell
            double openTopElev = disFile.GetCellTopElevation(layer, Row, Column);
            if (double.IsNaN(openTopElev) || double.IsNaN(this.OpenTopElevation))
            {
                return 0.0;
            }
            if (this.OpenTopElevation < openTopElev)
            {
                openTopElev = this.OpenTopElevation;
            }

            // Define bottom of open interval in cells
            double openBottomElev = disFile.GetCellBottomElevation(layer, Row, Column);
            if (double.IsNaN(openBottomElev) || double.IsNaN(this.OpenBottomElevation))
            {
                return 0.0;
            }
            if (this.OpenBottomElevation > openBottomElev)
            {
                openBottomElev = this.OpenBottomElevation;
            }

            if (openTopElev > openBottomElev)
            {
                return openTopElev - openBottomElev;
            }
            else
            {
                return 0.0;
            }
        }

        public double FractionOfOpenLength(DiscretizationFile disFile, int layer)
        {
            double totalOpenLength = TotalOpenLength(disFile);
            double cellOpenLength = CellOpenLength(disFile, layer);
            double fraction = cellOpenLength / totalOpenLength;
            return fraction;
        }

        #endregion Methods
    }
}
