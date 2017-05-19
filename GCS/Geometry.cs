using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GCS
{
    public static class Geometry
    {
        public static Vector2[] GetIntersect((Vector2, Vector2) line1, (Vector2, Vector2) line2)
        {
            throw new NotImplementedException();
        }

        public static Vector2[] GetIntersect((Vector2, float) circle, (Vector2, Vector2) line)
        {
            throw new NotImplementedException();
        }

        public static Vector2[] GetIntersect((Vector2, float) circle1, (Vector2, float) circle2)
        {
            float dist = Vector2.Distance(circle1.Item1, circle2.Item1);
            if (circle1.Item2 + circle2.Item2 < dist)
                return new Vector2[] { };
            else if (circle1.Item2 + circle2.Item2 == dist)
                return new Vector2[] {  };

            throw new NotImplementedException();
        }
    }
}
