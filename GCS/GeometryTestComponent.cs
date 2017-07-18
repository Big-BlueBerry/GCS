using System.Linq;
using System.Collections.Generic;
using Grid.Framework;
using Grid.Framework.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GCS
{
    class GeometryTestComponent : Renderable
    {
        Circle v2;
        Vector2 first;
        Dot d;
        public GeometryTestComponent() { OnCamera = false; }
        public override void Start()
        {
            base.Start();

            v2 = new Circle(Dot.FromCoord(500, 500), Dot.FromCoord(100, 200));
            first = Geometry.GetNearest(v2, new Vector2(300, 100));
            d = Dot.FromCoord(first);
            //Circle c = new Circle(Dot.FromCoord(100, 100), Dot.FromCoord(100, 200));

        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Begin();
            v2.Draw(sb);
            GUI.DrawPoint(sb, first, 10f, Color.Red);

            //GUI.DrawPoint(sb, Geometry.GetNearest(v1, v3), 10f, Color.Blue);
            //Debug.WriteLine($"Distance to Circle : {Geometry.GetNearestDistance(v1, v3)}");
            //GUI.DrawPoint(sb, Geometry.GetNearest(v2, v3), 10f, Color.Red);
            d.Draw(sb);
            v2.Draw(sb);
            //Debug.WriteLine($"Distance to Line : {Geometry.GetNearestDistance(v2, v3)}");
            //Debug.WriteLine(Dot)
            sb.End();
        }
    }
}
