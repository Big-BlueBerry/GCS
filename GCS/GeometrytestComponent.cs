using Grid.Framework.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GCS
{
    class GeometrytestComponent : Renderable
    {
        public override void Draw(SpriteBatch sb)
        {
            Ellipse elp = Ellipse.FromThreeDots(Dot.FromCoord(0, 0), Dot.FromCoord(300, 300), Dot.FromCoord(400, 250));
            //Line lin = Line.FromTwoPoints(new Vector2(100,120), new Vector2(500,670));
            Vector2 p = new Vector2(400,100);
            Dot d = Dot.FromCoord(Geometry.GetNearest(elp, p));
            Line lin1 = Line.FromTwoPoints(elp.Focus1, p);
            Line lin2 = Line.FromTwoPoints(elp.Focus2, p);
            Dot d2 = Dot.FromCoord(Geometry.GetIntersect(lin1, elp)[1]);

            elp.Draw(sb);

            Dot.FromCoord(elp.Focus1).Draw(sb);
            Dot.FromCoord(elp.Focus2).Draw(sb);
            Dot.FromCoord(p).Draw(sb);

            lin1.Draw(sb);
            lin2.Draw(sb);
            d.Draw(sb);
            d2.Draw(sb);
        }
    }
}
