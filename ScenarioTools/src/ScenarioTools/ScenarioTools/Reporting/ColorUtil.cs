using System.Drawing;

namespace ScenarioTools.Reporting
{
    public class ColorUtil
    {
        public static bool ColorEquals(Color color1, Color color2)
        {
            return color1.R == color2.R && color1.G == color2.G && color1.B == color2.B && color1.A == color2.A;
        }
    }
}
