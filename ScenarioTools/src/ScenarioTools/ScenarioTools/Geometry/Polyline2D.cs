using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using MapTools;
using ScenarioTools.Spatial;
using ScenarioTools.Util;

namespace ScenarioTools.Geometry
{
    public class Polyline2D : Shape2D
    {
        private Range2D range;
        private Point2D[] vertices;

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
        public override int NumVertices
        {
            get
            {
                return vertices.Length;
            }
        }

        public Polyline2D(Point2D[] vertices)
        {
            // Store a reference to the vertex array.
            this.vertices = vertices;
        }
        public override Point2D GetVertex(int index)
        {
            return vertices[index];
        }

        public LineSegment2D[] splitSegment(LineSegment2D segment)
        {
            Point2D[] intersections = GetIntersections(segment);

            LineSegment2D[] segments = new LineSegment2D[intersections.Length + 1];
            Point2D start = segment.getP1();
            for (int i = 0; i < intersections.Length; i++)
            {
                segments[i] = new LineSegment2D(start, intersections[i]);
                start = intersections[i];
            }

            segments[segments.Length - 1] = new LineSegment2D(start, segment.getP2());

            return segments;
        }
        public Point2D[] GetIntersections(LineSegment2D segment)
        {
            // This method returns intersections in order from start to end point of segment.
            List<Point2D> intersections = new List<Point2D>();

            int numVertices = getNumVertices();
            for (int i = 0; i < numVertices; i++)
            {
                Point2D p1 = GetVertex(i);
                Point2D p2 = GetVertex((i + 1) % numVertices);
                LineSegment2D s = new LineSegment2D(p1, p2);
                if (segment.intersects(s))
                    intersections.Add(segment.intersection(s, true));
            }

            if (segment.getP1().GetX() < segment.getP2().GetX())
            {
                intersections.Sort(new Comparison<Point2D>(xAscendingComparer));
            }
            else if (segment.getP1().GetX() > segment.getP2().GetX())
            {
                intersections.Sort(new Comparison<Point2D>(xDescendingComparer));
            }
            else if (segment.getP1().GetY() < segment.getP2().GetY())
            {
                intersections.Sort(new Comparison<Point2D>(yAscendingComparer));
            }
            else
            {
                intersections.Sort(new Comparison<Point2D>(yDescendingComparer));
            }

            return intersections.ToArray();
        }
        private int xAscendingComparer(Point2D p1, Point2D p2)
        {
            return p1.GetX().CompareTo(p2.GetX());
        }
        private int xDescendingComparer(Point2D p1, Point2D p2)
        {
            return p2.GetX().CompareTo(p1.GetX());
        }
        private int yAscendingComparer(Point2D p1, Point2D p2)
        {
            return p1.GetY().CompareTo(p2.GetY());
        }
        private int yDescendingComparer(Point2D p1, Point2D p2)
        {
            return p2.GetY().CompareTo(p1.GetY());
        }

        public int getNumVertices()
        {
            return vertices.Length;
        }

        public static Polyline2D[] ReadFromShapefile(string shapefilePath)
        {            
            // Open the shapefile.
            IntPtr shapefileReference = ShapeLib.SHPOpen(shapefilePath, "rb");
            
            // Get the basic shapefile information.
            double[] boundingMinCoordinates = new double[4];
            double[] boundingMaxCoordinates = new double[4];
            int numShapesInFile = 0;
            ShapeLib.ShapeType shapeType = 0;
            ShapeLib.SHPGetInfo(shapefileReference, ref numShapesInFile, ref shapeType, boundingMinCoordinates, boundingMaxCoordinates);

            // Make the array for the shapes.
            Polyline2D[] polylines = new Polyline2D[numShapesInFile];

            // Read the shapes and store them in the array.
            for (int j = 0; j < numShapesInFile; j++)
            {
                // Get a pointer to the shape and copy it to the shape object.
                IntPtr pointerToShape = ShapeLib.SHPReadObject(shapefileReference, j);
                ShapeLib.SHPObject shapeObject = new ShapeLib.SHPObject();
                Marshal.PtrToStructure(pointerToShape, shapeObject);

                // Get the x-coordinates of the vertices.
                int numVertices = shapeObject.nVertices;
                double[] xCoordinates = new double[numVertices];
                Marshal.Copy(shapeObject.padfX, xCoordinates, 0, numVertices);

                // Get the y-coordinates of the vertices.
                double[] yCoordinates = new double[numVertices];
                Marshal.Copy(shapeObject.padfY, yCoordinates, 0, numVertices);

                // Make the vertices.
                Point2D[] vertices = new Point2D[numVertices];
                for (int i = 0; i < numVertices; i++)
                {
                    vertices[i] = new Point2D(xCoordinates[i], yCoordinates[i]);
                }

                // Make the polyline.
                polylines[j] = new Polyline2D(vertices);

                // Attach the attributes to the polyline.
                polylines[j].SetAttribute("id", j);
            }

            // Return the array of shapes.
            return polylines;
        }

        public Polyline2D[] ClipBy(Polygon2D polygon)
        {
            // This method returns zero to many polyline features that contain the pieces of this polyline contained in the specified polygon.

            // If the ranges do not overlap, return an empty array.
            if (!this.Extent.overlaps(polygon.Extent))
            {
                return new Polyline2D[0];
            }

            // Make a list for the clipped lines.
            List<Polyline2D> clippedPolylines = new List<Polyline2D>();

            // Clip all segments.
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                // Make a line segment object.
                Point2D start = vertices[i];
                Point2D end = vertices[i + 1];
                LineSegment2D segment = new LineSegment2D(start, end);

                // Split the segment.
                LineSegment2D[] split = polygon.SplitSegment(segment);

                // The resulting segments may or may not be in the polygon. Only keep those that are in the polygon.
                for (int j = 0; j < split.Length; j++)
                {
                    if (polygon.Contains(split[j].Midpoint()))
                    {
                        clippedPolylines.Add(split[j].ToPolyline());
                    }
                }
            }

            // Reassemble the polylines (at this point, they're just segments) where possible.
            for (int j = 0; j < clippedPolylines.Count; j++)
            {
                // Get the geometry at the current index.
                Polyline2D line1 = clippedPolylines[j];

                // Compare the polyline to the rest of the polylines in the list.
                bool foundConnection = false;
                for (int i = j + 1; i < clippedPolylines.Count && !foundConnection; i++)
                {
                    // Get line 2 and declare the line for the combination of the compared lines.
                    Polyline2D line2 = clippedPolylines[i];
                    Polyline2D combinedPolyline = null;

                    // This is the case when the end of line 1 connects to the beginning of line 2.
                    if (line1.GetVertex(line1.NumVertices - 1).Equals(line2.GetVertex(0)))
                    {
                        // Make the combined polyline.
                        Point2D[] vertices = new Point2D[line1.NumVertices + line2.NumVertices - 1];
                        for (int k = 0; k < line1.NumVertices; k++) {
                            vertices[k] = line1.GetVertex(k);
                        }
                        for (int k = 1; k < line2.NumVertices; k++) {
                            vertices[k + line1.NumVertices - 1] = line2.GetVertex(k);
                        }
                        combinedPolyline = new Polyline2D(vertices);
                    }

                    // This is the case when the end of line 2 connects to the beginning of line 1.
                    else if (line2.GetVertex(line2.NumVertices - 1).Equals(line1.GetVertex(0)))
                    {
                        Point2D[] vertices = new Point2D[line1.NumVertices + line2.NumVertices - 1];
                        for (int k = 0; k < line2.NumVertices; k++) {
                            vertices[k] = line2.GetVertex(k);
                        }
                        for (int k = 1; k < line1.NumVertices; k++) {
                            vertices[k + line2.NumVertices - 1] = line1.GetVertex(k);
                        }
                        combinedPolyline = new Polyline2D(vertices);
                    }

                    // If the geometry was combined, add it to the end of the list, set the flag,
                    // remove the pieces, and step the index back so the next geometry gets checked.
                    if (combinedPolyline != null)
                    {
                        // Remove i first so the position of j doesn't change.
                        clippedPolylines.RemoveAt(i);
                        clippedPolylines.RemoveAt(j);

                        // Add the combined polyline.
                        clippedPolylines.Add(combinedPolyline);

                        // Set the flag to stop iteration.
                        foundConnection = true;

                        // Step the j index back so the polyline that just fell into the j position will be examined.
                        j--;
                    }
                }
            }

            // Return the result.
            return clippedPolylines.ToArray();
        }
        public static void WriteShapefile(string baseFilename, Polyline2D[] polylines, SpatialReference spatialReference)
        {
            // Delete the files associated with the shapefile.
            Shape2D.CleanForShapefile(baseFilename);

            // The shape type is polyline.
            ShapeLib.ShapeType shpType = ShapeLib.ShapeType.PolyLine;

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
            DbfFieldDescriptor[] fieldDescriptors = DbfFieldDescriptor.GetFieldDescriptors(polylines);
            Console.WriteLine("There will be " + fieldDescriptors.Length + " fields in the shapefile.");
            for (int i = 0; i < fieldDescriptors.Length; i++)
            {
                fieldDescriptors[i].AddToDbf(dbfFile);
            }

            // Add the shapes to the file.
            for (int j = 0; j < polylines.Length; j++)
            {
                // Make the coordinate arrays.
                double[] xCoord = polylines[j].getXCoords();
                double[] yCoord = polylines[j].getYCoords();

                // Make and write the shape.
                IntPtr pShp = ShapeLib.SHPCreateObject(shpType, -1, 0, null, null, xCoord.Length, xCoord, yCoord, null, null);
                ShapeLib.SHPWriteObject(shapefile, -1, pShp);

                // Write the shape attributes.
                for (int i = 0; i < fieldDescriptors.Length; i++)
                {
                    string name = fieldDescriptors[i].Name;
                    object value = polylines[j].GetAttributeValue(name);
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
            int n = this.NumVertices;
            double[] xCoords = new double[n];
            for (int i = 0; i < n; i++)
            {
                xCoords[i] = this.vertices[i].X;
            }

            // Return the array.
            return xCoords;
        }
        private double[] getYCoords()
        {
            // Make an array of the y-coordinates of this shape.
            int n = this.NumVertices;
            double[] yCoords = new double[n];
            for (int i = 0; i < n; i++)
            {
                yCoords[i] = this.vertices[i].Y;
            }

            // Return the array.
            return yCoords;
        }

        public Point2D GetClosestPoint(Point2D p)
        {
            double minDistance = double.MaxValue;
            Point2D closestPoint = null;

            for (int i = 0; i < this.NumVertices - 1; i++)
            {
                LineSegment2D segment = new LineSegment2D(this.vertices[i], this.vertices[i + 1]);
                Point2D closestPointOnSegment = segment.getClosestPoint(p);
                double distance = closestPointOnSegment.distance(p);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closestPointOnSegment;
                }
            }

            return closestPoint;
        }
        public double GetLength()
        {
            // Sum the lengths of all segments in the polyline.
            double length = 0.0;
            for (int i = 0; i < this.NumVertices - 1; i++)
            {
                double dx = this.vertices[i].X - this.vertices[i + 1].X;
                double dy = this.vertices[i].Y - this.vertices[i + 1].Y;
                length += Math.Sqrt(dx * dx + dy * dy);
            }

            // Return the result.
            return length;
        }
        public Point2D GetPointAtRatio(double ratio)
        {
            // Get the length of the line.
            double length = this.GetLength();

            // Limit the ratio between zero and one.
            ratio = ratio < 0.0 ? 0.0 : (ratio > 1.0 ? 1.0 : ratio);
            
            // Find the point along the line where the ratio lies.
            double remainingLength = ratio * length;
            for (int i = 0; i < this.NumVertices - 1; i++)
            {
                Point2D v0 = this.vertices[i];
                Point2D v1 = this.vertices[i + 1];

                // If the segment does not contain the desired point, advance.
                double segmentLength = v0.distance(v1);
                if (segmentLength < remainingLength)
                {
                    remainingLength -= segmentLength;
                }
                // Otherwise, find the point along the line.
                else
                {
                    double ratioWithinSegment = remainingLength / segmentLength;
                    double dx = v1.GetX() - v0.GetX();
                    double dy = v1.GetY() - v0.GetY();
                    double x = v0.GetX() + dx * ratioWithinSegment;
                    double y = v0.GetY() + dy * ratioWithinSegment;

                    return new Point2D(x, y);
                }
            }

            return this.vertices[this.vertices.Length - 1];
        }

        public double GetRatioOfClosestPoint(Point2D p)
        {
            double minDistance = double.MaxValue;
            Point2D closestPoint = null;
            double polylineLength = this.GetLength();
            double cumulativeLength = 0.0;
            double ratioOfClosest = 0.0;

            for (int i = 0; i < this.NumVertices - 1; i++)
            {
                LineSegment2D segment = new LineSegment2D(this.vertices[i], this.vertices[i + 1]);
                double segmentLength = segment.getLength();
                Point2D closestPointOnSegment = segment.getClosestPoint(p);
                double distance = closestPointOnSegment.distance(p);
                if (distance < minDistance)
                {
                    minDistance = distance;

                    ratioOfClosest = (cumulativeLength + closestPointOnSegment.distance(this.vertices[i])) / polylineLength;
                }

                cumulativeLength += segmentLength;
            }

            return ratioOfClosest;
        }

        public Polyline2D TrimToRatios(double startRatio, double endRatio)
        {
            // If the case that the ratios describe the entire polyline, return this.
            if (startRatio <= 0.0 && endRatio >= 1.0)
            {
                return this;
            }

            // Otherwise, trim the polyline.
            else
            {
                // Make a list for the vertices of the trimmed polyline.
                List<Point2D> trimmedVertices = new List<Point2D>();

                // Add the appropriate vertices to the list.
                double length = this.GetLength();
                double lengthAtStartOfSegment = 0.0;
                bool done = false;
                for (int i = 0; i < vertices.Length - 1 && !done; i++)
                {
                    double segmentLength = vertices[i].distance(vertices[i + 1]);
                    double lengthAtEndOfSegment = lengthAtStartOfSegment + segmentLength;

                    double ratioAtStartOfSegment = lengthAtStartOfSegment / length;
                    double ratioAtEndOfSegment = lengthAtEndOfSegment / length;

                    // If the list already has a vertex on it, add the starting vertex.
                    if (trimmedVertices.Count > 0)
                    {
                        trimmedVertices.Add(vertices[i]);
                    }

                    // Otherwise, if the starting vertex is at or after the start ration, add the starting vertex.
                    else if (ratioAtStartOfSegment >= startRatio)
                    {
                        trimmedVertices.Add(vertices[i]);
                    }

                    // Otherwise, if the end vertex is after the start ratio, add the appropriate midpoint.
                    else if (ratioAtEndOfSegment > startRatio)
                    {
                        double lengthAtStartOfTrimmedLine = startRatio * length;
                        double lengthWithinSegment = lengthAtStartOfTrimmedLine - lengthAtStartOfSegment;
                        double ratioWithinSegment = lengthWithinSegment / segmentLength;
                        double dx = vertices[i + 1].X - vertices[i].X;
                        double dy = vertices[i + 1].Y - vertices[i].Y;
                        double x = vertices[i].X + dx * ratioWithinSegment;
                        double y = vertices[i].Y + dy * ratioWithinSegment;
                        trimmedVertices.Add(new Point2D(x, y));
                    }

                    // If the end of the segment is at of after the end ratio, add the appropriate point and flag as done.
                    if (ratioAtEndOfSegment >= endRatio)
                    {
                        if (ratioAtEndOfSegment == endRatio)
                        {
                            trimmedVertices.Add(vertices[i + 1]);
                        }
                        else
                        {
                            double lengthAtEndOfTrimmedLine = endRatio * length;
                            double lengthWithinSegment = lengthAtEndOfTrimmedLine - lengthAtStartOfSegment;
                            double ratioWithinSegment = lengthWithinSegment / segmentLength;
                            double dx = vertices[i + 1].X - vertices[i].X;
                            double dy = vertices[i + 1].Y - vertices[i].Y;
                            double x = vertices[i].X + dx * ratioWithinSegment;
                            double y = vertices[i].Y + dy * ratioWithinSegment;
                            trimmedVertices.Add(new Point2D(x, y));
                        }

                        done = true;
                    }

                    // Advance the start marker.
                    lengthAtStartOfSegment = lengthAtEndOfSegment;
                }

                return new Polyline2D(trimmedVertices.ToArray());
            }
        }
    }
}
