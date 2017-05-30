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
        public override void Draw(SpriteBatch sb)
        {
            //var v1 = new Circle(new Vector2(100, 100), 350);
            var v1 = new Segment(new Vector2(50,50),new Vector2(120,120));
            v1.Draw(sb);
            Vector2 v2 = (Camera.Current as Camera2D).GetRay(Mouse.GetState().Position.ToVector2());

            GUI.DrawPoint(sb, Geometry.GetNearest(v1, v2), 10f, Color.Blue);
        }
    }
}
