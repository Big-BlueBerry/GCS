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
            float distance = (float)Math.Sqrt(Math.Pow(circle2.Center.X - circle1.Center.X, 2) + Math.Pow(circle2.Center.Y - circle1.Center.Y, 2));

            if (distance == 0) { throw new NotFiniteNumberException("일치"); }
            else if (distance > circle1.Radius + circle2.Radius)//두 원이 밖에 있으면서 만나지 않음.
            {
                return new Vector2[] { };

            }
            else if ((float)Math.Abs(circle1.Radius - circle2.Radius) > distance)// 두 원 중 하나가 서로를 포함.
            {
                return new Vector2[] { };
            }
            else
            {
                Line Centerline = new Line(circle1.Center, circle2.Center);
                float subgrad = -1 / (Centerline.Grad);

                var cos = cosThm(circle1.Radius, distance, circle2.Radius);
                Vector2 subpoint = divpoint(
                    circle1.Center,
                    circle2.Center,
                    (cos.Item1 * circle1.Radius) / distance,
                    (cos.Item2 * circle2.Radius) / distance
                    );

                throw new NotImplementedException();
                //Line intersectline = new Line(subgrad, subpoint);
                //return getIntersect(intersectline, circle1);
            }
        }

        private static float[] solve2Eq(float a, float b, float c)
        {
            if (a == 0) throw new ArgumentException("a는 0이 아님 ㅅㄱ");
            else
            {
                float discriminant = b * b - 4 * a * c;
                if (discriminant < 0) return new float[] { };
                else if (discriminant == 0) { return new float[] { -b / 2 * a }; }
                else
                {
                    return new float[] { (-b + (float)Math.Sqrt(discriminant)) / 2 * a,
                        (-b - (float)Math.Sqrt(discriminant)) / 2 * a };
                }
            }
        }

        private static (float, float) cosThm(float a, float b, float c)
        {
            return ((float)(Math.Pow(c, 2) - Math.Pow(a, 2) - Math.Pow(b, 2)) / 2 * a * b,
            (float)(Math.Pow(a, 2) - Math.Pow(b, 2) - Math.Pow(a, 2)) / 2 * b * c);
        }

        /// <summary>
        /// p1과 p2를 m:n으로 내분하는 점의 좌표 리턴
        /// </summary>
        private static Vector2 divpoint(Vector2 p1, Vector2 p2, float m, float n)
        {
            return new Vector2((m * p2.X + n * p1.X) / (m + n), (m * p2.Y + n * p1.Y) / (m + n));
        }
    }
}