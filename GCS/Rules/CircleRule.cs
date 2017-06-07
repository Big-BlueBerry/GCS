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

        public CircleRule(Dot dot, Circle parent)
        {
            parent.Moved += Parent_Moved;
            Dot = dot;
            Parent = parent;
            _angle = -1; throw new WorkWoorimException("웅림앙 leftratio 구해죠..");
        }

        private void Parent_Moved()
        {
            throw new WorkWoorimException("점이 원의 angle에 맞춰서...해야댐");
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return Geometry.GetNearest(Parent, original);
        }
    }
}
