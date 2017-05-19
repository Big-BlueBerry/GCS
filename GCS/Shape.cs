using System;
using Grid.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GCS
{
    public abstract class Shape
    {
        public abstract void Draw(SpriteBatch sb, float border, Color color);
    }

    public class Circle : Shape
    {
        public static int Sides = 100;
        public Vector2 Center;
        public float Radius;
        
        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override void Draw(SpriteBatch sb, float border, Color color)
        {
            GUI.DrawCircle(sb, Center, Radius, border, color, Sides);
        }
    }

    public class Line : Shape
    {
        public Vector2 Point1;
        public Vector2 Point2;

        public Line(Vector2 p1, Vector2 p2)
        {
            Point1 = p1;
            Point2 = p2;
        }

        public override void Draw(SpriteBatch sb, float border, Color color)
        {
            GUI.DrawLine(sb, Point1, Point2, border, color);
        }
    }

    public class Dot : Shape
    {
        public Vector2 Coord;

        public Dot(Vector2 coord)
        {
            Coord = coord;
        }

        public override void Draw(SpriteBatch sb, float border, Color color)
        {
            GUI.DrawPoint(sb, Coord, border, color);
        }
    }
}
