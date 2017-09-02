using System;
using Microsoft.Xna.Framework;

namespace GCS
{
    public partial class LineLike
    {
        public class LineLikeOnTwoDotsRule : ShapeRule
        {
            public LineLikeOnTwoDotsRule(LineLike line, Dot d1, Dot d2) : base(line)
            {
                d1.Childs.Add(line);
                d2.Childs.Add(line);
                line.Parents.Add(d1);
                line.Parents.Add(d2);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as LineLike;

                Shape.Parents[0].MoveTo(line.Point1);
                Shape.Parents[1].MoveTo(line.Point2);

                Fix(); // 부모 점이 도형에 의존하여 움직임이 규제된다면 다시 옮겨줘야 함
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
                var line = Shape as LineLike;
                line.Point1 = (line.Parents[0] as Dot).Coord;
                line.Point2 = (line.Parents[1] as Dot).Coord;
            }
        }

        public class ReflectedLineLikeRule : ShapeRule
        {
            public ReflectedLineLikeRule(LineLike axis, LineLike original, LineLike lin ) : base(lin)
            {
                lin.Parents.Add(axis);
                lin.Parents.Add(original);

                axis.Childs.Add(lin);
                original.Childs.Add(lin);

                Fix();
            }
            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as LineLike;

                //Shape.Parents[0].MoveTo(line.Point1);
                //Shape.Parents[1].MoveTo(line.Point2);

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
                LineLike lin = Shape as LineLike;
                LineLike axis = lin.Parents[0] as LineLike;
                LineLike original = lin.Parents[1] as LineLike;

                Vector2 ReflectedPoint1 = Geometry.Reflect(original.Point1, axis);
                Vector2 ReflectedPoint2 = Geometry.Reflect(original.Point2, axis);
                lin.Point1 = ReflectedPoint1;
                lin.Point2 = ReflectedPoint2;

            }

        }


    }

    public partial class Line
    {
        public class ParallelLineRule : ShapeRule
        {
            private Vector2 _last = new Vector2();
            public ParallelLineRule(Line line, LineLike original, Dot on) : base(line)
            {
                original.Childs.Add(line);
                on.Childs.Add(line);
                line.Parents.Add(original);
                line.Parents.Add(on);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as Line;
                var delta = line.Point1 - _last;

                var dot = line.Parents[1] as Dot;
                dot.Move(delta);

                MoveChilds();
                Fix();
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
                var line = Shape as Line;
                var parent = (line.Parents[0] as LineLike);
                var dot = (line.Parents[1] as Dot).Coord;

                var gap = new Vector2(dot.X - (parent.Point1.X + parent.Point2.X) / 2,
                                      dot.Y - (parent.Point1.Y + parent.Point2.Y) / 2);

                line.Point1 = parent.Point1 + gap;
                line.Point2 = parent.Point2 + gap;

                _last = line.Point1;
            }
        }

        public class PerpendicularLineRule : ShapeRule
        {
            private Vector2 _last = new Vector2();
            public PerpendicularLineRule(Line line, LineLike original, Dot on) : base(line)
            {
                original.Childs.Add(line);
                on.Childs.Add(line);
                line.Parents.Add(original);
                line.Parents.Add(on);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as Line;
                var delta = line.Point1 - _last;

                var dot = line.Parents[1] as Dot;
                dot.Move(delta);

                MoveChilds();
                Fix();
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
                var line = Shape as Line;
                var parent = (line.Parents[0] as LineLike);
                var dot = (line.Parents[1] as Dot).Coord;

                var gap = new Vector2(dot.X - (parent.Point1.X + parent.Point2.X) / 2,
                                      dot.Y - (parent.Point1.Y + parent.Point2.Y) / 2);

                var p1 = Geometry.Rotate(parent.Point1 + gap - dot, MathHelper.ToRadians(90)) + dot;
                var p2 = Geometry.Rotate(parent.Point2 + gap - dot, MathHelper.ToRadians(90)) + dot;

                line.Point1 = p1;
                line.Point2 = p2;

                _last = line.Point1;
            }
        }

        public class TangentLineRule : ShapeRule
        {
            private Vector2 _last = new Vector2();
            public TangentLineRule(Line line, Circle original, Dot on) : base(line)
            {
                original.Childs.Add(line);
                on.Childs.Add(line);
                line.Parents.Add(original);
                line.Parents.Add(on);

                Fix();
            }

            public TangentLineRule(Line line, Ellipse original, Dot on) : base(line)
            {
                original.Childs.Add(line);
                on.Childs.Add(line);
                line.Parents.Add(original);
                line.Parents.Add(on);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as Line;
                var delta = line.Point1 - _last;

                var dot = line.Parents[1] as Dot;
                dot.Move(delta);

                MoveChilds();
                Fix();
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
                var line = Shape as Line;
                var dot = (line.Parents[1] as Dot).Coord;
                if(line.Parents[0] is Ellipse)
                {
                    var parent = (line.Parents[0] as Ellipse);
                    Vector2 diff = parent.Focus1 - parent.Focus2;
                    float angle = (float)Math.Atan2(diff.Y, diff.X);
                    var tempcenter = Geometry.Rotate(parent.Center, -angle);
                    var tempdot = Geometry.Rotate(dot, -angle);
                    
                    float tempgrad = (parent.Semiminor * parent.Semiminor * (tempcenter.X - tempdot.X))/
                        (parent.Semimajor * parent.Semimajor * (tempdot.Y - tempcenter.Y ) );
                    float grad = (float)((tempgrad + Math.Tan(angle)) / (1 - tempgrad * Math.Tan(angle)));
                    //tangent 덧셈 정리
                    line.Point1 = dot;
                    line.Point2 = dot + new Vector2(1, grad);

                }
                else
                {
                    var parent = (line.Parents[0] as Circle);
                    float grad = (parent.Center.X - dot.X) / (dot.Y - parent.Center.Y);
                    //float Weight = (float)Math.Sqrt(parent.Radius/(grad*grad+1));
                    line.Point1 = dot;
                    line.Point2 = dot + new Vector2(1, grad); // or (Weight, Weight * grad)
                    //Weight 는 Point1 과 Point2 사이의 거리를 Radius 로 뽑아줄 때 쓰면 됨.
                }

                _last = line.Point1;
            }

        }

    }
}
