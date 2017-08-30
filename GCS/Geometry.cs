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
        // 주의) Ellipse에 대해서는 항상 GetIntersect 로 호출
        public static Vector2[] GetIntersect(Shape shape1, Shape shape2)
        {
            if (shape2 is Ellipse) return GetIntersect(shape2, shape1);

            if (shape1 is Segment)
            {
                if (shape2 is Segment)
                    return GetIntersect(shape1 as Segment, shape2 as Segment);
                else if (shape2 is Line)
                    return GetIntersect(shape2 as Line, shape1 as Segment);
                else if (shape2 is Circle)
                    return GetIntersect(shape1 as Segment, shape2 as Circle);
            }
            else if (shape1 is Line)
            {
                if (shape2 is Segment)
                    return GetIntersect(shape1 as Line, shape2 as Segment);
                else if (shape2 is Line)
                    return GetIntersect(shape1 as Line, shape2 as Line);
                else if (shape2 is Circle)
                    return GetIntersect(shape1 as Line, shape2 as Circle);
            }
            else if (shape1 is Circle)
            {
                if (shape2 is Segment)
                    return GetIntersect(shape2 as Segment, shape1 as Circle);
                else if (shape2 is Line)
                    return GetIntersect(shape2 as Line, shape1 as Circle);
                else if (shape2 is Circle)
                    return GetIntersect(shape1 as Circle, shape2 as Circle);
            }
            else if (shape1 is Ellipse)
            {
                Vector2[] result;

                Ellipse elp = shape1 as Ellipse;
                Vector2 diff = elp.Focus1 - elp.Focus2;

                float angle = (float)Math.Atan2(diff.Y, diff.X);
                Vector2 cent = Rotate(elp.Center, -angle);
                Ellipse ellipse = Ellipse.FromThreeDots(Dot.FromCoord(Rotate(elp.Focus1, -angle) - cent),
                    Dot.FromCoord(Rotate(elp.Focus2, -angle) - Rotate(elp.Center, -angle)), Dot.FromCoord(Rotate(elp.PinPoint, -angle) - cent));

                if (shape2 is Segment)
                {
                    Segment seg = shape2 as Segment;
                    Segment segment = Segment.FromTwoDots(Dot.FromCoord(Rotate(seg.Point1, -angle) - cent),
                        Dot.FromCoord(Rotate(seg.Point2, -angle) - Rotate(elp.Center, -angle)));
                    result = GetIntersect(ellipse, segment);
                }
                else if(shape2 is Line)
                {
                    Line lin = shape2 as Line;
                    Line line = Line.FromTwoDots(Dot.FromCoord(Rotate(lin.Point1, -angle) - cent), 
                        Dot.FromCoord(Rotate(lin.Point2, -angle) - cent));
                    result = GetIntersect(ellipse, line);
                }
                else if (shape2 is Circle)
                {
                    Circle cir = shape2 as Circle;
                    Circle circle = Circle.FromTwoDots(Dot.FromCoord(Rotate(cir.Center, -angle) - cent),
                        Dot.FromCoord(Rotate(cir.Another, -angle) - cent));
                    result = GetIntersect(ellipse, circle);
                }
                else if (shape2 is Ellipse)
                {
                    Ellipse elps = shape2 as Ellipse;
                    Ellipse ellipse2 = Ellipse.FromThreeDots(Dot.FromCoord(Rotate(elps.Focus1, -angle) - cent),
                        Dot.FromCoord(Rotate(elps.Focus2, -angle) - cent),
                        Dot.FromCoord(Rotate(elps.PinPoint, -angle) - cent));
                    result = getIntersect(ellipse, ellipse2);
                }
                else result = new Vector2[] { };

                for (int i = 0; i< result.Length; i++)
                    result[i] = Rotate(result[i] + cent, angle);

                return result;
            }

            if (shape1 is Dot || shape2 is Dot) return new Vector2[] { };

            throw new ArgumentException("뀨;;");
        }

        private static Vector2[] GetIntersect(Segment line1, Segment line2)
        {
            if (line1.Grad == line2.Grad)
            {
                if (line1.Yint == line2.Yint)
                    throw new NotFiniteNumberException("일치");

                return new Vector2[] { };
            }

            float intersectx = (line2.Yint - line1.Yint) / (line1.Grad - line2.Grad);

            Vector2 intersect = new Vector2(intersectx, intersectx * line1.Grad + line1.Yint);

            if (Vector2.Distance(intersect, line1.Point1) < Vector2.Distance(line1.Point1, line1.Point2) &&
                Vector2.Distance(intersect, line1.Point2) < Vector2.Distance(line1.Point1, line1.Point2) &&
                Vector2.Distance(intersect, line2.Point1) < Vector2.Distance(line2.Point1, line2.Point2) &&
                Vector2.Distance(intersect, line2.Point2) < Vector2.Distance(line2.Point1, line2.Point2))
            {
                return new Vector2[] { intersect };
            }

            return new Vector2[] { };
        }

        private static Vector2[] GetIntersect(Line line1, Segment line2)
        {
             if (line1.Grad == line2.Grad)
            {
                if (line1.Yint == line2.Yint)
                    throw new NotFiniteNumberException("일치");

                return new Vector2[] { };
            }

            float intersectx = (line2.Yint - line1.Yint) / (line1.Grad - line2.Grad);

            Vector2 intersect = new Vector2(intersectx, intersectx * line1.Grad + line1.Yint);

            if (Vector2.Distance(intersect, line2.Point1) < Vector2.Distance(line2.Point1, line2.Point2) &&
                Vector2.Distance(intersect, line2.Point2) < Vector2.Distance(line2.Point1, line2.Point2))
            {
                return new Vector2[] { intersect };
            }

            return new Vector2[] { };
        }

        private static Vector2[] GetIntersect(Line line1, Line line2)
        {
            if (line1.Grad == line2.Grad)
            {
                if (line1.Yint == line2.Yint)
                    throw new NotFiniteNumberException("일치");

                return new Vector2[] { };
            }

            float intersectx = (line2.Yint - line1.Yint) / (line1.Grad - line2.Grad);

            return new Vector2[] { new Vector2(intersectx, intersectx * line1.Grad + line1.Yint) };
        }

        private static Vector2[] GetIntersect(Line line, Circle circle)
        {
            Vector2 d = line.Point2 - line.Point1;
            float dr = d.Length();
            float D = (line.Point1.X - circle.Center.X) * (line.Point2.Y - circle.Center.Y)
                - (line.Point2.X - circle.Center.X) * (line.Point1.Y - circle.Center.Y);
            float discriminant = circle.Radius * circle.Radius * dr * dr - D * D;

            if (discriminant < 0)
                return new Vector2[] { };
            else if (discriminant == 0)
                return new Vector2[] { new Vector2(D * d.Y / (dr * dr) + circle.Center.X, -D * d.X / (dr * dr) + circle.Center.Y) };
            else
            {
                float x = D * d.Y / (dr * dr) + circle.Center.X;
                float y = -D * d.X / (dr * dr) + circle.Center.Y;
                float sgnDy = d.Y < 0 ? -1 : 1;
                float xd = sgnDy * d.X * (float)Math.Sqrt(discriminant) / (dr * dr);
                float yd = Math.Abs(d.Y) * (float)Math.Sqrt(discriminant) / (dr * dr);

                return new Vector2[]
                {
                    new Vector2(x + xd, y + yd),
                    new Vector2(x - xd, y - yd)
                };
            }
        }

        private static Vector2[] GetIntersect(Segment line, Circle circle)
        {
            Line lin = Line.FromTwoDots(Dot.FromCoord(line.Point1), Dot.FromCoord(line.Point2));
            Vector2[] intersects = GetIntersect(lin, circle);
            float len = Vector2.Distance(line.Point1, line.Point2);

            if (intersects.Length == 0)
                return new Vector2[] { };
            else if (intersects.Length == 1)
            {
                if (Vector2.Distance(intersects[0], line.Point1) < len && Vector2.Distance(intersects[0], line.Point2) < len)
                    return intersects;
                else
                    return new Vector2[] { };
            }
            else
            {
                if (Vector2.Distance(intersects[0], line.Point1) < len && Vector2.Distance(intersects[0], line.Point2) < len)
                {
                    if (Vector2.Distance(intersects[1], line.Point1) < len && Vector2.Distance(intersects[1], line.Point2) < len)
                        return intersects;
                    else
                        return new Vector2[] { intersects[0] };
                }
                else if (Vector2.Distance(intersects[1], line.Point1) < len && Vector2.Distance(intersects[1], line.Point2) < len)
                    return new Vector2[] { intersects[1] };
                else
                    return new Vector2[] { };
            }
            
        }

        private static Vector2[] GetIntersect(Circle circle1, Circle circle2)
        {
            float distance = (float)Math.Sqrt(Math.Pow(circle2.Center.X - circle1.Center.X, 2) + Math.Pow(circle2.Center.Y - circle1.Center.Y, 2));

            if (distance == 0)
                return new Vector2[] { };
            else if (distance > circle1.Radius + circle2.Radius) // 두 원이 밖에 있으면서 만나지 않음.
                return new Vector2[] { };
            else if ((float)Math.Abs(circle1.Radius - circle2.Radius) > distance) // 두 원 중 하나가 서로를 포함.
                return new Vector2[] { };
            else
            {
                float d = (circle1.Center - circle2.Center).Length();
                float r1 = circle1.Radius;
                float r2 = circle2.Radius;

                float x = (d * d + r1 * r1 - r2 * r2) / (2 * d);

                Vector2 v1 = circle1.Center - circle2.Center;
                v1 = new Vector2(-v1.Y, v1.X);

                Vector2 p1 = circle1.Center + (circle2.Center - circle1.Center) * x / d;
                Vector2 p2 = p1 + v1;

                return GetIntersect(Line.FromTwoPoints(p1, p2), circle1);
            }
        }

        // 타원 초점의 x좌표가 서로 같을때 예외를 발생해 줘야 함. 근데 귀차늠 ^^
        private static Vector2[] GetIntersect(Ellipse ellipse1, Line line1)
        {   
            float a = ellipse1.Semimajor;
            float b = ellipse1.Semiminor;
            float c = line1.Grad;
            float d = line1.Yint;

            float [] xvec = MathMatics.Solve2Eq(a * a * c * c + b * b, 2 * a * a * c * d, a * a * (d * d - b * b));

            if (xvec.Length == 0) return new Vector2[] { };
            if (xvec[0] == xvec[1]) return new Vector2[] { new Vector2(xvec[0], c * xvec[0] + d) };
            else return new Vector2[]
            {
                    new Vector2(xvec[0] , c * xvec[0] + d ),
                    new Vector2(xvec[1] , c * xvec[1] + d)
             };
        }

        private static Vector2[] GetIntersect(Ellipse ellipse1, Segment segment1)
        {
            Line lin = Line.FromTwoDots(Dot.FromCoord(segment1.Point1), Dot.FromCoord(segment1.Point2));
            Vector2[] intersects = GetIntersect(ellipse1, lin);
            float len = Vector2.Distance(segment1.Point1, segment1.Point2);
            if (intersects.Length == 0) return new Vector2[] { };
            else if (intersects.Length == 1)
            {
                if (Vector2.Distance(intersects[0], segment1.Point1) < len && Vector2.Distance(intersects[0], segment1.Point2) < len)
                    return intersects;
                else
                    return new Vector2[] { };
            }
            else
            {
                if (Vector2.Distance(intersects[0], segment1.Point1) < len && Vector2.Distance(intersects[0], segment1.Point2) < len)
                {
                    if (Vector2.Distance(intersects[1], segment1.Point1) < len && Vector2.Distance(intersects[1], segment1.Point2) < len)
                        return intersects;
                    else
                        return new Vector2[] { intersects[0] };
                }
                else if (Vector2.Distance(intersects[1], segment1.Point1) < len && Vector2.Distance(intersects[1], segment1.Point2) < len)
                    return new Vector2[] { intersects[1] };
                else
                    return new Vector2[] { };
            }
        }

        private static Vector2[] GetIntersect(Ellipse ellipse1, Circle circle1)
        {
            throw new WorkWoorimException("일단 여긴 놔두쟈");
        }

        private static Vector2[] getIntersect(Ellipse ellipse1, Ellipse ellipse2)
        {
            throw new WorkWoorimException("여긴 더 X같고");
        }
        
        public static Vector2 GetNearest(Shape shape, Vector2 point)
        {
            if (shape is Segment) return GetNearest(shape as Segment, point);
            if (shape is Line) return GetNearest(shape as Line, point);
            if (shape is Circle) return GetNearest(shape as Circle, point);
            if (shape is Ellipse) return GetNearest(shape as Ellipse, point);
            if (shape is Dot) return (shape as Dot).Coord;

            throw new ArgumentException("뀨우;");
        }

        private static Vector2 GetNearest(Line line, Vector2 point)
        {
            Vector2 temppoint = point - new Vector2(0, line.Yint);
            Line templine = Line.FromTwoPoints(line.Point1 - new Vector2(0, line.Yint), line.Point2 - new Vector2(0, line.Yint));
            Line orthogonal = Line.FromTwoPoints(temppoint, new Vector2(temppoint.X + templine.Grad * 10, temppoint.Y - 10));
            Vector2 result = GetIntersect(templine, orthogonal)[0] + new Vector2(0, line.Yint);

            return result;
        }

        private static Vector2 GetNearest(Segment line, Vector2 point)
        {
            Vector2 temppoint = point - new Vector2(0, line.Yint);
            Line templine = Line.FromTwoPoints(line.Point1 - new Vector2(0, line.Yint), line.Point2 - new Vector2(0, line.Yint));
            Line orthogonal = Line.FromTwoPoints(temppoint, new Vector2(temppoint.X + templine.Grad * 10, temppoint.Y - 10));
            Vector2 result = GetIntersect(templine, orthogonal)[0] + new Vector2(0, line.Yint);

            return Vector2.Clamp(result, Vector2.Min(line.Point1, line.Point2), Vector2.Max(line.Point1, line.Point2));
        }

        private static Vector2 GetNearest(Circle circle, Vector2 point)
        {
            if (circle.Center == point) return circle.Center;

            Vector2[] res = GetIntersect(Line.FromTwoPoints(circle.Center, point), circle);

            return (Vector2.Distance(res[0], point) < Vector2.Distance(res[1], point))
                ? res[0]
                : res[1];
        }

        private static Vector2 GetNearest(Ellipse ellipse, Vector2 point)
        {
            Line lin1 = Line.FromTwoDots(Dot.FromCoord(ellipse.Focus1), Dot.FromCoord(point));
            Line lin2 = Line.FromTwoDots(Dot.FromCoord(ellipse.Focus2), Dot.FromCoord(point));

            Vector2[] intersects1 = GetIntersect(ellipse, (Shape)lin1);
            Vector2[] intersects2 = GetIntersect(ellipse, (Shape)lin2);

            try
            {
                // OnLiU: An error is generated when trying to create an ellipse => go to exception "what's this?"
                Vector2 intersect1 = Vector2.Distance(intersects1[0], point) < Vector2.Distance(intersects1[1], point) ? intersects1[0] : intersects1[1];
                Vector2 intersect2 = Vector2.Distance(intersects2[0], point) < Vector2.Distance(intersects2[1], point) ? intersects2[0] : intersects2[1];

                Line seg1 = Line.FromTwoDots(Dot.FromCoord(ellipse.Focus1), Dot.FromCoord(intersect2));
                Line seg2 = Line.FromTwoDots(Dot.FromCoord(ellipse.Focus2), Dot.FromCoord(intersect1));

                try
                {
                    Vector2[] intersects3 = GetIntersect(ellipse, (Shape)Line.FromTwoDots(Dot.FromCoord(GetIntersect(seg1, seg2)[0]), Dot.FromCoord(point)));
                    Vector2 near = Vector2.Distance(intersects3[0], point) < Vector2.Distance(intersects3[1], point) ? intersects3[0] : intersects3[1];

                    return near;
                }
                catch (NotFiniteNumberException)
                {
                    Vector2[] intsc = GetIntersect(ellipse, (Shape)seg1);

                    return Vector2.Distance(intsc[0], point) < Vector2.Distance(intsc[1], point) ? intsc[0] : intsc[1];
                }
            }
            catch
            {
                throw new Exception("what's this?");
            }

            throw new WorkWoorimException("엄밀한 증명 없이 쓴 알고리즘이지만 내가 증명하면 되니까 ㄱㅊ.");
        }

        public static float GetNearestDistance(Shape shape, Vector2 point)
        {
            if (shape is Line)
            {
                Line line = (Line)shape;

                return (float)(Math.Abs(line.Grad * point.X - point.Y + line.Yint) / Math.Sqrt(line.Grad * line.Grad + 1));
            }
            else if (shape is Segment)
            {
                Segment line = (Segment)shape;

                return Vector2.Distance(point, GetNearest(line, point));
            }
            else if (shape is Circle) return Math.Abs(Vector2.Distance((shape as Circle).Center, point) - ((shape as Circle).Radius));
            else if (shape is Ellipse) return Vector2.Distance(GetNearest(shape as Ellipse , point), point);
            else if (shape is Dot) return Vector2.Distance(point, (shape as Dot).Coord);

            throw new ArgumentException("뀨우;;;");
        }

        public static Vector2 Rotate(Vector2 vec, float angle)
        {
            return new Vector2((float)(vec.X * Math.Cos(angle) - vec.Y * Math.Sin(angle)), (float)(vec.X * Math.Sin(angle) + vec.Y * Math.Cos(angle)));
        }
    }
}
