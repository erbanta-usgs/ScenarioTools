using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestSpatial
{
    static class Program
    {
        static void Main()
        {
            SpatialReferenceTest spatialReferenceTest = new SpatialReferenceTest();
            spatialReferenceTest.CreateSpatialReference();
            spatialReferenceTest.UseOsrSpatialReference();
        }
    }
}
