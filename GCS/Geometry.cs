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
            if (shape1 is Dot || shape2 is Dot) return new Vector2[] { };
            throw new Exception("뀨;;");

        }

        private static Vector2[] getIntersect(Line line1, Line line2, bool isSegment=true)
        {

            if (line1.Grad == line2.Grad && line1.Yint == line2.Yint) { throw new NotFiniteNumberException("일치"); }

            else if (line1.Grad == line2.Grad) { return new Vector2[] { }; }

            float intersectx = (line2.Yint - line1.Yint) / (line1.Grad - line2.Grad);
            if (!isSegment)
                return new Vector2[] { new Vector2(intersectx, intersectx * line1.Grad + line1.Yint) };

            Vector2 intersect = new Vector2(intersectx, intersectx * line1.Grad + line1.Yint);
            // HACK: 여기 최적화 존나 애매함
            if (Vector2.Distance(intersect, line1.Point1) < Vector2.Distance(line1.Point1, line1.Point2) &&
                Vector2.Distance(intersect, line1.Point2) < Vector2.Distance(line1.Point1, line1.Point2) &&
                Vector2.Distance(intersect, line2.Point1) < Vector2.Distance(line2.Point1, line2.Point2) &&
                Vector2.Distance(intersect, line2.Point2) < Vector2.Distance(line2.Point1, line2.Point2))
            {
                return new Vector2[] { intersect };
            }
            else return new Vector2[] { };
        }

        private static Vector2[] getIntersect(Line line, Circle circle, bool isSegment = true)
        {

            Vector2 d = line.Point2 - line.Point1;
            float dr = d.Length();
            float D = (line.Point1.X - circle.Center.X) * (line.Point2.Y - circle.Center.Y)
                - (line.Point2.X - circle.Center.X) * (line.Point1.Y - circle.Center.Y);
            float discriminant = circle.Radius * circle.Radius * dr * dr - D * D;

            if (discriminant < 0)
                return new Vector2[] { };
            else if (discriminant == 0)
                return new[] { new Vector2(D * d.Y / (dr * dr) + circle.Center.X, -D * d.X / (dr * dr) + circle.Center.Y) };
            else
            {
                float x = D * d.Y / (dr * dr) + circle.Center.X;
                float y = -D * d.X / (dr * dr) + circle.Center.Y;
                float sgnDy = d.Y < 0 ? -1 : 1;
                float xd = sgnDy * d.X * (float)Math.Sqrt(discriminant) / (dr * dr);
                float yd = Math.Abs(d.Y) * (float)Math.Sqrt(discriminant) / (dr * dr);
                Vector2 intersect1 = new Vector2(x + xd, y + yd);
                Vector2 intersect2 = new Vector2(x - xd, y - yd);
                float len = Vector2.Distance(line.Point1, line.Point2);
                if (!isSegment)
                {
                    return new[]
                    {
                        new Vector2(x + xd, y + yd),
                        new Vector2(x - xd, y - yd)
                    };
                }
                if (Vector2.Distance(intersect1, line.Point1) < len && Vector2.Distance(intersect1, line.Point2) < len)
                {
                    if (Vector2.Distance(intersect2, line.Point1) < len && Vector2.Distance(intersect2, line.Point2) < len)
                        return new Vector2[] { intersect1, intersect2 };
                    else return new Vector2[] { intersect1 };
                }
                else if (Vector2.Distance(intersect2, line.Point1) < len && Vector2.Distance(intersect2, line.Point2) < len)
                    return new Vector2[] { intersect2 };
                else return new Vector2[] { };
            }
        }

        private static Vector2[] getIntersect(Circle circle1, Circle circle2)
        {
            float distance = (float)Math.Sqrt(Math.Pow(circle2.Center.X - circle1.Center.X, 2) + Math.Pow(circle2.Center.Y - circle1.Center.Y, 2));

            if (distance == 0) { throw new NotFiniteNumberException("일치"); }
            else if (distance > circle1.Radius + circle2.Radius) //두 원이 밖에 있으면서 만나지 않음.
            {
                return new Vector2[] { };

            }
            else if ((float)Math.Abs(circle1.Radius - circle2.Radius) > distance) // 두 원 중 하나가 서로를 포함.
            {
                return new Vector2[] { };
            }
            else
            {
                float d = (circle1.Center - circle2.Center).Length();
                float r1 = circle1.Radius;
                float r2 = circle2.Radius;

                float x = (d * d + r1 * r1 - r2 * r2) / (2 * d);

                Vector2 p1 = circle1.Center + (circle2.Center - circle1.Center) * x / d;
                Vector2 v1 = circle1.Center - circle2.Center;
                v1 = new Vector2(-v1.Y, v1.X);
                Vector2 p2 = p1 + v1;
                return getIntersect(new Line(p1, p2), circle1, false);
            }

        }
    }
}