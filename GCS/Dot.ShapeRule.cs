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

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();

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

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                if (IsHandling) return;
                Fix();

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();
            }

            protected void RecalcRatio()
            {
                var dot = Shape as Dot;
                var parent = Shape.Parents[0];

                if (parent is Circle)
                {
                    var circle = parent as Circle;
                    _ratio = (float)Math.Atan2(circle.Center.Y - dot.Coord.Y,
                                               -dot.Coord.X + circle.Center.X) + (float)Math.PI;
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
    }
}
