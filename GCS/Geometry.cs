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
            if (shape1 is Line)
            {
                if (shape2 is Line)
                    return getIntersect(shape1 as Line, shape2 as Line);
                else if (shape2 is Circle)
                    return getIntersect(shape1 as Line, shape2 as Circle);
            }
            else if (shape1 is Circle)
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

            if (line1.Grad == line2.Grad && line1.Yint == line2.Yint) { throw new NotFiniteNumberException("일치"); }

            else if (line1.Grad == line2.Grad) { return new Vector2[] { }; }

            float intersectx = (line2.Yint - line1.Yint) / (line1.Grad - line2.Grad);
            return new Vector2[] { new Vector2(intersectx, intersectx * line1.Grad + line1.Yint) };
        }

        private static Vector2[] getIntersect(Line line, Circle circle)
        {
            Vector2 d = line.Point2 - line.Point1;
            float dr = d.Length();
            float D = (line.Point1.X - circle.Center.X) * (line.Point2.Y - circle.Center.Y) 
                - (line.Point2.X - circle.Center.X) * (line.Point1.Y - circle.Center.Y);
            float discriminant = circle.Radius * circle.Radius * dr * dr - D * D;

            if (discriminant < 0)
                return new Vector2[] { };
            else if (discriminant == 0)
                return new [] { new Vector2(D * d.Y / (dr * dr) + circle.Center.X, -D * d.X / (dr * dr) + circle.Center.Y) };
            else
            {
                float x = D * d.Y / (dr * dr) + circle.Center.X;
                float y = -D * d.X / (dr * dr) + circle.Center.Y;
                float sgnDy = d.Y < 0 ? -1 : 1;
                float xd = sgnDy * d.X * (float)Math.Sqrt(discriminant) / (dr * dr);
                float yd = Math.Abs(d.Y) * (float)Math.Sqrt(discriminant) / (dr * dr);
                return new[]
                {
                    new Vector2(x + xd, y + yd),
                    new Vector2(x - xd, y - yd)
                };
            }
        }

        private static Vector2[] getIntersect(Circle circle1, Circle circle2)
        {
            float d = (circle1.Center - circle2.Center).Length();
            float r1 = circle1.Radius;
            float r2 = circle2.Radius;

            float x = (d * d + r1 * r1 - r2 * r2) / (2 * d);

            Vector2 p1 = circle1.Center + (circle2.Center - circle1.Center) * x / d;
            Vector2 v1 = circle1.Center - circle2.Center;
            v1 = new Vector2(-v1.Y, v1.X);
            Vector2 p2 = p1 + v1;
            return getIntersect(new Line(p1, p2), circle1);
        }
    }
}