using System;
using System.Collections.Generic;
using System.Text;

namespace ModflowOutputViewer
{
    public class GridGeoReference
    {
        #region Constructors
        public GridGeoReference()
        {
            OriginX = 0;
            OriginY = 0;
            Angle = 0;
            ProjectionString = "";
        }
        public GridGeoReference(double originX, double originY, double angle, string projectionString)
        {
            OriginX = originX;
            OriginY = originY;
            Angle = angle;
            ProjectionString = projectionString;
        }
        #endregion

        #region Public Properties
        private double _OriginX = 0;
        public double OriginX
        {
            get { return _OriginX; }
            set { _OriginX = value; }
        }

        private double _OriginY = 0;
        public double OriginY
        {
            get { return _OriginY; }
            set { _OriginY = value; }
        }

        private double _Angle = 0;
        public double Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        private string _ProjectionString = "";
        public string ProjectionString
        {
            get { return _ProjectionString; }
            set { _ProjectionString = value; }
        }
        #endregion

    }
}
