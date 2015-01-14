using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using MapTools;
using ScenarioTools.Spatial;
using ScenarioTools.Util;

namespace ScenarioTools.Geometry
{
    public class Polygon2D : Shape2D
    {
        private Range2D range;
        private Point2D[] vertices;

        public Polygon2D(Point2D[] vertices)
        {
            this.vertices = vertices;
        }

        public override int NumVertices
        {
            get
            {
                return vertices.Length;
            }
        }

        public override Range2D Extent
        {
            get
            {
                if (range == null)
                {
                    range = GeometryHelper.CalculateRange(vertices);
                }
                return range;
            }
        }
        public Point2D[] GetIntersections(LineSegment2D segment)
        {
            // Make a list for the intersections.
            List<Point2D> intersections = new List<Point2D>();

            // Check each edge of this polygon for intersections with the line segment.
            int numVertices = this.NumVertices;
            for (int i = 0; i < numVertices; i++)
            {
                Point2D p1 = vertices[i];
                Point2D p2 = vertices[(i + 1) % numVertices];
                LineSegment2D s = new LineSegment2D(p1, p2);
                if (segment.intersects(s))
                {
                    intersections.Add(segment.intersection(s, true));
                }
            }

            // Sort the segments from the beginning to the end of the original segment.
            Point2D[] intersectionsArray = intersections.ToArray();
            if (segment.getP1().X < segment.getP2().X)
            {
                Array.Sort(intersectionsArray, delegate(Point2D point1, Point2D point2)
                {
                    return point1.X.CompareTo(point2.X);
                });
            }
            else if (segment.getP1().X > segment.getP2().X) {
                Array.Sort(intersectionsArray, delegate(Point2D point1, Point2D point2)
                {
                    return point2.X.CompareTo(point1.X);
                });
            }
            else if (segment.getP1().Y < segment.getP2().Y) {
                Array.Sort(intersectionsArray, delegate(Point2D point1, Point2D point2)
                {
                    return point1.Y.CompareTo(point2.Y);
                });
            }
            else
            {
                Array.Sort(intersectionsArray, delegate(Point2D point1, Point2D point2)
                {
                    return point2.Y.CompareTo(point1.Y);
                });
            }

            // Return the result.
            return intersectionsArray;
        }
        public LineSegment2D[] SplitSegment(LineSegment2D segment)
        {
            // Get the intersections of this polygon's edges with the segment.
            Point2D[] intersections = GetIntersections(segment);

            // Make line segments out of each piece.
            LineSegment2D[] segments = new LineSegment2D[intersections.Length + 1];
            Point2D start = segment.getP1();
            for (int i = 0; i < intersections.Length; i++)
            {
                segments[i] = new LineSegment2D(start, intersections[i]);
                start = intersections[i];
            }

            // Make the last segment.
            segments[segments.Length - 1] = new LineSegment2D(start, segment.getP2());

            // Return the segments.
            return segments;
        }
        public bool Contains(Point2D p)
        {
            int numIntersections = 0;

            int numVertices = this.NumVertices;
            for (int i = 0; i < NumVertices; i++)
            {
                Point2D p1 = vertices[i];
                Point2D p2 = vertices[(i + 1) % numVertices];
                if (p.Y > Math.Min(p1.Y, p2.Y))
                {
                    if (p.Y <= Math.Max(p1.Y, p2.Y))
                    {
                        if (p.X <= Math.Max(p1.X, p2.X))
                        {
                            if (p1.Y != p2.Y)
                            {
                                double xIntersection = (p.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                if (p1.X == p2.X || p.X <= xIntersection)
                                {
                                    numIntersections++;
                                }
                            }
                        }
                    }
                }
            }

            return numIntersections % 2 == 1;
        }
        public static void WriteShapefile(string baseFilename, Polygon2D[] polygons, SpatialReference spatialReference)
        {
            // The shape type is polygon.
            ShapeLib.ShapeType shpType = ShapeLib.ShapeType.Polygon;

            // Write the projection file.
            FileUtil.WriteFile(baseFilename + ".prj", new string[] { spatialReference.GetWktString() });

            // Create the shapefile.
            IntPtr shapefile = ShapeLib.SHPCreate(baseFilename + ".shp", shpType);
            if (shapefile.Equals(IntPtr.Zero))
            {
                throw new FieldAccessException("Unable to create shapefile: " + baseFilename + ".shp");
            }

            // Create the DBF file.
            IntPtr dbfFile = ShapeLib.DBFCreate(baseFilename + ".dbf");

            // Add the appropriate fields to the shapefile.
            DbfFieldDescriptor[] fieldDescriptors = DbfFieldDescriptor.GetFieldDescriptors(polygons);
            Console.WriteLine("There will be " + fieldDescriptors.Length + " fields in the shapefile.");
            for (int i = 0; i < fieldDescriptors.Length; i++)
            {
                fieldDescriptors[i].AddToDbf(dbfFile);
            }

            // Add the shapes to the file.
            for (int j = 0; j < polygons.Length; j++) {
                // Make the coordinate arrays.
                double[] xCoord = polygons[j].getXCoords();
                double[] yCoord = polygons[j].getYCoords();

                // Make and write the shape.
                IntPtr pShp = ShapeLib.SHPCreateObject(shpType, -1, 0, null, null, xCoord.Length, xCoord, yCoord, null, null);
                ShapeLib.SHPWriteObject(shapefile, -1, pShp);

                // Write the shape attributes.
                for (int i = 0; i < fieldDescriptors.Length; i++)
                {
                    string name = fieldDescriptors[i].Name;
                    object value = polygons[j].GetAttributeValue(name);
                    fieldDescriptors[i].WriteAttributeValue(dbfFile, j, i, value);
                }

                // Reclaim the resources consumed by the shape.
                ShapeLib.SHPDestroyObject(pShp);
            }

            // Close the shapefile and the DBF file.
            ShapeLib.SHPClose(shapefile);
            ShapeLib.DBFClose(dbfFile);
        }

        private double[] getXCoords()
        {
            // Make an array of the x-coordinates of this shape.
            int n = this.NumVertices + 1;
            double[] xCoords = new double[n];
            for (int i = 0; i < n; i++)
            {
                xCoords[i] = this.vertices[i % this.NumVertices].X;
            }

            // Return the array.
            return xCoords;
        }
        private double[] getYCoords()
        {
            // Make an array of the y-coordinates of this shape.
            int n = this.NumVertices + 1;
            double[] yCoords = new double[n];
            for (int i = 0; i < n; i++)
            {
                yCoords[i] = this.vertices[i % this.NumVertices].Y;
            }

            // Return the array.
            return yCoords;
        }

        public override Point2D GetVertex(int index)
        {
            return vertices[index];
        }
    }
}
