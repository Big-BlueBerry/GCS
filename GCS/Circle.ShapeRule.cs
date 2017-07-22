using System;

namespace GCS
{
    public partial class Circle
    {
		public class CircleOnTwoDotsRule : ShapeRule
        {
			public CircleOnTwoDotsRule(Circle circle, Dot center, Dot another) : base(circle)
            {
                circle.Parents.Add(center);
                circle.Parents.Add(another);

                center.Childs.Add(circle);
                another.Childs.Add(circle);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var circle = Shape as Circle;

                Shape.Parents[0].MoveTo(circle.Center);
                Shape.Parents[1].MoveTo(circle.Another);

                Fix();
                MoveChilds();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                if (IsHandling) return;
                Fix();
                MoveChilds();
            }

            protected override void Fix()
            {
                var circle = Shape as Circle;
                circle.Center = (circle.Parents[0] as Dot).Coord;
                circle.Another = (circle.Parents[1] as Dot).Coord;
            }
        }
    }
}
