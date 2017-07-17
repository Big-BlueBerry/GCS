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

        public ShapeRule(Shape shape)
        {
            Shape = shape;
        }

        public virtual void OnSelfMoved() { }
        public virtual void OnParentMoved() { }
        public virtual void OnChildMoved() { }
    }

    public partial class Line
    {
        public static Shape Parallel(Shape shape, float distance, float angle)
        {
            throw new WorkWoorimException("라인을 리턴함");

            // 라인하르트! 대령했소이다
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

        public override void OnParentMoved()
        {
            base.OnParentMoved();
            throw new WorkWoorimException("자기 자신을 움직여야 함");
        }

        public override void OnSelfMoved()
        {
            base.OnSelfMoved();
        }
    }

    public class DotOnShapeRule : ShapeRule
    {
        public DotOnShapeRule(Dot dot, Shape parent) : base(dot)
        {
            dot.Parents.Clear();
            dot.Parents.Add(parent);
        }
    }

    public partial class Dot
    {
        public static Dot OnShape(Shape shape, Vector2 coord)
        {
            throw new WorkWoorimException("점을 리턴함");
        }
    }
}
