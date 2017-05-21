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
        public float grad;
        public float yint;
 
        
        public Line(Vector2 p1, Vector2 p2)
        {
            Point1 = p1;
            Point2 = p2;
            grad =(p2-p1).Y/(p2-p1).X;
            yint = p1.Y -grad*p1.X;
        }
        public Line(float grad, Vector2 point)
        {
            Point1 = point;
            Point2 = new Vector2(Point1.X + 1, point.Y + grad);//나머지 한 점은 x좌표가 입력된 점보다 1 오른쪽에 있는 점.
            this.grad = grad;
            yint = point.Y - grad * point.X;
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
