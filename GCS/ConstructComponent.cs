using System.Linq;
using System.Collections.Generic;
using Grid.Framework;
using Grid.Framework.Components;
using GCS.Rules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GCS
{
    public class ConstructComponent : Renderable
    {
        private DrawState _drawState = DrawState.NONE;
        private bool _wasDrawing = false;
        private bool _isDragging = false;
        private Dot _lastPoint = new Dot(0, 0);
        private List<Shape> _shapes;
        private Vector2 _pos;

        private readonly float _nearDistance = 5;
        private readonly float _nearDotDistance = 10;
        private List<(Shape, float)> _nearShapes;
        private List<Shape> _selectedShapes;
        private List<IntersectRule> _currentRules=new List<IntersectRule>(); 
        private List<ImportantAction> _actionStack=new List<ImportantAction>();

        public ConstructComponent()
        {
            _shapes = new List<Shape>();
            _nearShapes = new List<(Shape, float)>();
            _selectedShapes = new List<Shape>();
            OnCamera = false;
        }

        public void Clear()
        {
            _shapes.Clear();
        }
        public void Delete()// 여기 removeAll 이나 removeRange 같은 걸로 리팩토링 좀 해주셈 저것들 사용법 모름.
        {
            //그리고 for문 돌면서 리스트 객체들 없애면 예외 발생함
            foreach (var shape in _selectedShapes)
            {
                _shapes.Remove(shape);
                List<IntersectRule> temp = new List<IntersectRule>();
                foreach (IntersectRule r in _currentRules)
                {
                    if (r.Parent1 == shape || r.Parent2 == shape)
                    {
                        Dot d = r.Dot;
                        Vector2 newcoord = d.Coord;
                        _shapes.Remove(d);
                        foreach (var s in _shapes)//이건 _nearShapes 재설정 부분인데 코드 단축시켜야 될까?
                        {
                                var dist = Geometry.GetNearestDistance(s, newcoord);
                                if (EnoughClose(s, newcoord))
                                {
                                    if (!s.Focused)
                                    {
                                        _nearShapes.Add((s, dist));
                                        s.Focused = true;
                                    }
                                }
                                else if (s.Focused)
                                {
                                    for (int i = 0; i < _nearShapes.Count; i++)
                                    {
                                        if (_nearShapes[i].Item1 == s)
                                        {
                                            _nearShapes.RemoveAt(i);
                                            break;
                                        }
                                    }
                                s.Focused = false;
                                }
                        }
                        AddShape(GetDot(newcoord));
                    }
                }
 
            }
            _selectedShapes.Clear();
        }

        public void Undo()
        {

        }

        public void ChangeState(DrawState state)
            => _drawState = state;

        private void UpdateLists(SpriteBatch sb)
        {
            foreach (var s in _shapes)
            {
                s.Draw(sb);
            }
        }

        private void AddShape(Shape shape)
        {
            if (_shapes.Contains(shape)) return;
            var keypoints = new List<Shape>();
            foreach (var s in _shapes)
            {
                var dots = from d in Geometry.GetIntersect(shape, s)
                           select new Dot(d);
                if (dots.Count() != 0)
                    keypoints.AddRange(dots.Where(d => !_shapes.Contains(d)));
            }
            _shapes.Add(shape);
            //_shapes.AddRange(keypoints);
        }

        public override void Update()
        {
            base.Update();
            //_pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            _pos = Mouse.GetState().Position.ToVector2();
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (_drawState != DrawState.NONE)
                {
                    if (!_wasDrawing)
                    {
                        _lastPoint = GetDot(_pos);
                        _wasDrawing = true;
                    }
                    else if (_drawState == DrawState.DOT)
                        _lastPoint.MoveTo(_pos);
                }
            }
            if (_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_drawState == DrawState.DOT)
                {
                    AddShape(_lastPoint);
                }
                else
                {
                    var p = GetDot(_pos);
                    AddShape(p);
                    AddShape(_lastPoint);
                    if (_drawState == DrawState.CIRCLE)
                    {
                        AddShape(new Circle(_lastPoint, p));
                    }
                    else if (_drawState == DrawState.SEGMENT)
                    {
                        AddShape(new Segment(_lastPoint, p));
                    }
                    else if (_drawState == DrawState.LINE)
                    {
                        AddShape(new Line(_lastPoint, p));
                    }
                }
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }

            //선택, 가까이있는 점 선택
            foreach (var s in _shapes)
            {
                var dist = Geometry.GetNearestDistance(s, _pos);
                if (EnoughClose(s, _pos))
                {
                    if (!s.Focused)
                    {
                        _nearShapes.Add((s, dist));
                        s.Focused = true;
                    }
                }
                else if (s.Focused)
                {
                    for (int i = 0; i < _nearShapes.Count; i++)
                    {
                        if (_nearShapes[i].Item1 == s)
                        {
                            _nearShapes.RemoveAt(i);
                            break;
                        }
                    }
                    s.Focused = false;
                }
            }

            if (_drawState == DrawState.NONE)
            {
                if (_nearShapes.Count > 0)
                {
                    Shape nearest = _nearShapes[0].Item1;
                    if (Scene.CurrentScene.IsLeftMouseDown)
                    {
                        float dist = _nearDistance;
                        foreach (var (s, d) in _nearShapes)
                        {
                            if (s is Dot)
                            {
                                // 다 끝났다 그지 깽깽이들아!! 점이 우선순위 최고다!
                                nearest = s;
                                break;
                            }
                            if (dist > d)
                            {
                                nearest = s;
                                dist = d;
                            }
                        }
                        if (!nearest.Selected)
                        {
                            _selectedShapes.ForEach(s => s.UnSelect = true);
                            _selectedShapes.Add(nearest);
                            nearest.Selected = true;
                            nearest.UnSelect = false;
                        }
                        else
                            nearest.UnSelect = true;
                    }
                    if (Scene.CurrentScene.IsLeftMouseUp)
                    {
                        if (nearest.Selected && nearest.UnSelect)
                        {
                            _selectedShapes.Remove(nearest);
                            nearest.Selected = false;
                        }
                    }
                }
                if (_isDragging || Scene.CurrentScene.IsLeftMouseClicking && Scene.CurrentScene.IsMouseMoved)
                {
                    var diff = Scene.CurrentScene.MousePosition - Scene.CurrentScene.LastMousePosition;
                    if (_isDragging || _selectedShapes.Any(s => EnoughClose(s, _pos)))
                    {
                        if (!_isDragging) _isDragging = true;
                        _selectedShapes.ForEach(s => { s.UnSelect = false; s.Move(diff.ToVector2()); });
                    }
                    if (Scene.CurrentScene.IsLeftMouseUp)
                        _isDragging = false;
                }

            }
        }

        /// <summary>
        /// 가까운 점이 있다면 그 점을, 없다면 새 점을
        /// </summary>
        private Dot GetDot(Vector2 coord)
        {
            Dot nearestDot = null;
            float distDot = _nearDotDistance;
            Shape nearest = null;
            float dist = _nearDistance;
            foreach (var (s, d) in _nearShapes)
            {
                if (s is Dot)
                {
                    if (d <= distDot && d <= _nearDotDistance)
                    {
                        nearestDot = s as Dot;
                        distDot = d;
                    }
                }
                else
                {
                    if (d <= dist)
                    {
                        nearest = s;
                        dist = d;
                    }
                }
            }
            if (nearestDot == null) // 가장 가까운게 점이 아니라면
            {
                if (_nearShapes.Count == 0)
                    return new Dot(coord);
                if (_nearShapes.Count == 1)
                {
                    return OneShapeRuleDot(nearest, coord);
                }
                else if (_nearShapes.Count == 2)
                {
                    Vector2[] intersects = Geometry.GetIntersect(_nearShapes[0].Item1, _nearShapes[1].Item1);
                    if (intersects.Length != 0)
                    {
                        Dot dot;
                        if (intersects.Length == 2 )
                         {
                            dot = Vector2.Distance(coord, intersects[0]) < Vector2.Distance(intersects[1], coord) ? new Dot(intersects[0]) : new Dot(intersects[1]);
                        }
                         else dot = new Dot(intersects[0]);
                        //dot = new Dot(intersects[0]);
                        IntersectRule rule=new IntersectRule(dot, _nearShapes[0].Item1, _nearShapes[1].Item1);
                        _currentRules.Add(rule);
                        return dot;
                    }
                    else return OneShapeRuleDot(nearest, coord);
                }
                else return OneShapeRuleDot(nearest, coord);
            }
            else if (nearestDot is Dot) return nearestDot as Dot;
            else return new Dot(coord);
        }

        private Dot OneShapeRuleDot(Shape nearest, Vector2 coord)
        {
            IParentRule rule = null;
            Dot dot = new Dot(Geometry.GetNearest(nearest, coord));
            if (nearest is Line)
                rule = new LineRule(dot, nearest as Line);
            else if (nearest is Segment)
                rule = new SegmentRule(dot, nearest as Segment);
            else if (nearest is Circle)
                rule = new CircleRule(dot, nearest as Circle);
            else
                throw new System.Exception("일어날 수 없음");
            return dot;
        }

        private bool EnoughClose(Shape shape, Vector2 coord)
        {
            var distance = Geometry.GetNearestDistance(shape, coord);
            return shape is Dot ? distance <= _nearDotDistance : distance <= _nearDistance;
        }

        public override void Draw(SpriteBatch sb)
        {
            //_pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            sb.BeginAA();
            _pos = Mouse.GetState().Position.ToVector2();
            if (_wasDrawing)
            {
                if (_drawState == DrawState.CIRCLE)
                {
                    float radius = (_pos - _lastPoint.Coord).Length();
                    GUI.DrawCircle(sb, _lastPoint.Coord, radius, 2, Color.DarkGray, 100);
                }
                else if (_drawState == DrawState.SEGMENT)
                {
                    GUI.DrawLine(sb, _lastPoint.Coord, _pos, 2, Color.DarkGray);
                }
                else if (_drawState == DrawState.LINE)
                {
                    new Line(new Dot(_lastPoint.Coord), new Dot(_pos)).Draw(sb);
                }
                else if (_drawState == DrawState.DOT)
                {
                    _lastPoint.Draw(sb);
                }
            }

            UpdateLists(sb);
            sb.End();
        }
    }
}
