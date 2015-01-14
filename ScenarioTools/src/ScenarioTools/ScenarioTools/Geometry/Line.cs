﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Geometry
{
    public struct Line
    {
        public float x0;
        public float x1;
        public float y0;
        public float y1;

        public Line(float x0, float y0, float x1, float y1)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
        }
    }
}
