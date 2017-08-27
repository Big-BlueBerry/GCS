using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GCS
{
    public static class MathMatics
    {
        public static float [] solve2Eq(float a, float b, float c)
        {
            float d = b * b - 4 * a * c;
            if (d < 0) return new float[] { };
            else if (d == 0) return new float[] { -b / (2 * a), -b / (2 * a) };
            else return new float[] { (-b + (float)Math.Sqrt(d)) / (2 * a), (-b - (float)Math.Sqrt(d)) / (2 * a) };

        }
        /*
        public static float[] solveNEq(float[] polynorm, int accuracy = 3)
        {
        
        }
        */
    }
}
