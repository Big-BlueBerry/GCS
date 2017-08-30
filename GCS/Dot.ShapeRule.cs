using System;
using Microsoft.Xna.Framework;

namespace GCS
{
    public partial class Dot
    {
        public class EmptyDotRule : ShapeRule
        {
            public EmptyDotRule(Dot dot) : base(dot) { }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                MoveChilds();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                throw new NotSupportedException("부모도 없는게..");
            }

            protected override void Fix()
            {
                // Do nothing. Just like you.
            }
        }

        public class DotOnShapeRule : ShapeRule
        {
            /// <summary>
            /// 선이면 비율, 원이면 각
            /// </summary>
            private float _ratio;

            public DotOnShapeRule(Dot dot, Shape parent) : base(dot)
            {
                dot.Parents.Add(parent);
                parent.Childs.Add(dot);

                RecalcRatio();
                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                RecalcRatio();
                Fix();
                MoveChilds();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                if (IsHandling) return;
                Fix();
                MoveChilds();
            }

            protected void RecalcRatio()
            {
                var dot = Shape as Dot;
                var parent = Shape.Parents[0];
                //타원이라고 다를 건 없음. 다만 로테이션을 신경써주면 될 뿐
                if (parent is Circle )
                {
                    var circle = parent as Circle;
                    _ratio = (float)Math.Atan2(circle.Center.Y - dot.Coord.Y,
                                               -dot.Coord.X + circle.Center.X) + (float)Math.PI;
                    //tan(theta + pi) = tan(theta);
                }
                else if (parent is Ellipse)
                {
                    Ellipse ellipse = parent as Ellipse;
                    Vector2 diff = ellipse.Focus1 - ellipse.Focus2;
                    float angle = (float)Math.Atan2(diff.Y, diff.X);
                    Ellipse elp = Ellipse.FromThreeDots(Dot.FromCoord(Geometry.Rotate(ellipse.Focus1, -angle)),
                        Dot.FromCoord(Geometry.Rotate(ellipse.Focus2, -angle)),
                        Dot.FromCoord(Geometry.Rotate(ellipse.PinPoint, -angle)));
                    Vector2 dotc = Geometry.Rotate(dot.Coord, -angle);

                    _ratio = (float)Math.Atan2(dotc.Y - elp.Center.Y, -dotc.X + elp.Center.X) + (float)Math.PI;
                }
                else if (parent is LineLike)
                {
                    var line = parent as LineLike;
                    _ratio = (dot.Coord.X - line.Point1.X) / (line.Point2.X - line.Point1.X);
                }
            }

            protected override void Fix()
            {
                var dot = Shape as Dot;
                var parent = Shape.Parents[0];

                if (parent is Circle)
                {
                    var circle = parent as Circle;
                    dot.Coord = new Vector2(circle.Center.X + (float)Math.Cos(_ratio) * circle.Radius,
                                            circle.Center.Y + (float)Math.Sin(_ratio) * circle.Radius);
                }
                else if(parent is Ellipse)
                {
                    var ellipse = parent as Ellipse;
                    
                    Vector2 diff = ellipse.Focus1 - ellipse.Focus2;
                    float angle = (float)Math.Atan2(diff.Y, diff.X);
                    Ellipse elp = Ellipse.FromThreeDots(Dot.FromCoord(Geometry.Rotate(ellipse.Focus1, -angle)),
                        Dot.FromCoord(Geometry.Rotate(ellipse.Focus2, -angle)),
                        Dot.FromCoord(Geometry.Rotate(ellipse.PinPoint, -angle)));
                        
                    float grad = (float)Math.Tan(_ratio);
                    Line lin = Line.FromTwoPoints(elp.Center, elp.Center + new Vector2(1, grad));
                    Vector2 [] stdpoints = Geometry.GetIntersect(elp, lin);
                    
                    float dist = Vector2.Distance(stdpoints[0] , elp.Center);
                    dot.Coord = Geometry.Rotate(new Vector2(elp.Center.X + (float)Math.Cos(-_ratio ) * dist,
                                            elp.Center.Y + (float)Math.Sin(-_ratio ) * dist), angle);//어케 수정할지 1도 모르겠다
                    if (true) { };
                }
                else if (parent is LineLike)
                {
                    var line = parent as LineLike;
                    var p1 = line.Point1;
                    var p2 = line.Point2;

                    dot.Coord = new Vector2(p2.X * _ratio + p1.X * (1 - _ratio),
                                            p2.Y * _ratio + p1.Y * (1 - _ratio));
                }

                dot.Coord = Geometry.GetNearest(parent, dot.Coord);
            }
        }

        public class DotOnIntersectionRule : ShapeRule
        {
            /// <summary>
            /// 교점이 두개일 경우에 대해 방향 결정
            /// </summary>
            private int _orientation;
            private int _firstDirection;

            public DotOnIntersectionRule(Dot dot, Shape p1, Shape p2) : base(dot)
            {
                dot.Parents.Add(p1);
                dot.Parents.Add(p2);
                p1.Childs.Add(dot);
                p2.Childs.Add(dot);

                Fix();
            }

            public override void OnMoved()
            {
                /*
                if (IsHandling) return;
                IsHandling = true;
                
                Fix();

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();

                IsHandling = false;
                */

                Fix();
            }

            public override void OnParentMoved()
            {
                if (IsHandling) return;
                Fix();

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();
            }

            protected override void Fix()
            {
                var dot = Shape as Dot;
                var p1 = Shape.Parents[0];
                var p2 = Shape.Parents[1];
                var vs = Geometry.GetIntersect(p1, p2);

                if(_orientation == 0)
                {
                    if (p1 is Circle && p2 is Circle)
                    {
                        var c1 = p1 as Circle;
                        var c2 = p2 as Circle;
                        if (_firstDirection == 0) _firstDirection = c2.Center.X < c1.Center.X ? -1 : 1;
                        _orientation = CheckOrient(dot.Coord, Line.FromTwoPoints(c1.Center, c2.Center));
                    }
                    else if (p1 is Circle || p2 is Circle)
                    {
                        var c = p1 is Circle ? p1 as Circle : p2 as Circle;
                        var s = p1 is Circle ? p2 : p1;
                        if (_firstDirection == 0)
                        {
                            _firstDirection = (s as LineLike).Point1.Y < (s as LineLike).Point2.Y ? -1 : 1;
                            _orientation = CheckOrient(dot.Coord, Line.FromTwoPoints(c.Center, Geometry.GetNearest(s, c.Center)));
                        }
                    }
                }

                if (vs.Length == 0)
                {
                    dot.Disabled = true;
                    return;
                }
                else
                    dot.Disabled = false;
                
                if (vs.Length == 1)
                {
                    dot.Coord = vs[0];
                    return;
                }
                else
                {
                    Vector2 result = Vector2.Zero;
                    int orient = -1;

                    var c = p1 is Circle ? p1 as Circle : p2 as Circle;
                    var s = p1 is Circle ? p2 : p1;
                    int i1 = (_firstDirection + 1) / 2;
                    int i2 = Math.Abs(i1 - 1);

                    if (s is LineLike)
                    {
                        var l = s as LineLike;
                        orient = CheckOrient(vs[0], Line.FromTwoPoints(c.Center, Geometry.GetNearest(s, c.Center)));

                        if (l.Point1.Y < l.Point2.Y)
                            result = orient == _orientation ? vs[i1] : vs[i2];
                        else result = orient == _orientation ? vs[i2] : vs[i1];
                    }
                    else if (s is Circle)
                    {
                        orient = CheckOrient(vs[0], Line.FromTwoPoints(c.Center, (s as Circle).Center));

                        if((p2 as Circle).Center.X < (p1 as Circle).Center.X)
                            result = orient == _orientation ? vs[i1] : vs[i2];
                        else result = orient == _orientation ? vs[i2] : vs[i1];
                    }

                    dot.Coord = result;
                    return;
                }
            }

            private static int CheckOrient(Vector2 d, Line l)
            {
                if (l.Grad * d.X + l.Yint > d.Y) return -1;
                else return 1;
            }
        }

        public class DotOnDotRule : ShapeRule
        {
            public DotOnDotRule(Dot dot, Dot parent) : base(dot)
            {
                dot.Parents.Add(parent);
                parent.Childs.Add(dot);
                Fix();
                MoveChilds();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var dot = Shape.Parents[0] as Dot;
                dot.MoveTo((Shape as Dot).Coord);
                
                Fix();
                MoveChilds();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                if (IsHandling) return;
                Fix();
                MoveChilds();
            }

            protected override void Fix()
            {
                (Shape as Dot).Coord = (Shape.Parents[0] as Dot).Coord;
            }
        }
    }
}
