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
        Segment v2;
        Dot d;
        public GeometryTestComponent() { OnCamera = false; }
        public override void Start()
        {
            base.Start();

            v2 = new Segment(new Dot(0, 0), new Dot(200, 100));
            d = new Dot(Geometry.GetNearest(v2, new Vector2(100, 100)));
            var rule = new GCS.Rules.SegmentRule(d, v2);
        }
        public override void Update()
        {
            base.Update();
            v2.Point2.MoveTo(Mouse.GetState().Position.ToVector2());
        }
 
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Begin();
            v2.Draw(sb);

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
