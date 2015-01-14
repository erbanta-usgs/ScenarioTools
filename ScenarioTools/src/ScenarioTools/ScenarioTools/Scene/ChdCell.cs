using USGS.Puma.FiniteDifference;

namespace ScenarioTools.Scene
{
    public class ChdCell : ScenarioFeature
    {

        #region Constructors

        public ChdCell()
        {
            _type = PackageType.ChdType;
            _lay = 0;
            _row = 0;
            _col = 0;
        }

        public ChdCell(int row, int col) : this()
        {
            _row = row;
            _col = col;
        }

        public ChdCell(int lay, int row, int col) : this()
        {
            _lay = lay;
            _row = row;
            _col = col;
        }

        public ChdCell(GridCell gridCell) : this()
        {
            _lay = gridCell.Layer;
            _row = gridCell.Row;
            _col = gridCell.Column;
        }

        #endregion Constructors

        #region Methods

        public override object Clone()
        {
            ChdCell chdCell = new ChdCell();
            chdCell._lay = _lay;
            chdCell._col = _col;
            chdCell._row = _row;
            chdCell._name = _name;
            return chdCell;
        }

        public override void Draw()
        {
        }

        #endregion Methods
    }
}
