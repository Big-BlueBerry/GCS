using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class IntersectRule : IParentRule
    {
        public Shape Parent1;
        public Shape Parent2;
        public Dot Dot { get; }
        public event Action<Vector2> MoveTo;
        private bool _parentMoved = false;


        public IntersectRule(Dot dot, Shape p1, Shape p2)
        {
            Dot = dot;
            Dot.Rule = this;
            Parent1 = p1; Parent2 = p2;
            p1.Moved += Parent_Moved;
            p2.Moved += Parent_Moved;
            dot.Moved += Dot_Moved;
        }

        private void Dot_Moved()
        {
            if (_parentMoved) return;
            //Dot.Coord = FixedCoord(Dot.Coord);
        }

        private void Parent_Moved()
        {
            _parentMoved = true;
            Dot.Coord = FixedCoord(Dot.Coord);
            MoveTo?.Invoke(Dot.Coord);
            _parentMoved = false;
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            _parentMoved = true;
            var vs = Geometry.GetIntersect(Parent1, Parent2);
            if (vs.Length == 0) { Dot.Enabled = false; _parentMoved = false; return Vector2.Zero; }
            else Dot.Enabled = true;
            _parentMoved = false;
            return vs[0]; // 우림아 일해라~~~
        }
    }
}
