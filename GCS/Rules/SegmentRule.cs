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

        bool parent_Moved = false;

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
            if (!parent_Moved)
                _leftRatio = (Dot.Coord.X - Parent.Point1.Coord.X) / (Parent.Point2.Coord.X - Parent.Point1.Coord.X);
        }

        private void Parent_Moved()
        {
            parent_Moved = true;
            var p1 = Parent.Point1.Coord;
            var p2 = Parent.Point2.Coord;
            Vector2 moved = Vector2.Zero;
            moved =  new Vector2((p2.X * _leftRatio + p1.X * (1 - _leftRatio)),
                                        (p2.Y * _leftRatio + p1.Y * (1 - _leftRatio)));

            Grid.Framework.Debug.WriteLine($"leftratio : {_leftRatio}");
            Grid.Framework.Debug.WriteLine(moved.ToString());

            MoveTo?.Invoke(moved);
            parent_Moved = false;
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return Geometry.GetNearest(Parent, original);
        }
    }
}
