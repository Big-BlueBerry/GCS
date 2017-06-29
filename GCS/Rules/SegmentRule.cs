using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class SegmentRule : IParentRule
    {
        public Segment Parent;
        public Dot Dot { get; private set; }
        public event Action<Vector2> MoveTo;
        protected float _leftRatio;

        bool _parentMoved = false;

        public SegmentRule(Dot dot, Segment parent)
        {
            parent.Moved += Parent_Moved;
            Dot = dot;
            dot.Moved += Dot_Moved;
            Dot.Rule = this;
            Parent = parent;
            _leftRatio = (Dot.Coord.X - Parent.Point1.Coord.X) / (Parent.Point2.Coord.X - Parent.Point1.Coord.X);
        }

        private void Dot_Moved()
        {
            if (!_parentMoved)
                _leftRatio = (Dot.Coord.X - Parent.Point1.Coord.X) / (Parent.Point2.Coord.X - Parent.Point1.Coord.X);
        }

        private void Parent_Moved()
        {
            _parentMoved = true;
            var p1 = Parent.Point1.Coord;
            var p2 = Parent.Point2.Coord;
            Vector2 moved = Vector2.Zero;
            
            moved = new Vector2((p2.X * _leftRatio + p1.X * (1 - _leftRatio)),
                                        (p2.Y * _leftRatio + p1.Y * (1 - _leftRatio)));
            MoveTo?.Invoke(moved);
            _parentMoved = false;
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return Geometry.GetNearest(Parent, original);
        }

        public void Dispose()
        {
            Parent = null;
            Dot = null;
            MoveTo = null;
        }

        public bool IsParent(Shape shape)
            => Parent == shape;
    }
}
