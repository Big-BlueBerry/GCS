using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GCS
{
    public abstract class ShapeRule
    {
        public Shape Shape { get; }
        protected bool IsHandling = false;

        public ShapeRule(Shape shape)
        {
            Shape = shape;
        }

        protected abstract void Fix();

        public abstract void OnMoved();
        public abstract void OnParentMoved();
    }

    public partial class Shape
    {
        public static Shape Parallel(Shape shape, float distance, float angle)
        {
            throw new WorkWoorimException("도형을 리턴함");
        }
    }

    public class ParallelRule : ShapeRule
    {
        private float _distance, _angle;

        public ParallelRule(Shape shape, Shape parent, float dist, float angle) : base(shape)
        {
            _distance = dist;
            _angle = angle;

            shape._rule = this;

            shape.Parents.Clear();
            shape.Parents.Add(parent);
        }

        public override void OnMoved()
        {
            throw new NotImplementedException();
        }

        protected override void Fix()
        {
            throw new NotImplementedException();
        }
    }

    public partial class Line
    {
        public class LineOnTwoDotsRule : ShapeRule
        {
            public LineOnTwoDotsRule(Line line, Dot d1, Dot d2) : base(line)
            {
                d1.Childs.Add(line);
                d2.Childs.Add(line);
                line.Parents.Add(d1);
                line.Parents.Add(d2);
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as Line;

                Shape.Parents[0].MoveTo(line.Point1);
                Shape.Parents[1].MoveTo(line.Point2);

                Fix(); // 부모 점이 도형에 의존하여 움직임이 규제된다면 다시 옮겨줘야 함

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                Fix();
                
                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();
            }

            protected override void Fix()
            {
                var line = Shape as Line;
                line.Point1 = (line.Parents[0] as Dot).Coord;
                line.Point2 = (line.Parents[1] as Dot).Coord;
            }
        }
    }

    public partial class Dot
    {
        public class DotOnShapeRule : ShapeRule
        {
            public DotOnShapeRule(Dot dot, Shape parent) : base(dot)
            {
                dot.Parents.Clear();
                dot.Parents.Add(parent);
            }

            public override void OnMoved()
            {
                throw new NotImplementedException();
            }

            protected override void Fix()
            {
                throw new NotImplementedException();
            }
        }
    }
}
