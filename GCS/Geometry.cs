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
        public static Vector2[] GetIntersect(Shape shape1, Shape shape2)
        {
            if(shape1 is Line)
            {
                if (shape2 is Line)
                    return getIntersect(shape1 as Line, shape2 as Line);
                else if (shape2 is Circle)
                    return getIntersect(shape1 as Line, shape2 as Circle);
            }
            else if(shape1 is Circle)
            {
                if (shape2 is Line)
                    return getIntersect(shape2 as Line, shape1 as Circle);
                else if (shape2 is Circle)
                    return getIntersect(shape1 as Circle, shape2 as Circle);
            }
            throw new Exception("뀨;;");
        }
        private static Vector2[] getIntersect(Line line1, Line line2)
        {
            throw new NotImplementedException();
        }

        private static Vector2[] getIntersect(Line line, Circle circle)
        {
            throw new NotImplementedException();
        }

        private static Vector2[] getIntersect(Circle circle1, Circle circle2)
        {
            throw new NotImplementedException();
        }
    }
}
