using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public  class LineRule : IParentRule
    {
        public Line Parent;
        public Dot Dot { get; }
        public event Action<Vector2> MoveTo;
        protected float _leftRatio;

        public LineRule(Dot dot, Line parent)
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
            /* 
             * 우림 여기서 점이 라인 밖에 있으면 외분이잖아
             * 외분이면 _leftRatio가 0 미만 혹은 1 초과가 되는데 그래도 잘 작동할까?
             */

            MoveTo?.Invoke(moved);
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            return Geometry.GetNearest(Parent, original);
        }
    }
}
