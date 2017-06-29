using System;
using Microsoft.Xna.Framework;

namespace GCS.Rules
{
    public class IntersectRule : IParentRule
    {
        public Shape Parent1;
        public Shape Parent2;
        public Dot Dot { get; private set; }
        public event Action<Vector2> MoveTo;
        private bool _parentMoved = false;
        private int _orientation=0;
        private int _firstdirection=0;
        
        public IntersectRule(Dot dot, Shape p1, Shape p2)
        {
            Dot = dot;
            Dot.Rule = this;
            
            Parent1 = p1; Parent2 = p2;
            p1.Moved += Parent_Moved;
            p2.Moved += Parent_Moved;
            dot.Moved += Dot_Moved;
            
        }

        private void Dot_Moved()
        {
            if (_parentMoved) return;
            //Dot.Coord = FixedCoord(Dot.Coord);
        }

        private void Parent_Moved()
        {
            _parentMoved = true;
            Dot.Coord = FixedCoord(Dot.Coord);
            MoveTo?.Invoke(Dot.Coord);
            _parentMoved = false;
        }

        public Vector2 FixedCoord(Vector2 original)
        {
            _parentMoved = true;
            var vs = Geometry.GetIntersect(Parent1, Parent2);

            if (_orientation == 0 )
            {
                if (Parent1 is Circle && Parent2 is Circle)
                {
                    if (_firstdirection == 0) _firstdirection = (Parent2 as Circle).Center.Coord.X < (Parent1 as Circle).Center.Coord.X ? -1 : 1;
                    _orientation = orient_check(Dot.Coord, new Line((Parent1 as Circle).Center, (Parent2 as Circle).Center));
                }
                else if (Parent1 is Circle || Parent2 is Circle)
                {
                    Circle c = Parent1 is Circle ? (Parent1 as Circle) : (Parent2 as Circle);
                    Shape s = Parent1 is Circle ?  Parent2  : Parent1 ;//요기
                    if (s is Line && _firstdirection == 0) _firstdirection = (s as Line).Point1.Coord.Y < (s as Line).Point2.Coord.Y ? -1 : 1;
                    else if (s is Segment && _firstdirection == 0) _firstdirection = (s as Segment).Point1.Coord.Y < (s as Segment).Point2.Coord.Y ? -1 : 1;
                    _orientation = orient_check(Dot.Coord, new Line(c.Center, new Dot(Geometry.GetNearest(s, c.Center.Coord))));
                }

            }
            
            if (vs.Length == 0) { Dot.Enabled = false; _parentMoved = false;   return Vector2.Zero; }
            else Dot.Enabled = true;
            _parentMoved = false;
            if(vs.Length==1)
            {
                return vs[0];
            }
            else if (Parent2 is Line || Parent1 is Line)
            {
                Circle c = Parent1 is Circle ? (Parent1 as Circle) : (Parent2 as Circle);
                Line s = Parent1 is Circle ? Parent2 as Line : Parent1 as Line;
                int line_orient = orient_check(vs[0], new Line(c.Center, new Dot(Geometry.GetNearest(s, c.Center.Coord))));

                if (_firstdirection == -1)
                {
                    if (s.Point1.Coord.Y < s.Point2.Coord.Y)
                        return line_orient == _orientation ? vs[0] : vs[1];
                    else return line_orient == _orientation ? vs[1] : vs[0];
                }
                else
                {
                    if (s.Point1.Coord.Y < s.Point2.Coord.Y)
                        return line_orient == _orientation ? vs[1] : vs[0];
                    else return line_orient == _orientation ? vs[0] : vs[1];
                }
            }
            else if (Parent2 is Segment || Parent1 is Segment)
            {
                Circle c = Parent1 is Circle ? (Parent1 as Circle) : (Parent2 as Circle);
                Segment s = Parent1 is Circle ? Parent2 as Segment : Parent1 as Segment;
                int line_orient = orient_check(vs[0], new Line(c.Center, new Dot(Geometry.GetNearest(s, c.Center.Coord))));

                if (_firstdirection == -1)
                {
                    if (s.Point1.Coord.Y < s.Point2.Coord.Y)
                        return line_orient == _orientation ? vs[0] : vs[1];
                    else return line_orient == _orientation ? vs[1] : vs[0];
                }
                else
                {
                    if (s.Point1.Coord.Y < s.Point2.Coord.Y)
                        return line_orient == _orientation ? vs[1] : vs[0];
                    else return line_orient == _orientation ? vs[0] : vs[1];
                }
            }
            else
            {
                int circle_orient = orient_check(vs[0], new Line((Parent1 as Circle).Center, (Parent2 as Circle).Center));
                if (_firstdirection == -1)
                {
                    if ((Parent2 as Circle).Center.Coord.X < (Parent1 as Circle).Center.Coord.X)
                        return circle_orient == _orientation ? vs[0] : vs[1];
                    else return circle_orient == _orientation ? vs[1] : vs[0];
                }
                else
                {
                    if ((Parent2 as Circle).Center.Coord.X < (Parent1 as Circle).Center.Coord.X)
                        return circle_orient == _orientation ? vs[1] : vs[0];
                    else return circle_orient == _orientation ? vs[0] : vs[1];
                }
            }
            //else return Vector2.Distance(vs[0], Dot.Coord) < Vector2.Distance(vs[1], Dot.Coord) ? vs[0] : vs[1];
        }
        public static int orient_check(Vector2 d, Line l)
        {
            if (l.Grad * d.X + l.Yint > d.Y) return -1;
            else return 1;
        }

        public void Dispose()
        {
            Parent1.Moved -= Parent_Moved;
            Parent2.Moved -= Parent_Moved;
            Parent1 = null;
            Parent2 = null;
            Dot = null;
            MoveTo = null;
        }

        public bool IsParent(Shape shape)
            => Parent1 == shape || Parent2 == shape;
    }
}
