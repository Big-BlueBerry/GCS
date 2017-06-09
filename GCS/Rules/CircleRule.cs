using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class CircleRule : IParentRule
    {
        public Circle Parent;
        public Dot Dot { get; }
        public event Action<Vector2> MoveTo;
        protected float _angle;
        protected float _cosangle, _sinangle;

        public CircleRule(Dot dot, Circle parent)
        {
            parent.Moved += Parent_Moved;
            Dot = dot;
            Dot.Rule = this;
            Parent = parent;

            float dx = parent.Center.Coord.X - Dot.Coord.X;
            float dy = parent.Center.Coord.Y - Dot.Coord.Y;
            _angle = (float) Math.Acos(parent.Radius / dx);
            _cosangle = parent.Radius / dx;
            _sinangle = parent.Radius / dy;
        }

        private void Parent_Moved()
        {
            var p1 = Parent.Center.Coord;
            float rad = Parent.Radius;
            Vector2 moved = Vector2.Zero;
            moved = new Vector2(p1.X + _cosangle * rad, p1.Y + _sinangle * rad);

            Grid.Framework.Debug.WriteLine($"angle : {_angle}");
            Grid.Framework.Debug.WriteLine(moved.ToString());

            MoveTo?.Invoke(moved);
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return Geometry.GetNearest(Parent, original);
        }
    }
}
