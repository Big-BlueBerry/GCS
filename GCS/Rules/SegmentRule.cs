using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class SegmentRule : IParentRule
    {
        public Segment Parent;
        public Dot Dot { get; }
        public event Action<Vector2> MoveTo;
        protected float _leftRatio;

        public SegmentRule(Dot dot, Segment parent)
        {
            parent.Moved += Parent_Moved;
            Dot = dot;
            Parent = parent;
            _leftRatio = -1; throw new WorkWoorimException("웅림앙 leftratio 구해죠..");
        }

        private void Parent_Moved()
        {
            var p1 = Parent.Point1.Coord;
            var p2 = Parent.Point2.Coord;
            Vector2 moved = Vector2.Zero;
            moved = new Vector2((p1.X * _leftRatio + p2.X * (1 - _leftRatio)) / (p1.X + p2.X),
                                        (p1.Y * _leftRatio + p2.Y * (1 - _leftRatio)) / (p1.Y + p2.Y));

            MoveTo?.Invoke(moved);
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return Geometry.GetNearest(Parent, original);
        }
    }
}
