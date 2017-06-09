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
            
            v2 = new Circle(new Dot(500, 500), new Dot(100, 200));
            first = Geometry.GetNearest(v2, new Vector2(300, 100));
            d = new Dot(first);
            var rule = new GCS.Rules.CircleRule(d, v2);
            //Circle c = new Circle(new Dot(100, 100), new Dot(100, 200));
            
        }
        public override void Update()
        {
            base.Update();
            v2.Another.MoveTo(Mouse.GetState().Position.ToVector2());
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
