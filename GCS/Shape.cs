using System;
using Grid.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GCS
{
    public abstract partial class Shape
    {
        private static readonly float _nearDistance = 5;

        internal List<Shape> Parents { get; private set; } = new List<Shape>();
        internal List<Shape> Childs { get; private set; } = new List<Shape>();

        internal ShapeRule _rule = null;

        public bool Enabled { get; set; } = true;

        public string Name { get; set; }
        public float Border { get; set; } = 2f;
        public Color Color { get; set; } = Color.Black;
        public bool Focused { get; set; } = false;
        public bool Selected { get; set; } = false;
        public abstract event Action Moved;
        /// <summary>
        /// 마우스를 떼면 Selected가 false가 되어야 하는가?
        /// </summary>
        internal bool UnSelect { get; set; } = false;
        /// <summary>
        /// 커서와의 거리
        /// </summary>
        public float Distance { get; set; } = -1;
        public virtual void Draw(SpriteBatch sb)
        {
            Color = Color.Black;
            if (Focused) Color = Color.Orange;
            if (Selected) Color = Color.Cyan;
        }
        public abstract void Move(Vector2 add);

        public Shape()
        {

        }

        public virtual bool IsEnoughClose(Vector2 coord)
            => Distance <= _nearDistance;

        public virtual void Update(Vector2 cursor)
        {
            Distance = Geometry.GetNearestDistance(this, cursor);
            if (IsEnoughClose(cursor))
                Focused = true;
            else if (Focused)
                Focused = false;
        }

        public void Delete()
        {
            //_manager.DeleteShapes(Childs);
            foreach (var child in Childs)
                child.Delete();
        }
        
        public virtual void MoveTo(Vector2 delta)
        {
            _rule?.OnSelfMoved();
        }
    }

    public interface ICircle
    {
        Vector2 Center { get; set; }
        float Radius { get; set; }
    }

    public class Circle : Shape, ICircle
    {
        public static int Sides = 100;

        public Dot CenterDot => Parents[0] as Dot;
        public Dot AnotherDot => Parents[1] as Dot;

        public Vector2 Center { get => CenterDot.Coord; set => CenterDot.Coord = value; }
        public Vector2 Another { get => AnotherDot.Coord; set => AnotherDot.Coord = value; }

        public float Radius
        {
            get => Vector2.Distance(Center, AnotherDot.Coord);
            set => throw new NotSupportedException();
        }

        public override event Action Moved;

        public Circle(Dot center, Dot another) : base()
        {
            Parents.Clear();
            Parents.Add(center);
            Parents.Add(another);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!(Enabled && CenterDot.Enabled && AnotherDot.Enabled)) return;
            base.Draw(sb);
            GUI.DrawCircle(sb, Center, Radius, Border, Color, Sides);
        }

        public override void Move(Vector2 add)
        {
            Center += add;
            Another += add;
        }
    }

    public interface ILine
    {
        Vector2 Point1 { get; set; }
        Vector2 Point2 { get; set; }

        float Grad { get; set; }
        float Yint { get; set; }
    }

    /*
    public abstract class LineLike : Shape
    {
        protected Dot _p1;
        public Dot Point1 { get => _p1; set { _p1 = value; _p1.Moved += dot_Moved; dot_Moved(); } }
        protected Dot _p2;
        public Dot Point2 { get => _p2; set { _p2 = value; _p2.Moved += dot_Moved; dot_Moved(); } }
        

        protected float _grad;
        public float Grad { get => _grad; set { _grad = value; dot_Moved(); } }
        protected float _yint;
        public float Yint { get => _yint; set { _yint = value; dot_Moved(); } }

        public override event Action Moved;

        public LineLike(ShapeManager manager, Dot p1, Dot p2) : base(manager)
        {
            _p1 = p1;
            _p2 = p2;
            _p1.Moved += () => { ResetAB(); Moved?.Invoke(); };
            _p2.Moved += () => { ResetAB(); Moved?.Invoke(); };
            Point1.dotParents.Add(this);
            Point2.dotParents.Add(this);
            ResetAB();
        }

        protected void dot_Moved()
        {
            ResetAB();
            Moved?.Invoke();
        }

        protected void ResetAB()
        {
            _grad = (Point2.Coord - Point1.Coord).Y / (Point2.Coord - Point1.Coord).X;
            _yint = (Point1.Coord.Y) - Grad * Point1.Coord.X;
        }

        protected void ResetPoints()
        {
            _p1 = new Dot(0, Yint);
            _p2 = new Dot(1, Grad + Yint);
            _p1.Moved += dot_Moved;
            _p2.Moved += dot_Moved;
        }

        public override void Move(Vector2 add)
        {
            if (!_p1.Selected)
                _p1.Move(add);
            if (!_p2.Selected)
                _p2.Move(add);
            ResetAB();

            Moved?.Invoke();
        }
    }
    */

    public partial class Line : Shape, ILine
    {
        public Dot Dot1 => Parents[0] as Dot;
        public Dot Dot2 => Parents[1] as Dot;

        public Vector2 Point1 { get => Dot1.Coord; set => Dot1.Coord = value; }
        public Vector2 Point2 { get => Dot2.Coord; set => Dot2.Coord = value; }

        public float Grad { get; set; }
        public float Yint { get; set; }

        public override event Action Moved;

        public Line(Dot d1, Dot d2) : base()
        {
            Parents.Clear();
            Parents.Add(d1);
            Parents.Add(d2);
        }
        
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (!(Enabled && Dot1.Enabled && Dot2.Enabled)) return;
            GUI.DrawLine(sb, new Vector2(0, Yint), new Vector2(Scene.CurrentScene.ScreenBounds.X, Scene.CurrentScene.ScreenBounds.X * Grad + Yint), Border, Color);
        }

        public override void Move(Vector2 add)
        {
            Point1 += add;
            Point2 += add;
        }
    }

    public class Segment : Shape, ILine
    {
        protected Dot Dot1 => Parents[0] as Dot;
        protected Dot Dot2 => Parents[1] as Dot;

        public Vector2 Point1 { get => Dot1.Coord; set => Dot1.Coord = value; }
        public Vector2 Point2 { get => Dot2.Coord; set => Dot2.Coord = value; }

        public float Grad { get; set; }
        public float Yint { get; set; }

        public override event Action Moved;

        public Segment(Dot p1, Dot p2) : base()
        {
            Parents.Clear();
            Parents.Add(p1);
            Parents.Add(p2);
        }
        
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (!(Enabled && Dot1.Enabled && Dot2.Enabled)) return;
            GUI.DrawLine(sb, Point1, Point2, Border, Color);
        }

        public override void Move(Vector2 add)
        {
            Point1 += add;
            Point2 += add;
        }
    }
    
    public class Vector : Segment
    {
        readonly static float arrowAngle = (float)Math.PI/6;
        readonly static float arrowlength = 20;

        //Point2 가 종점임.
        public Vector(Dot d1, Dot d2) : base(d1, d2)
        {
           
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            Vector2 del = Point2 - Point1;
            Vector2 delta1, delta2;
            float angle = (float)Math.Atan(del.Y/del.X);
            Vector2 Initvector = new Vector2(Point1.X + Vector2.Distance(Point1, Point2), Point1.Y);
            delta1 = new Vector2((float)(Initvector.X - arrowlength*Math.Cos(arrowAngle)) , (float)(Initvector.Y + arrowlength*Math.Sin(arrowAngle)));
            delta2 = new Vector2((float)(Initvector.X - arrowlength * Math.Cos(arrowAngle)), (float)(Initvector.Y - arrowlength * Math.Sin(arrowAngle)));
            if(del.X >0)
            {
                GUI.DrawLine(sb, Point2, Point1 + Geometry.Rotate(delta1 - Point1, angle), Border, Color);
                GUI.DrawLine(sb, Point2, Point1 + Geometry.Rotate(delta2 - Point1, angle), Border, Color);
            }
            else
            {
                GUI.DrawLine(sb, Point2, Point1 - Geometry.Rotate(delta1 - Point1, angle), Border, Color);
                GUI.DrawLine(sb, Point2, Point1 - Geometry.Rotate(delta2 - Point1, angle), Border, Color);
            }

        }
    }

    public partial class Dot : Shape
    {
        private static readonly float _nearDotDistance = 10;

        private Vector2 _coord;
        public Vector2 Coord { get => _coord; set => _coord = value; }
        public List<Shape> dotParents = new List<Shape>();

        public override event Action Moved;

        public Dot(Vector2 coord) : base()
        {
            _coord = coord;
            Color = Color.OrangeRed;
        }

        public Dot(float x, float y) : this(new Vector2(x, y))
        { }

        public override bool Equals(object obj)
        {
            var o = obj as Dot;
            return o == null ? false : Coord.Equals(o.Coord);
        }

        public override int GetHashCode()
            => Coord.GetHashCode();

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (!Enabled) return;
            GUI.DrawCircle(sb, Coord, 4f, Border, Color, 20);
            // GUI.DrawPoint(sb, Coord, Border, Color);
        }

        public override void MoveTo(Vector2 delta)
        {
            Coord += delta;
        }

        public override void Move(Vector2 add)
            => MoveTo(Coord + add);

        public override bool IsEnoughClose(Vector2 coord)
            => Distance <= _nearDotDistance;
    }
}
