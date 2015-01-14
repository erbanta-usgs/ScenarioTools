using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Spatial
{
    public static class GlobalSpatialStaticVariables
    {
        private static Spatial.SpatialReference _spatialReference = null;

        /// <summary>
        /// Initial assignment stores (copy of) spatial reference.  
        /// Subsequent assignments only check for equality with
        /// initial spatial reference, unless value==null, in which
        /// case the backing field is nullified.
        /// </summary>
        public static Spatial.SpatialReference SpatialReference
        {
            get
            {
                return _spatialReference;
            }
            set
            {
                if (value == null)
                {
                    _spatialReference = null;
                }
                else if (_spatialReference == null)
                {
                    _spatialReference = new ScenarioTools.Spatial.SpatialReference(value.GetWktString(), value.GeoreferencedObject);
                }
                else
                {
                    string errMsg = "";
                    if (!_spatialReference.Matches(value, ref errMsg))
                    {
                        SpatialReferenceMismatchDialog srmd = new SpatialReferenceMismatchDialog(_spatialReference, value, errMsg);
                        srmd.ShowDialog();
                    }
                }
            }
        }
    }
}
