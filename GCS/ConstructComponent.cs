﻿using System.Linq;
using System.Collections.Generic;
using Grid.Framework;
using Grid.Framework.Components;
using Grid.Framework.GUIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;

namespace GCS
{
    public class ConstructComponent : Renderable
    {
        public Vector2 Location { get; set; }
        public Point Size { get; set; } = new Point(10000, 10000);
        public Rectangle Bound => new Rectangle(Location.ToPoint(), Scene.CurrentScene.ScreenBounds);

        private DrawState _drawState = DrawState.NONE;
        private DotNaming _dotNamer = new DotNaming();
        private bool _wasDrawing = false;
        private bool _readyForDrag = false;
        private bool _isDragging = false;
        private Dot _ellipseLastPoint;
        private Dot _lastPoint;
        private List<Shape> _shapes;
        private Vector2 _pos;
        private Vector2 _rightPos;
        private List<Shape> _rightNearShapes;
        private List<Shape> _nearShapes;
        private List<Shape> _selectedShapes;
        private ContextMenuStrip _menuStrip;
        private List<ConstructRecode> _recodes;

        private bool _isAnyGuiUseMouse => _menuStrip.Focused || (Scene.CurrentScene as Main).GetFocused();
        private bool _isLeftMouseDown => Scene.CurrentScene.IsLeftMouseDown && _isAnyGuiUseMouse;
        private bool _isLeftMouseUp => Scene.CurrentScene.IsLeftMouseUp && _isAnyGuiUseMouse;
        private bool _isLeftMouseClicking => Scene.CurrentScene.IsLeftMouseClicking && _isAnyGuiUseMouse;

        private Dot samplepoint = Dot.FromCoord(0, 0);
        private Vector2 _lastpos;

        public ConstructComponent()
        {
            _shapes = new List<Shape>();
            _nearShapes = new List<Shape>();
            _selectedShapes = new List<Shape>();
            _recodes = new List<ConstructRecode>();
            _lastPoint = Dot.FromCoord(0, 0);
            OnCamera = false;

            InitMenuStrip();
        }

        private void InitMenuStrip()
        {
            _menuStrip = new ContextMenuStrip();
            _menuStrip.Items.Add("제거");

            _menuStrip.Items.Add("여기로 병합");
            _menuStrip.Items.Add("실행 취소");

            _menuStrip.Items[0].Click += (s, e) => DeleteSelected();
            _menuStrip.Items[1].Click += (s, e) => UpdateAttach();
            _menuStrip.Items[2].Click += (s, e) => Undo();
        }

        public void Clear()
        {
            _shapes.Clear();
            _selectedShapes.Clear();
        }

        public void DeleteSelected()
        {
            if (_selectedShapes.Count == 0) return;
            Delete(_selectedShapes);
            _selectedShapes.Clear();
        }

        private void Delete(IEnumerable<Shape> target)
        {
            List<Shape> sh = new List<Shape>();
            foreach (var s in target)
            {
                foreach (var ss in s.Delete())
                    if (_shapes.Contains(ss))
                    {
                        sh.Add(ss);
                        _shapes.Remove(ss);
                    }

            }
            _recodes.Add(new ConstructRecode(RecodeType.DELETE, sh));
        }

        public void Undo()
        {
            if (_recodes.Count == 0) return;
            ConstructRecode r = _recodes.Last();
            switch (r.type)
            {
                case RecodeType.CREATE:
                    foreach (var s in r.targetshapes)
                    {
                        _shapes.Remove(s);
                    }
                    break;

                case RecodeType.DELETE:
                    foreach (var s in r.targetshapes)
                        AddShape(s);
                    break;

                case RecodeType.MOVE:
                    foreach (Shape s in r.targetshapes)
                    {
                        s.Move(r.moveRecode);
                    }
                    break;
            }
            _recodes.RemoveAt(_recodes.Count - 1);
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
            if (shape is Dot && string.IsNullOrEmpty(shape.Name))
                shape.Name = _dotNamer.GetCurrent();
            shape.InitializeProperties();
            _shapes.Add(shape);
            _recodes.Add(new ConstructRecode(RecodeType.CREATE, new List<Shape>() { shape }));
        }

        private void Select(Shape shape)
        {
            shape.Selected = true;
            shape.UnSelect = false;
            _selectedShapes.Add(shape);
        }

        private void SelectAll()
        {
            foreach (var s in _shapes)
                Select(s);
        }

        private void Unselect(Shape shape)
        {
            shape.UnSelect = true;
            shape.Selected = false;
            _selectedShapes.Remove(shape);
        }

        private void UnselectAll()
        {
            _selectedShapes.ForEach(s => { s.UnSelect = true; s.Selected = false; });
            _selectedShapes.Clear();
        }

        public override void Update()
        {
            base.Update();
            //_pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            _pos = Mouse.GetState().Position.ToVector2() + Location;
            foreach (var s in _shapes) s.Update(_pos);
            //선택, 가까이있는 점 선택
            _nearShapes = _shapes.Where(s => s.Focused).ToList();

            UpdateRightClick();
            UpdateAdding();
            UpdateSelect();
            UpdateDrag();
            UpdateShortcuts();
        }

        private void UpdateAdding()
        {
            if (_isAnyGuiUseMouse) return;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (_drawState != DrawState.NONE)
                {
                    if (!_wasDrawing)
                    {
                        _lastPoint = GetDot(_pos);
                        _wasDrawing = true;
                    }
                    else if (_drawState == DrawState.DOT || _drawState == DrawState.ELLIPSE)
                        _lastPoint.MoveTo(_pos);
                }
            }
            if (_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_drawState == DrawState.DOT)
                {
                    AddShape(_lastPoint);
                }
                else if (_drawState == DrawState.ELLIPSE)
                {
                    _ellipseLastPoint = _lastPoint;
                    _drawState = DrawState.ELLIPSE_POINT;
                    _wasDrawing = false;
                    return;
                }
                else
                {
                    var p = GetDot(_pos);
                    AddShape(p);
                    AddShape(_lastPoint);
                    Shape sp = null;
                    if (_drawState == DrawState.CIRCLE)
                    {
                        sp = Circle.FromTwoDots(_lastPoint, p);
                        AddShape(sp);
                    }
                    else if (_drawState == DrawState.SEGMENT)
                    {
                        sp = Segment.FromTwoDots(_lastPoint, p);
                        AddShape(sp);
                    }
                    else if (_drawState == DrawState.LINE)
                    {
                        sp = Line.FromTwoDots(_lastPoint, p);
                        AddShape(sp);
                    }
                    else if (_drawState == DrawState.VECTOR)
                    {
                        sp = Vector.FromTwoDots(_lastPoint, p);
                        AddShape(sp);
                    }
                    else if (_drawState == DrawState.ELLIPSE_POINT)
                    {
                        AddShape(_ellipseLastPoint);
                        sp = Ellipse.FromThreeDots(_ellipseLastPoint, _lastPoint, p);
                        AddShape(sp);
                    }
                }
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }
        }

        private void UpdateAttach()
        {
            if (_drawState == DrawState.NONE)
            {
                if (_selectedShapes.Count == 1)
                {
                    if (_selectedShapes[0] is Dot)
                    {
                        if (_rightNearShapes?.Count > 0)
                        {
                            if (_selectedShapes[0].Parents.Count == 0)
                            {
                                var parent = GetDot(_rightPos, _rightNearShapes);
                                AddShape(parent);
                                (_selectedShapes[0] as Dot).AttachTo(parent);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSelect()
        {

            if (_isAnyGuiUseMouse) return;
            if (_drawState == DrawState.NONE)
            {
                if (_nearShapes.Count > 0)
                {
                    Shape nearest = _nearShapes[0];

                    float dist = int.MaxValue;
                    foreach (var s in _nearShapes)
                    {
                        if (s is Dot)
                        {
                            // 다 끝났다 그지 깽깽이들아!! 점이 우선순위 최고다!
                            nearest = s;
                            break;
                        }
                        if (dist > s.Distance)
                        {
                            nearest = s;
                            dist = s.Distance;
                        }
                    }

                    if (Scene.CurrentScene.IsLeftMouseDown)
                    {
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
                else
                {
                    if (Scene.CurrentScene.IsLeftMouseUp)
                    {
                        //UnselectAll();
                    }
                }
            }
        }

        private void UpdateDrag()
        {
            if (_drawState == DrawState.NONE)
            {
                if (_selectedShapes.Count == 0) return;
                if (Scene.CurrentScene.IsLeftMouseDown && _selectedShapes.Any(s => s.IsEnoughClose(_pos)))
                {
                    _readyForDrag = true;
                }
                if (_readyForDrag && Scene.CurrentScene.IsMouseMoved)
                {
                    _readyForDrag = false;
                    ConstructRecode r = new ConstructRecode(RecodeType.MOVE, _selectedShapes);
                    _recodes.Add(r);
                    _lastpos = samplepoint.Coord;
                    if (Scene.CurrentScene.IsLeftMouseClicking)
                        _isDragging = true;
                }

                if (_isDragging || Scene.CurrentScene.IsLeftMouseClicking && Scene.CurrentScene.IsMouseMoved)
                {
                    var diff = Scene.CurrentScene.MousePosition - Scene.CurrentScene.LastMousePosition;
                    if (_isDragging || _selectedShapes.Any(s => s.IsEnoughClose(_pos)))
                    {
                        if (!_isDragging) _isDragging = true;
                        _selectedShapes.ForEach(s => { s.UnSelect = false; s.Move(diff.ToVector2()); });
                        samplepoint.Move(diff.ToVector2());
                    }
                    if (Scene.CurrentScene.IsLeftMouseUp)
                    {
                        _isDragging = false;
                        _recodes.Last().WriteMoveRecode(samplepoint.Coord - _lastpos);
                        return;
                    }

                }
            }
        }

        private void UpdateRightClick()
        {
            // 우선 테스트 정도로 작동 대충 되도록 짜봤음
            if (_drawState == DrawState.NONE)
            {
                if (Scene.CurrentScene.IsRightMouseUp)
                {
                    //UnselectAll();
                    //Select(_nearShapes[0]);
                    _rightPos = _pos;
                    _rightNearShapes = _nearShapes.ToList();
                    _menuStrip.Show(System.Windows.Forms.Control.FromHandle(Scene.CurrentScene.Window.Handle),
                        Scene.CurrentScene.MousePosition.X, Scene.CurrentScene.MousePosition.Y);
                }
            }

            /*
            if (_shapeMenuStrip.IsSelected)
            {
                if (_shapeMenuStrip.SelectedItem.Text == "Delete")
                    DeleteSelected();
                else if (_shapeMenuStrip.SelectedItem.Text == "Merge")
                    UpdateAttach();
            }
            */
        }

        private void UpdateShortcuts()
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Delete))
            {
                DeleteSelected();
            }

            if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.A))
            {
                SelectAll();
            }
        }

        private Dot GetDot(Vector2 coord)
            => GetDot(coord, _nearShapes);

        /// <summary>
        /// 가까운 점이 있다면 그 점을, 없다면 새 점을
        /// </summary>
        private Dot GetDot(Vector2 coord, List<Shape> nears)
        {
            Dot nearestDot = null;
            Shape nearest = null;
            float distDot = int.MaxValue;
            float dist = int.MaxValue;
            foreach (var s in nears)
            {
                if (s is Dot)
                {
                    if (s.IsEnoughClose(coord))
                    {
                        nearestDot = s as Dot;
                        distDot = s.Distance;
                    }
                }
                else
                {
                    if (s.Distance <= dist)
                    {
                        nearest = s;
                        dist = s.Distance;
                    }
                }
            }
            if (nearestDot == null) // 가장 가까운게 점이 아니라면
            {
                if (nears.Count == 0)
                    return Dot.FromCoord(coord);
                if (nears.Count == 1)
                {
                    return OneShapeRuleDot(nearest, coord);
                }
                else if (nears.Count == 2)
                {
                    Vector2[] intersects = Geometry.GetIntersect(nears[0], nears[1]);
                    if (intersects.Length != 0)
                    {
                        Vector2 dot = intersects[0];
                        if (intersects.Length == 2)
                        {
                            dot = Vector2.Distance(coord, intersects[0]) < Vector2.Distance(intersects[1], coord)
                                ? intersects[0] : intersects[1];
                        }
                        return Dot.FromIntersection(nears[0], nears[1], dot);
                    }
                    else return OneShapeRuleDot(nearest, coord);
                }
                else return OneShapeRuleDot(nearest, coord);
            }
            else if (nearestDot is Dot) return nearestDot as Dot;
            else return Dot.FromCoord(coord);
        }

        private Dot OneShapeRuleDot(Shape nearest, Vector2 coord)
        {
            return Dot.FromOneShape(nearest, coord);
        }

        public override void Draw(SpriteBatch sb)
        {
            //_pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            sb.BeginAA();
            _pos = Mouse.GetState().Position.ToVector2() + Location;
            if (_wasDrawing || _drawState == DrawState.ELLIPSE_POINT)
            {
                if (_drawState == DrawState.CIRCLE)
                {
                    float radius = (_pos - _lastPoint.Coord).Length();
                    GUI.DrawCircle(sb, _lastPoint.Coord - Location, radius, 2, Color.DarkGray, 100);
                }
                else if (_drawState == DrawState.ELLIPSE)
                {
                    _lastPoint.Draw(sb);
                }
                else if (_drawState == DrawState.ELLIPSE_POINT)
                {
                    _ellipseLastPoint.Draw(sb);
                    Ellipse.FromThreeDots(_ellipseLastPoint, _lastPoint, Dot.FromCoord(_pos)).Draw(sb);
                }
                else if (_drawState == DrawState.SEGMENT)
                {
                    GUI.DrawLine(sb, _lastPoint.Coord - Location, _pos - Location, 2, Color.DarkGray);
                }
                else if (_drawState == DrawState.VECTOR)
                {
                    Vector.FromTwoDots(_lastPoint, Dot.FromCoord(_pos)).Draw(sb);
                }
                else if (_drawState == DrawState.LINE)
                {
                    Line.FromTwoPoints(_lastPoint.Coord, _pos).Draw(sb);
                }
                else if (_drawState == DrawState.DOT)
                {
                    _lastPoint.Draw(sb);
                }
            }

            UpdateLists(sb);
            sb.End();
        }

        public void SelectConstruct(ConstructType type)
        {
            switch (type)
            {
                case ConstructType.ParallelLine:
                    if (_selectedShapes.Count == 2)
                    {
                        if (_selectedShapes[0] is LineLike && _selectedShapes[1] is Dot
                        || _selectedShapes[0] is Dot && _selectedShapes[1] is LineLike)
                        {
                            var line = _selectedShapes[0] as LineLike ?? _selectedShapes[1] as LineLike;
                            var dot = _selectedShapes[0] as Dot ?? _selectedShapes[1] as Dot;
                            _shapes.Add(Line.ParallelLine(line, dot));
                        }
                    }
                    break;
                case ConstructType.PerpendicularLine:
                    if (_selectedShapes.Count == 2)
                    {
                        if (_selectedShapes[0] is LineLike && _selectedShapes[1] is Dot
                        || _selectedShapes[0] is Dot && _selectedShapes[1] is LineLike)
                        {
                            var line = _selectedShapes[0] as LineLike ?? _selectedShapes[1] as LineLike;
                            var dot = _selectedShapes[0] as Dot ?? _selectedShapes[1] as Dot;
                            _shapes.Add(Line.PerpendicularLine(line, dot));
                        }
                    }
                    break;
                case ConstructType.Tangent:
                    if (_selectedShapes.Count == 2)
                    {
                        if (_selectedShapes[0] is Circle && _selectedShapes[1] is Dot
                        || _selectedShapes[0] is Dot && _selectedShapes[1] is Circle)
                        {
                            var cir = _selectedShapes[0] as Circle ?? _selectedShapes[1] as Circle;
                            var dot = _selectedShapes[0] as Dot ?? _selectedShapes[1] as Dot;
                            _shapes.Add(Line.TangentLine(cir, dot));
                        }
                        else if (_selectedShapes[0] is Ellipse && _selectedShapes[1] is Dot
                        || _selectedShapes[0] is Dot && _selectedShapes[1] is Ellipse)
                        {
                            var elp = _selectedShapes[0] as Ellipse ?? _selectedShapes[1] as Ellipse;
                            var dot = _selectedShapes[0] as Dot ?? _selectedShapes[1] as Dot;
                            _shapes.Add(Line.TangentLine(elp, dot));
                        }
                    }
                    break;

                case ConstructType.Reflection:// 첫 선택이 대칭축, 두번째 선택이 대칭시킬 도형
                    if (_selectedShapes.Count == 2 )
                    {
                        if (_selectedShapes[0] is LineLike)
                        {
                            var axis = _selectedShapes[0] as LineLike;

                            if (_selectedShapes[1] is Ellipse) _shapes.Add(Ellipse.FromReflection(axis, _selectedShapes[1] as Ellipse));
                            else if (_selectedShapes[1] is Circle) _shapes.Add(Circle.FromReflection(axis, _selectedShapes[1] as Circle));
                            else if (_selectedShapes[1] is Line) _shapes.Add(Line.FromReflection(axis, _selectedShapes[1] as Line));
                            else if (_selectedShapes[1] is Vector) _shapes.Add(Vector.FromReflection(axis, _selectedShapes[1] as Vector));
                            else if (_selectedShapes[1] is Segment) _shapes.Add(Segment.FromReflection(axis, _selectedShapes[1] as Segment));
                            else _shapes.Add(Dot.FromReflection(axis, _selectedShapes[1] as Dot));

                        }

                    }
                    break;
                case ConstructType.Ellipse:
                    {
                        if (_selectedShapes.Count == 3)
                        {
                            var f1 = _selectedShapes[0] as Dot;
                            var f2 = _selectedShapes[1] as Dot;
                            var pin = _selectedShapes[2] as Dot;
                            if (f1 == null) return;
                            if (f2 == null) return;
                            if (pin == null) return;
                            AddShape(Ellipse.FromThreeDots(f1, f2, pin));
                        }
                        break;
                    }
            }
        }
    }
}
