using System.Linq;
using System.Collections.Generic;
using Grid.Framework;
using Grid.Framework.Components;
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
            _shapes.AddRange(keypoints);
        }

        public override void Update()
        {
            base.Update();
            //_pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            _pos = Mouse.GetState().Position.ToVector2();
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (_drawState == DrawState.CIRCLE || _drawState == DrawState.SEGMENT || _drawState == DrawState.LINE)
                {
                    if (!_wasDrawing)
                    {
                        _lastPoint = GetDot(_pos);
                        _wasDrawing = true;
                    }
                }
            }
            if (_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
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
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }
            
            //선택, 가까이있는 점 선택
            foreach (var s in _shapes)
            {
                var dist = Geometry.GetNearestDistance(s, _pos);
                if (dist <= (s is Dot ? _nearDotDistance : _nearDistance))
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
                if(_isDragging || Scene.CurrentScene.IsLeftMouseClicking && Scene.CurrentScene.IsMouseMoved)
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
            Dot dot = null;
            float dist = _nearDotDistance;
            foreach(var s in _shapes)
            {
                if (!(s is Dot)) continue;
                var d = Geometry.GetNearestDistance(s, coord);
                if(d < dist && d < _nearDotDistance)
                {
                    dot = s as Dot;
                    dist = d;
                }
            }
            return dot ?? new Dot(coord);
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
            if (_wasDrawing && _drawState == DrawState.CIRCLE)
            {
                float radius = (_pos - _lastPoint.Coord).Length();
                GUI.DrawCircle(sb, _lastPoint.Coord, radius, 2, Color.DarkGray, 100);
            }
            else if (_wasDrawing && _drawState == DrawState.SEGMENT)
            {
                GUI.DrawLine(sb, _lastPoint.Coord, _pos, 2, Color.DarkGray);
            }
            else if(_wasDrawing && _drawState == DrawState.LINE)
            {
                new Line(new Dot(_lastPoint.Coord), new Dot(_pos)).Draw(sb);
            }

            UpdateLists(sb);
            sb.End();
        }
    }
}
