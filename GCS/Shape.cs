using System;
using Grid.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GCS
{
    public abstract class Shape
    {
        public string Name { get; set; }
        public float Border { get; set; } = 2f;
        public Color Color { get; set; } = Color.Black;
        public abstract void Draw(SpriteBatch sb);
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

        public override void Draw(SpriteBatch sb)
        {
            GUI.DrawCircle(sb, Center, Radius, Border, Color, Sides);
        }
    }

    public class Line : Shape
    {
        private Vector2 _p1;
        public Vector2 Point1 { get => _p1; set { _p1 = value; ResetAB(); } }
        private Vector2 _p2;
        public Vector2 Point2 { get => _p2; set { _p2 = value; ResetAB(); } }

        private float _grad;
        public float Grad { get => _grad; set { _grad = value; ResetPoints(); } }
        private float _yint;
        public float Yint { get => _yint; set { _yint = value; ResetPoints(); } }
        
        public Line(Vector2 p1, Vector2 p2)
        {
            _p1 = p1;
            _p2 = p2;
            ResetAB();
        }

        public Line(float grad, float yint)
        {
            _grad = grad;
            _yint = yint;
            ResetPoints();
        }

        public Line(Vector2 p1, float grad)
        {
            _p1 = p1;
            _p2 = new Vector2(Point1.X + 1, Point1.Y + grad);
            _grad = grad;
            _yint = p1.Y - grad * p1.X;
        }

        private void ResetAB()
        {
            _grad = (Point2 - Point1).Y / (Point2 - Point1).X;
            _yint = (Point1.Y) - Grad * Point1.X;
        }

        private void ResetPoints()
        {
            _p1 = new Vector2(0, Yint);
            _p2 = new Vector2(1, Grad + Yint);
        }

        public override void Draw(SpriteBatch sb)
        {
            GUI.DrawLine(sb, Point1, Point2, Border, Color);
        }
    }
    
    public class Segment : Shape
    {
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }
        public float Grad { get => (Point2 - Point1).Y / (Point2 - Point1).X; }
        public float Yint => (Point1.Y) - Grad * Point1.X;
        
        public Segment(Vector2 p1, Vector2 p2)
        {
            Point1 = p1;
            Point2 = p2;
        }

        public override void Draw(SpriteBatch sb)
        {
            GUI.DrawLine(sb, Point1, Point2, Border, Color);
        }

        public Line ToLine()
        {
            return new Line(Grad, Yint);
        }
    }

    public class Dot : Shape
    {
        public Vector2 Coord;

        public Dot(Vector2 coord)
        {
            Coord = coord;
            Color = Color.Red;
        }

        public override bool Equals(object obj)
        {
            var o = obj as Dot;
            return o == null ? false : Coord.Equals(o.Coord);
        }

        public override int GetHashCode()
            => Coord.GetHashCode();

        public override void Draw(SpriteBatch sb)
        {
            GUI.DrawCircle(sb, Coord, 4f, Border, Color, 20);
            // GUI.DrawPoint(sb, Coord, Border, Color);
        }
    }
}
