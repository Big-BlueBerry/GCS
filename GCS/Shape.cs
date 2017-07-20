using System;
using System.Linq;
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
        
        private bool _disabled = false;
        public bool Disabled
        {
            get => _disabled || Parents.Any(p => p.Disabled);
            set => _disabled = value;
        }

        public string Name { get; set; }
        public float Border { get; set; } = 2f;
        public Color Color { get; set; } = Color.Black;
        public bool Focused { get; set; } = false;
        public bool Selected { get; set; } = false;
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
        public abstract void MoveTo(Vector2 at);

        public Shape()
        {
            Parents.Clear();
            Childs.Clear();
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

        public IEnumerable<Shape> Delete()
        {
            yield return this;
            foreach (var child in Childs)
                foreach (var c in child.Delete())
                    yield return c;
        }
    }

    public partial class Circle : Shape
    {
        public static int Sides = 100;

        public Vector2 Center { get; protected set; }
        public Vector2 Another { get; protected set; }

        public float Radius
        {
            get => Vector2.Distance(Center, Another);
            set => throw new NotSupportedException();
        }

        public Circle() : base() { }

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawCircle(sb, Center, Radius, Border, Color, Sides);
        }

        public override void Move(Vector2 add)
        {
            Center += add;
            Another += add;
            _rule.OnMoved();
        }

        public override void MoveTo(Vector2 at)
        {
            var diff = at - Center;
            Move(diff);
        }

        public static Circle FromTwoDots(Dot center, Dot another)
        {
            Circle circle = new Circle();
            new CircleOnTwoDotsRule(circle, center, another);
            return circle;
        }

        public void DetachRule()
        {
            _rule.Detach();
        }
    }

    public abstract partial class LineLike : Shape
    {
        public Vector2 Point1 { get; protected set; }
        public Vector2 Point2 { get; protected set; }

        public float Grad => (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
        public float Yint => Point1.Y - Grad * Point1.X;

        protected LineLike(Vector2? p1 = null, Vector2? p2 = null)
        {
            Point1 = p1 ?? new Vector2();
            Point2 = p2 ?? new Vector2();
        }

        public override void Move(Vector2 add)
        {
            Point1 += add;
            Point2 += add;

            _rule.OnMoved();
        }

        public override void MoveTo(Vector2 at)
        {
            var diff = at - Point1;
            Move(diff);
        }
    }

    public partial class Line : LineLike
    {
        protected Line(Vector2? p1 = null, Vector2? p2 = null) : base(p1, p2) { }
        
        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawLine(sb, new Vector2(0, Yint), new Vector2(Scene.CurrentScene.ScreenBounds.X, Scene.CurrentScene.ScreenBounds.X * Grad + Yint), Border, Color);
        }

        public static Line FromTwoDots(Dot d1, Dot d2)
        {
            Line line = new Line();
            var rule = new LineLikeOnTwoDotsRule(line, d1, d2);

            return line;
        }

        internal static Line FromTwoPoints(Vector2 p1, Vector2 p2)
            => new Line(p1, p2);
    }

    public class Segment : LineLike
    {
        protected Segment(Vector2? p1 = null, Vector2? p2 = null) : base(p1, p2) { }

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawLine(sb, Point1, Point2, Border, Color);
        }

        public static Segment FromTwoDots(Dot d1, Dot d2)
        {
            var seg = new Segment();
            var rule = new LineLikeOnTwoDotsRule(seg, d1, d2);

            return seg;
        }
    }
    
    public class Vector : Segment
    {
        readonly static float arrowAngle = (float)Math.PI/6;
        readonly static float arrowlength = 20;

        protected Vector(Vector2? p1 = null, Vector2? p2 = null) : base(p1, p2) { }

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
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

        public new static Vector FromTwoDots(Dot d1, Dot d2)
        {
            var vec = new Vector();
            var rule = new LineLikeOnTwoDotsRule(vec, d1, d2);

            return vec;
        }
    }

    public partial class Dot : Shape
    {
        private static readonly float _nearDotDistance = 10;

        private Vector2 _coord;
        public Vector2 Coord { get => _coord; set => _coord = value; }

        protected Dot(Vector2 coord) : base()
        {
            _coord = coord;
            Color = Color.OrangeRed;
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
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawCircle(sb, Coord, 4f, Border, Color, 20);
            // GUI.DrawPoint(sb, Coord, Border, Color);
        }

        public override void MoveTo(Vector2 at)
        {
            var diff = at - Coord;
            Move(diff);
        }

        public override void Move(Vector2 delta)
        {
            Coord += delta;
            _rule?.OnMoved();
        }

        public override bool IsEnoughClose(Vector2 coord)
            => Distance <= _nearDotDistance;

        public static Dot FromCoord(Vector2 coord)
        {
            var dot = new Dot(coord);
            new EmptyDotRule(dot);
            return dot;
        }

        public static Dot FromCoord(float x, float y)
            => FromCoord(new Vector2(x, y));

        public static Dot FromOneShape(Shape shape, Vector2 coord)
        {
            var dot = new Dot(coord);
            new DotOnShapeRule(dot, shape);
            return dot;
        }

        public static Dot FromIntersection(Shape p1, Shape p2, Vector2 coord)
        {
            var dot = new Dot(coord);
            new DotOnIntersectionRule(dot, p1, p2);
            return dot;
        }
    }
}
