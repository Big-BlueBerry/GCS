using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class FollowRule : IParentRule
    {
        private Dot _parent;
        public Dot Dot { get; private set; }
        public event Action<Vector2> MoveTo;

        bool _parentMoved = false;

        public FollowRule(Dot dot, Dot parent)
        {
            parent.Moved += Parent_Moved;
            Dot = dot;
            Dot.Rule = this;
            dot.Moved += Dot_Moved;
            _parent = parent;
        }

        private void Dot_Moved()
        {
            if (_parentMoved) return;
        }

        private void Parent_Moved()
        {
            _parentMoved = true;
            Dot.SetCoordForce(_parent.Coord);
            MoveTo?.Invoke(_parent.Coord);
            _parentMoved = false;
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return _parent.Coord;
        }

        public bool IsParent(Shape shape)
            => _parent == shape;

        public void Dispose()
        {
            _parent.Moved -= Parent_Moved;
            Dot.Moved -= Dot_Moved;
            _parent = null;
            Dot = null;
            MoveTo = null;
        }
    }
}
