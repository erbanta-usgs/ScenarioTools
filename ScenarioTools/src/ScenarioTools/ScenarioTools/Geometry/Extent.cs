using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using GeoAPI.Geometries;
using ScenarioTools.Xml;
using USGS.Puma.NTS.Geometries;

namespace ScenarioTools.Geometry
{
    public class Extent : GeoAPI.Geometries.IEnvelope
    {
        private const string XML_NODE_NAME = "ScenarioToolsGeometryExtent";
        public const string XML_NODE_TYPE_EXTENT = "extent";
        private const string XML_NAME_KEY = "name";
        private const string XML_NAME_DEFAULT_VALUE = "";

        // These are projected coordinates (e.g. UTM meters), not lat-long,
        // so _east should be > _west
        private double _west, _south, _east, _north;
        private bool _yIsFlipped;
        private string _name;
        private long _key;
        private USGS.Puma.NTS.Geometries.Envelope _envelope;

        #region Constructors
        public Extent()
        {
            _name = "";
            _yIsFlipped = false;
            _envelope = new USGS.Puma.NTS.Geometries.Envelope();
            _key = getUniqueIdentifier();
            adjustExtent();
        }
        public Extent(string name) : this()
        {
            _name = name;
        }
        public Extent(double west, double south, double east, double north) : this()
        {
            // If all arguments are NaN, it's a null extent, so no action needed
            if (!double.IsNaN(west) || !double.IsNaN(south) || !double.IsNaN(east) || !double.IsNaN(north))
            {
                _name = "";
                // If y is flipped, indicate so and unflip.
                if (north < south)
                {
                    double temp = north;
                    north = south;
                    south = temp;
                    _yIsFlipped = true;
                }
                else
                {
                    _yIsFlipped = false;
                }

                // Store the extent values.
                this._west = west;
                this._south = south;
                this._east = east;
                this._north = north;
                _envelope = new USGS.Puma.NTS.Geometries.Envelope(west, east, south, north);
            }
        }
        public Extent(Extent extent) : this(extent._west, extent._south, extent._east, extent._north)
        {
            this._yIsFlipped = extent._yIsFlipped;
            this.Name = extent.Name;
        }
        public Extent(IEnvelope envelope)
            : this(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY)
        {
        }
        public Extent(ICoordinate point0, ICoordinate point1) : this()
        {
            if (point0.X < point1.X)
            {
                _west = point0.X;
                _east = point1.X;
            }
            else
            {
                _west = point1.X;
                _east = point0.X;
            }
            if (point0.Y < point1.Y)
            {
                _south = point0.Y;
                // Use property to invoke adjustExtent
                North = point1.Y;
            }
            else
            {
                _south = point1.Y;
                // Use property to invoke adjustExtent
                North = point0.Y;
            }
        }
        public static Extent FromFile(string filename)
        {
            StreamReader sr = null;
            Extent extent;

            // Read the extent from the specified file.
            try
            {
                sr = File.OpenText(filename);
                double west = double.Parse(sr.ReadLine());
                double south = double.Parse(sr.ReadLine());
                double east = double.Parse(sr.ReadLine());
                double north = double.Parse(sr.ReadLine());
                extent = new Extent(west, south, east, north);
                try
                {
                    extent._name = Path.GetFileNameWithoutExtension(filename);
                }
                catch
                {
                    extent._name = "";
                }
            }
            catch
            {
                extent = null;
            }

            // Close the reader.
            if (sr != null)
            {
                try
                {
                    sr.Close();
                }
                catch { }
            }

            // Return the result.
            return extent;
        }
        #endregion Constructors

        public double West
        {
            get
            {
                return _west;
            }
            set
            {
                _west = value;
                if (!double.IsNaN(_west) && !double.IsNaN(_east) && !double.IsNaN(_south) && !double.IsNaN(_north))
                {
                    _envelope.Init(_west, _east, _south, _north);
                    adjustExtent();
                }
            }
        }
        public double South
        {
            get
            {
                return _south;
            }
            set
            {
                _south = value;
                if (!double.IsNaN(_west) && !double.IsNaN(_east) && !double.IsNaN(_south) && !double.IsNaN(_north))
                {
                    _envelope.Init(_west, _east, _south, _north);
                    adjustExtent();
                }
            }
        }
        public double East
        {
            get
            {
                return _east;
            }
            set
            {
                _east = value;
                if (!double.IsNaN(_west) && !double.IsNaN(_east ) && !double.IsNaN(_south) && !double.IsNaN(_north))
                {
                    _envelope.Init(_west, _east, _south, _north);
                    adjustExtent();
                }
            }
        }
        public double North
        {
            get
            {
                return _north;
            }
            set
            {
                _north = value;
                if (!double.IsNaN(_west) && !double.IsNaN(_east) && !double.IsNaN(_south) && !double.IsNaN(_north))
                {
                    _envelope.Init(_west, _east, _south, _north);
                    adjustExtent();
                }
            }
        }
        public double Width
        {
            get
            {
                return Math.Abs(West - East);
            }
        }
        public double Height
        {
            get
            {
                return Math.Abs(North - South);
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public bool YIsFlipped
        {
            get
            {
                return _yIsFlipped;
            }
            set
            {
                _yIsFlipped = value;
            }
        }
        public long Key
        {
            get
            {
                return _key;
            }
        }

        private void adjustExtent(IEnvelope env)
        {
            if (env.IsNull)
            {
                _west = double.NaN;
                _east = double.NaN;
                _south = double.NaN;
                _north = double.NaN;
            }
            else
            {
                _west = env.MinX;
                _east = env.MaxX;
                _south = env.MinY;
                _north = env.MaxY;
            }
        }
        private void adjustExtent()
        {
            adjustExtent(_envelope);
        }
        private static RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
        private static long getUniqueIdentifier()
        {
            // This method is copied from WorkspaceUtil.GetUniqueIdentifier
            //
            // Remove a time-dependent number of values.
            int cycle = DateTime.Now.Millisecond % 50;
            byte[] discard = new byte[cycle];
            rng.GetBytes(discard);

            // Produce and return a long int value.
            byte[] buffer = new byte[sizeof(long)];
            rng.GetBytes(buffer);
            return Math.Abs(BitConverter.ToInt64(buffer, 0));
        }
        private static Extent getCachedExtent(Stream stream)
        {
            // Declare the reader and Extent fields
            BinaryReader br = null;
            //string name;
            double west, south, east, north;
            bool yIsFlipped;

            // Read data from file
            try
            {
                // Make a binary reader on the stream.
                br = new BinaryReader(stream);

                // Read the name
                //name = br.ReadString();

                // Read the coordinates
                west = br.ReadDouble();
                south = br.ReadDouble();
                east = br.ReadDouble();
                north = br.ReadDouble();

                // Read yIsFlipped
                yIsFlipped = br.ReadBoolean();
            }
            catch
            {
                return null;
            }
            Extent extent = new Extent(west, south, east, north);
            extent.YIsFlipped = yIsFlipped;
            return extent;
        }
        public static Extent GetEnclosingExtent(List<Extent> extents)
        {
            // If extents contains one extent, just copy it
            if (extents.Count == 1)
            {
                return new Extent(extents[0]);
            }

            List<IEnvelope> envelopes = new List<IEnvelope>();
            for (int i = 0; i < extents.Count; i++)
            {
                // Only include non-null extents when generating enclosing extent.
                if (!Extent.ExtentIsNull(extents[i]))
                {
                    envelopes.Add((IEnvelope) extents[i]);
                }
            }
            return GetEnclosingExtent(envelopes);
        }
        public static Extent GetEnclosingExtent(List<IEnvelope> envelopes)
        {
            // Initialize limits for enclosing extent
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            // Search to find limits for enclosing extent
            if (envelopes != null)
            {
                for (int i = 0; i < envelopes.Count; i++)
                {
                    if (envelopes[i].MinX < minX) minX = envelopes[i].MinX;
                    if (envelopes[i].MinY < minY) minY = envelopes[i].MinY;
                    if (envelopes[i].MaxX > maxX) maxX = envelopes[i].MaxX;
                    if (envelopes[i].MaxY > maxY) maxY = envelopes[i].MaxY;
                }
            }

            // Define enclosing extent
            if (minX < maxX && minY < maxY)
            {
                Extent newExtent = new Extent(minX, minY, maxX, maxY);
                return newExtent;
            }
            else
            {
                return new Extent();
            }
        }
        public bool HasDefaultEnvelope()
        {
            Envelope defaultEnvelop = new Envelope();
            return defaultEnvelop.Equals(this);
        }
        public static bool ExtentIsNull(Extent extent)
        {
            if (extent == null)
            {
                return true;
            }
            if (extent.IsNull)
            {
                return true;
            }
            return false;
        }
        public void CopyFrom(Extent sourceExtent)
        {
            _name = sourceExtent._name;
            _east = sourceExtent.East;
            _west = sourceExtent.West;
            _north = sourceExtent.North;
            _south = sourceExtent.South;
            _envelope.Init(_west, _east, _south, _north);
            adjustExtent();
            //_envelope = new USGS.Puma.NTS.Geometries.Envelope(West, East, South, North);
        }
        public override string ToString()
        {
            return Name;
        }
        public Coordinate[] GetCoordinates()
        {
            Coordinate[] points = new Coordinate[5];
            points[0] = new Coordinate(_west, _north);
            points[1] = new Coordinate(_west, _south);
            points[2] = new Coordinate(_east, _south);
            points[3] = new Coordinate(_east, _north);
            points[4] = new Coordinate(_west, _north);
            return points;
        }
        public LineString GetLineString()
        {
            return new LineString(GetCoordinates());
        }
        public Polygon GetPolygon()
        {
            LinearRing shell = new LinearRing(GetCoordinates());
            return new Polygon(shell);
        }
        public static Extent FromXml(XmlElement element)
        {
            Extent extent = new Extent(element.GetAttribute("name"));
            extent._key = XmlUtil.SafeGetLongAttribute(element, "key", getUniqueIdentifier());
            return extent;
        }
        public void LoadCacheFromStream(Stream stream)
        {
            Extent extent = getCachedExtent(stream);
            this._west = extent.West;
            this._south = extent.South;
            this._east = extent.East;
            this._north = extent.North;
            this._yIsFlipped = extent.YIsFlipped;
            this._envelope = extent._envelope;
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the XML element that represents this object.
            XmlElement element = document.CreateElement(XML_NODE_NAME);
            element.SetAttribute("NodeType", XML_NODE_TYPE_EXTENT);
            element.SetAttribute("key", _key.ToString());

            // Prepare the XML element.  
            prepareXmlElement(document, element, targetFileName);
            // The XML element contains only the Extent.Name at this point.

            // Return the result.
            return element;
        }
        private void prepareXmlElement(XmlDocument document, XmlElement element, string targetFileName)
        {
            // Set the name attribute.
            XmlUtil.FrugalSetAttribute(element, XML_NAME_KEY, Name, XML_NAME_DEFAULT_VALUE);
        }

        #region IEnvelope members
        #region IEnvelope properties
        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <remarks></remarks>
        public double Area 
        {
            get
            {
                return _envelope.Area;
            }
        }
        /// <summary>
        /// Gets the centre.
        /// </summary>
        /// <remarks></remarks>
        public ICoordinate Centre 
        {
            get
            {
                return _envelope.Centre;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <remarks></remarks>
        public bool IsNull
        {
            get
            {
                return _envelope.IsNull;
            }
        }
        /// <summary>
        /// Gets the max X.
        /// </summary>
        /// <remarks></remarks>
        public double MaxX 
        {
            get
            {
                return _envelope.MaxX;
            }
        }
        /// <summary>
        /// Gets the max Y.
        /// </summary>
        /// <remarks></remarks>
        public double MaxY 
        {
            get
            {
                return _envelope.MaxY;
            }
        }
        /// <summary>
        /// Gets the min X.
        /// </summary>
        /// <remarks></remarks>
        public double MinX 
        {
            get
            {
                return _envelope.MinX;
            }
        }
        /// <summary>
        /// Gets the min Y.
        /// </summary>
        /// <remarks></remarks>
        public double MinY 
        {
            get
            {
                return _envelope.MinY;
            }
        }
        #endregion IEnvelope properties
        #region IEnvelope methods
        /// <summary>
        /// Determines whether [contains] [the specified x].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns><c>true</c> if [contains] [the specified x]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool Contains(double x, double y)
        {
            return _envelope.Contains(x, y);
        }
        /// <summary>
        /// Determines whether [contains] [the specified p].
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns><c>true</c> if [contains] [the specified p]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool Contains(ICoordinate p)
        {
            return _envelope.Contains(p);
        }
        /// <summary>
        /// Determines whether [contains] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [contains] [the specified other]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool Contains(IEnvelope other)
        {
            return _envelope.Contains(other);
        }
        /// <summary>
        /// Distances the specified env.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double Distance(IEnvelope env)
        {
            return _envelope.Distance(env);
        }
        /// <summary>
        /// Expands by.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <remarks></remarks>
        public void ExpandBy(double distance)
        {
            _envelope.ExpandBy(distance);
            adjustExtent();
        }
        /// <summary>
        /// Expands by.
        /// </summary>
        /// <param name="deltaX">The delta X.</param>
        /// <param name="deltaY">The delta Y.</param>
        /// <remarks></remarks>
        public void ExpandBy(double deltaX, double deltaY)
        {
            _envelope.ExpandBy(deltaX, deltaY);
            adjustExtent();
        }
        /// <summary>
        /// Expands to include.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <remarks></remarks>
        public void ExpandToInclude(ICoordinate p)
        {
            _envelope.ExpandToInclude(p);
            adjustExtent();
        }
        /// <summary>
        /// Expands to include.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        public void ExpandToInclude(double x, double y)
        {
            _envelope.ExpandToInclude(x, y);
            adjustExtent();
        }
        /// <summary>
        /// Expands to include.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <remarks></remarks>
        public void ExpandToInclude(IEnvelope other)
        {
            _envelope.ExpandToInclude(other);
            adjustExtent();
        }
        /// <summary>
        /// Inits this instance.
        /// </summary>
        /// <remarks></remarks>
        public void Init()
        {
            _envelope.Init();
            adjustExtent();
        }
        /// <summary>
        /// Inits using the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <remarks></remarks>
        public void Init(ICoordinate p)
        {
            _envelope.Init(p);
            adjustExtent();
        }
        /// <summary>
        /// Inits using the specified env.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <remarks></remarks>
        public void Init(IEnvelope env)
        {
            _envelope.Init(env);
            adjustExtent();
        }
        /// <summary>
        /// Inits using the specified p1 and p2.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <remarks></remarks>
        public void Init(ICoordinate p1, ICoordinate p2)
        {
            _envelope.Init(p1, p2);
            adjustExtent();
        }
        /// <summary>
        /// Inits using the specified x1, x2, y1, and y2.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="y2">The y2.</param>
        /// <remarks></remarks>
        public void Init(double x1, double x2, double y1, double y2)
        {
            _envelope.Init(x1, x2, y1, y2);
            adjustExtent();
        }
        /// <summary>
        /// Intersections the specified env.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnvelope Intersection(IEnvelope env)
        {
            return _envelope.Intersection(env);
        }
        /// <summary>
        /// Translates the specified trans X.
        /// </summary>
        /// <param name="transX">The trans X.</param>
        /// <param name="transY">The trans Y.</param>
        /// <remarks></remarks>
        public void Translate(double transX, double transY)
        {
            _envelope.Translate(transX, transY);
            adjustExtent();
        }
        /// <summary>
        /// Unions the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnvelope Union(IPoint point)
        {
            return _envelope.Union(point);
        }
        /// <summary>
        /// Unions the specified coord.
        /// </summary>
        /// <param name="coord">The coord.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnvelope Union(ICoordinate coord)
        {
            return _envelope.Union(coord);
        }
        /// <summary>
        /// Unions the specified box.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnvelope Union(IEnvelope box)
        {
            return _envelope.Union(box);
        }
        /// <summary>
        /// Intersects the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Intersects(ICoordinate p)
        {
            return _envelope.Intersects(p);
        }
        /// <summary>
        /// Intersectses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Intersects(double x, double y)
        {
            return _envelope.Intersects(x, y);
        }
        /// <summary>
        /// Intersects the specified other envelope.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Intersects(IEnvelope other)
        {
            return _envelope.Intersects(other);
        }
        /// <summary>
        /// Sets to null.
        /// </summary>
        /// <remarks></remarks>
        public void SetToNull()
        {
            _envelope.SetToNull();
            adjustExtent();
        }
        /// <summary>
        /// Zooms the specified per cent.
        /// </summary>
        /// <param name="perCent">The per cent.</param>
        /// <remarks></remarks>
        public void Zoom(double perCent)
        {
            _envelope.Zoom(perCent);
            adjustExtent();
        }
        /// <summary>
        /// Overlapses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Overlaps(IEnvelope other)
        {
            return _envelope.Overlaps(other);
        }
        /// <summary>
        /// Overlapses the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Overlaps(ICoordinate p)
        {
            return _envelope.Overlaps(p);
        }
        /// <summary>
        /// Overlapses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Overlaps(double x, double y)
        {
            return _envelope.Overlaps(x, y);
        }
        /// <summary>
        /// Sets the centre.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks></remarks>
        public void SetCentre(double width, double height)
        {
            _envelope.SetCentre(width, height);
            adjustExtent();
        }
        /// <summary>
        /// Sets the centre.
        /// </summary>
        /// <param name="centre">The centre.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks></remarks>
        public void SetCentre(IPoint centre, double width, double height)
        {
            _envelope.SetCentre(centre, width, height);
            adjustExtent();
        }
        /// <summary>
        /// Sets the centre.
        /// </summary>
        /// <param name="centre">The centre.</param>
        /// <remarks></remarks>
        public void SetCentre(ICoordinate centre)
        {
            _envelope.SetCentre(centre);
            adjustExtent();
        }
        /// <summary>
        /// Sets the centre.
        /// </summary>
        /// <param name="centre">The centre.</param>
        /// <remarks></remarks>
        public void SetCentre(IPoint centre)
        {
            _envelope.SetCentre(centre);
            adjustExtent();
        }
        /// <summary>
        /// Sets the centre.
        /// </summary>
        /// <param name="centre">The centre.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks></remarks>
        public void SetCentre(ICoordinate centre, double width, double height)
        {
            _envelope.SetCentre(centre, width, height);
            adjustExtent();
        }
        #endregion IEnvelope methods
        #endregion IEnvelope members

        #region ICloneable members
        /// <summary>
        /// Creates a deep copy of the current extent.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            //return new Extent(_envelope.MinX, _envelope.MaxX, _envelope.MinY, _envelope.MaxY);
            return new Extent(this);
        }
        #endregion ICloneable members

        #region IComparable members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            return _envelope.CompareTo((IEnvelope)other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IEnvelope other)
        {
            return _envelope.CompareTo(other);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IEnvelope other)
        {
            bool same = _envelope.Equals(other);
            if (same)
            {
                if (other is Extent)
                {
                    Extent otherExtent = (Extent)other;
                    same = this.Name == otherExtent.Name;
                }
            }
            return same;
        }
        #endregion IComparable members
    }
}
