using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Geometry
{
    public class ShapeAttribute
    {
        private string name;
        private object value;

        public ShapeAttribute(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
        public object Value
        {
            get
            {
                return value;
            }
        }
    }
}
