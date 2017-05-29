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
        private Vector2 _lastPoint = new Vector2();
        private List<Shape> _shapes;
        private List<Vector2> _keypoints;
        private Vector2 _pos;

        public ConstructComponent()
        {
            _shapes = new List<Shape>();
            _keypoints = new List<Vector2>();
        }

        public void Clear()
        {
            _shapes.Clear();
            _keypoints.Clear();
        }

        public void ChangeState(DrawState state)
            => _drawState = state;

        private void UpdateLists(SpriteBatch sb)
        {
            foreach (var s in _shapes)
            {
                s.Draw(sb, 2, Color.Black);
            }
            foreach (var k in _keypoints)
            {
                GUI.DrawPoint(sb, k, 5, Color.Red);
            }
        }

        private void AddShape(Shape shape)
        {
            foreach (var s in _shapes)
            {
                var ints = Geometry.GetIntersect(shape, s);
                if (ints.Length != 0)
                    _keypoints.AddRange(ints.Where(i => !_keypoints.Contains(i)));
            }
            _shapes.Add(shape);
        }

        public override void Update()
        {
            base.Update();
            _pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (_drawState == DrawState.CIRCLE || _drawState == DrawState.LINE)
                {
                    if (!_wasDrawing)
                    {
                        _lastPoint = _pos;
                        _wasDrawing = true;
                    }
                }
            }
            if (_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_drawState == DrawState.CIRCLE)
                {
                    float radius = Vector2.Distance(_pos, _lastPoint);
                    AddShape(new Circle(_lastPoint, radius));
                }
                else if (_drawState == DrawState.LINE)
                {
                    AddShape(new Line(_lastPoint, _pos));
                }
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            _pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            if (_wasDrawing && _drawState == DrawState.CIRCLE)
            {
                float radius = (_pos - _lastPoint).Length();
                GUI.DrawCircle(sb, _lastPoint, radius, 2, Color.DarkGray, 100);
            }
            else if (_wasDrawing && _drawState == DrawState.LINE)
            {
                GUI.DrawLine(sb, _lastPoint, _pos, 2, Color.DarkGray);
            }

            UpdateLists(sb);
        }
    }
}
