using System;
using Microsoft.Xna.Framework;

namespace GCS
{
    public partial class Ellipse
    {
		public class EllipseOnThreeDotsRule : ShapeRule
        {
			public EllipseOnThreeDotsRule(Ellipse ellipse, Dot f1, Dot f2, Dot pin) : base(ellipse)
            {
                ellipse.Parents.Add(f1);
                ellipse.Parents.Add(f2);
                ellipse.Parents.Add(pin);
                f1.Childs.Add(ellipse);
                f2.Childs.Add(ellipse);
                pin.Childs.Add(ellipse);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var ellipse = Shape as Ellipse;

                ellipse.Parents[0].MoveTo(ellipse.Focus1);
                ellipse.Parents[1].MoveTo(ellipse.Focus2);
                ellipse.Parents[2].MoveTo(ellipse.PinPoint);

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
                var ellipse = Shape as Ellipse;
                ellipse.Focus1 = (ellipse.Parents[0] as Dot).Coord;
                ellipse.Focus2 = (ellipse.Parents[1] as Dot).Coord;
                ellipse.PinPoint = (ellipse.Parents[2] as Dot).Coord;
            }
        }

        public class ReflectedEllipseRule : ShapeRule
        {
            public ReflectedEllipseRule(LineLike axis, Ellipse original, Ellipse elp) : base(elp)
            {
                elp.Parents.Add(axis);
                elp.Parents.Add(original);

                axis.Childs.Add(elp);
                original.Childs.Add(elp);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var ellipse = Shape as Ellipse;

                //ellipse.Parents[0].MoveTo(ellipse.Focus1);
                //ellipse.Parents[1].MoveTo(ellipse.Focus2);
                //ellipse.Parents[2].MoveTo(ellipse.PinPoint);

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
                Ellipse elp = Shape as Ellipse;
                LineLike axis = Shape.Parents[0] as LineLike;
                Ellipse original = Shape.Parents[1] as Ellipse;

                Vector2 ReflectedFocus1 = Geometry.Reflect(original.Focus1, axis);
                Vector2 ReflectedFocus2 = Geometry.Reflect(original.Focus2, axis);
                Vector2 ReflectedPinPoint = Geometry.Reflect(original.PinPoint, axis);

                elp.Focus1 = ReflectedFocus1;
                elp.Focus2 = ReflectedFocus2;
                elp.PinPoint = ReflectedPinPoint;
            }
        }

    }
}
