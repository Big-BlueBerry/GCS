using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class CircleRule : IParentRule
    {
        public Circle Parent;
        public Dot Dot { get; private set; }
        public event Action<Vector2> MoveTo;
        protected float _angle;
        private bool _parentMoved = false;

        public CircleRule(Dot dot, Circle parent)
        {
            parent.Moved += Parent_Moved;
            Dot = dot;
            Dot.Rule = this;
            dot.Moved += Dot_Moved;
            Parent = parent;

            _angle = getAngle();
        }

        private float getAngle()
            => (float)Math.Atan2(Parent.Center.Coord.Y - Dot.Coord.Y, -Dot.Coord.X + Parent.Center.Coord.X) + (float)Math.PI;

        private void Dot_Moved()
        {
            if (_parentMoved) return;
            _angle = getAngle();
        }

        private void Parent_Moved()
        {
            _parentMoved = true;
            var p1 = Parent.Center.Coord;
            float rad = Parent.Radius;
            Vector2 moved = Vector2.Zero;
            moved = new Vector2(p1.X + (float)Math.Cos(_angle) * rad, p1.Y + (float)Math.Sin(_angle) * rad);

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
