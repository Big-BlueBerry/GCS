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

        public abstract class BasisDots : IEnumerable<Vector2>
        {
            protected Shape _shape;
            private int _count;
            public BasisDots(Shape shape, int count)
            {
                this._shape = shape;
                this._count = count;
            }
            public abstract Vector2 this[int i] { get; set; }

            public IEnumerator<Vector2> GetEnumerator()
            {
                return new DotEnumerator(this);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                => GetEnumerator();

            public class DotEnumerator : IEnumerator<Vector2>
            {
                BasisDots _dots;
                int _cur;
                public DotEnumerator(BasisDots dots)
                {
                    _dots = dots;
                    _cur = -1;
                }

                public bool MoveNext()
                {
                    return ++_cur < _dots._count;
                }

                public void Reset()
                {
                    _cur = -1;
                }

                object System.Collections.IEnumerator.Current
                    => this.Current;

                public Vector2 Current
                    => _dots[_cur];

                public void Dispose() { }
            }
        }

        public BasisDots DotList;
        internal List<Shape> Parents { get; private set; }
        internal List<Shape> Childs { get; private set; }

        internal ShapeRule _rule = null;
        
        private bool _disabled = false;
        public bool Disabled
        {
            get => _disabled || Parents.Any(p => p.Disabled);
            set => _disabled = value;
        }

        protected ConstructComponent _comp;
        protected Vector2 _drawDelta => -_comp.Location;

        public bool IsShowingName { get; set; } = true;
        public string Name { get; set; }
        public float Border { get; set; } = 2f;
        public Color Color { get; set; } = Color.Black;
        public Color FocusedColor { get; set; } = Color.Orange;
        public Color SelectedColor { get; set; } = Color.Cyan;
        public Color TextColor { get; set; } = Color.Black;
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
            if (Focused) Color = FocusedColor;
            if (Selected) Color = SelectedColor;
        }
        public abstract void Move(Vector2 add);
        public abstract void MoveTo(Vector2 at);

        public Shape()
        {
            Parents = new List<Shape>();
            Childs = new List<Shape>();

            if (_comp == null)
                _comp = GameObject.Find("construct").GetComponent<ConstructComponent>();
        }

        public void InitializeProperties()
        {
            Selected = false;
            Focused = false;
            UnSelect = false;
            Disabled = false;
        }

        public virtual bool IsEnoughClose(Vector2 coord)
            => Geometry.GetNearestDistance(this, coord) <= _nearDistance;

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

        public class CircleDots : BasisDots
        {
            public CircleDots(Circle circle) : base(circle, 2) { }
            public override Vector2 this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return (_shape as Circle).Center;
                        case 1: return (_shape as Circle).Another;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (i)
                    {
                        case 0: (_shape as Circle).Center = value; break;
                        case 1: (_shape as Circle).Another = value;  break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        public Vector2 Center { get; protected set; }
        public Vector2 Another { get; protected set; }

        public float Radius
        {
            get => Vector2.Distance(Center, Another);
            set => throw new NotSupportedException();
        }

        protected Circle() : base()
        {
            DotList = new CircleDots(this);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawCircle(sb, Center + _drawDelta, Radius, Border, Color, Sides);
        }

        public override void Move(Vector2 add)
        {
            Center += add;
            Another += add;
            _rule?.OnMoved();
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

        public static Circle FromReflection(LineLike axis, Circle original)
        {
            Circle cir = new Circle();
            new ReflectedShapeRule(axis, original, cir);
            return cir;
        }
    }

    public partial class Ellipse : Shape
    {
        public static int Sides = 100;

        public class EllipseDots : BasisDots
        {
            public EllipseDots(Ellipse ellipse) : base(ellipse, 3) { }
            public override Vector2 this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return (_shape as Ellipse).Focus1;
                        case 1: return (_shape as Ellipse).Focus2;
                        case 2: return (_shape as Ellipse).PinPoint;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (i)
                    {
                        case 0: (_shape as Ellipse).Focus1 = value; break;
                        case 1: (_shape as Ellipse).Focus2 = value; break;
                        case 2: (_shape as Ellipse).PinPoint = value; break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        public Vector2 Focus1 { get; protected set; }
        public Vector2 Focus2 { get; protected set; }
        public Vector2 PinPoint { get; protected set; }

        public Vector2 Center => (Focus1 + Focus2) / 2;
        public float Sublength => Vector2.Distance(Focus1, Focus2) / 2;//c 
        public float Semimajor => (Vector2.Distance(Focus1, PinPoint) + Vector2.Distance(Focus2, PinPoint)) / 2;//a
        public float Semiminor => (float)Math.Sqrt(Semimajor * Semimajor - Sublength * Sublength);//b


        protected Ellipse() : base()
        {
            DotList = new EllipseDots(this);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawEllipse(sb, Focus1 + _drawDelta, Focus2 + _drawDelta, PinPoint + _drawDelta, Border, Color, Sides);
        }

        public override void Move(Vector2 add)
        {
            Focus1 += add;
            Focus2 += add;
            PinPoint += add;
            _rule?.OnMoved();
        }

        public override void MoveTo(Vector2 at)
        {
            var diff = at - PinPoint;
            Move(diff);
        }

        public static Ellipse FromThreeDots(Dot f1, Dot f2, Dot pin)
        {
            Ellipse ellipse = new Ellipse();
            new EllipseOnThreeDotsRule(ellipse, f1, f2, pin);
            return ellipse;
        }

        public static Ellipse FromReflection(LineLike axis, Ellipse original)
        {
            Ellipse elp = new Ellipse();
            new ReflectedShapeRule(axis, original, elp);
            return elp;
        }
    }

    public abstract partial class LineLike : Shape
    {
        public class LineDots : BasisDots
        {
            public LineDots(LineLike line) : base(line, 2) { }
            public override Vector2 this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return (_shape as LineLike).Point1;
                        case 1: return (_shape as LineLike).Point2;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (i)
                    {
                        case 0: (_shape as LineLike).Point1 = value; break;
                        case 1: (_shape as LineLike).Point2 = value; break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }
        }
        public Vector2 Point1 { get; protected set; }
        public Vector2 Point2 { get; protected set; }
        
        public float Grad => (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
        public float Yint => Point1.Y - Grad * Point1.X;

        protected LineLike(Vector2? p1 = null, Vector2? p2 = null)
        {
            DotList = new LineDots(this);
            Point1 = p1 ?? new Vector2();
            Point2 = p2 ?? new Vector2();

            Move(Vector2.Zero);
        }

        public override void Move(Vector2 add)
        {
            Point1 += add;
            Point2 += add;

            //Grad = (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
            //Yint = Point1.Y - Grad * Point1.X;

            _rule?.OnMoved();
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
            GUI.DrawLine(sb, new Vector2(0, Yint) + _drawDelta, new Vector2(_comp.Size.X, _comp.Size.X * Grad + Yint) + _drawDelta, Border, Color);
        }

        public static Line FromTwoDots(Dot d1, Dot d2)
        {
            Line line = new Line();
            var rule = new LineLikeOnTwoDotsRule(line, d1, d2);

            return line;
        }

        internal static Line FromTwoPoints(Vector2 p1, Vector2 p2)
            => new Line(p1, p2);

        public static Line ParallelLine(LineLike original, Dot on)
        {
            Line line = new Line();
            new ParallelLineRule(line, original, on);

            return line;
        }

        public static Line PerpendicularLine(LineLike original, Dot on)
        {
            Line line = new Line();
            new PerpendicularLineRule(line, original, on);

            return line;
        }

        public static Line TangentLine(Circle original, Dot on)
        {
            Line line = new Line();
            new TangentLineRule(line, original, on);

            return line;
        }

        public static Line TangentLine(Ellipse original, Dot on)
        {
            Line line = new Line();
            new TangentLineRule(line, original, on);

            return line;
        }

        public static Line FromReflection(LineLike axis, Line original )
        {
            Line lin = new Line();
            new ReflectedShapeRule(axis, original, lin);
            return lin;
        }
    }

    public class Segment : LineLike
    {
        protected Segment(Vector2? p1 = null, Vector2? p2 = null) : base(p1, p2) { }

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            GUI.DrawLine(sb, Point1 + _drawDelta, Point2 + _drawDelta, Border, Color);
        }

        public static Segment FromTwoDots(Dot d1, Dot d2)
        {
            var seg = new Segment();
            var rule = new LineLikeOnTwoDotsRule(seg, d1, d2);

            return seg;
        }

        public static Segment FromReflection(LineLike axis, Segment original)
        {
            Segment lin = new Segment();
            new ReflectedShapeRule(axis, original, lin);
            return lin;
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
                GUI.DrawLine(sb, Point2 + _drawDelta, Point1 + Geometry.Rotate(delta1 - Point1, angle) + _drawDelta, Border, Color);
                GUI.DrawLine(sb, Point2 + _drawDelta, Point1 + Geometry.Rotate(delta2 - Point1, angle) + _drawDelta, Border, Color);
            }
            else
            {
                GUI.DrawLine(sb, Point2 + _drawDelta, Point1 - Geometry.Rotate(delta1 - Point1, angle) + _drawDelta, Border, Color);
                GUI.DrawLine(sb, Point2 + _drawDelta, Point1 - Geometry.Rotate(delta2 - Point1, angle) + _drawDelta, Border, Color);
            }
        }

        public new static Vector FromTwoDots(Dot d1, Dot d2)
        {
            var vec = new Vector();
            var rule = new LineLikeOnTwoDotsRule(vec, d1, d2);

            return vec;
        }

        public static Vector FromReflection(LineLike axis, Vector original)
        {
            Vector lin = new Vector();
            new ReflectedShapeRule(axis, original, lin);
            return lin;
        }
    }
 

    public partial class Dot : Shape
    {
        private static readonly float _nearDotDistance = 10;

        public class DotDots : BasisDots
        {
            public DotDots(Dot dot) : base(dot, 1) { }
            public override Vector2 this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return (_shape as Dot).Coord;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (i)
                    {
                        case 0: (_shape as Dot).Coord = value; break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }
        }
        private Vector2 _coord;
        public Vector2 Coord { get => _coord; set => _coord = value; }

        protected Dot(Vector2 coord) : base()
        {
            DotList = new DotDots(this);
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

        public override bool IsEnoughClose(Vector2 coord)
            => Geometry.GetNearestDistance(this, coord) <= _nearDotDistance;

        public override void Draw(SpriteBatch sb)
        {
            if (Disabled) return;
            base.Draw(sb);
            if (_rule != null && _rule is DotOnDotRule)
                if (Parents[0].Selected)
                    Color = SelectedColor;
            GUI.DrawCircle(sb, Coord + _drawDelta, 4f, Border, Color, 20);
            // GUI.DrawPoint(sb, Coord, Border, Color);\
            if (IsShowingName && Name != null)
            {
                var size = Resources.Font.MeasureString(Name).ToPoint();
                GUI.DrawString(sb,
                    Resources.Font,
                    Name,
                    Alignment.Left | Alignment.Bottom,
                    new Rectangle((Coord - _comp.Location).ToPoint() - size, size),
                    TextColor,
                    0f);
            }
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

        public static Dot FromReflection(LineLike axis, Dot original )
        {
            Dot d = new Dot(Vector2.Zero);
            new ReflectedShapeRule(axis, original, d);
            return d;
        }
        public void AttachTo(Dot parent)
        {
            new DotOnDotRule(this, parent);
        }
    }
}
