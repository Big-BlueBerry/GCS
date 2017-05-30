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
        private Vector2 _pos;

        public ConstructComponent()
        {
            _shapes = new List<Shape>();
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
                //GUI.DrawPoint(sb, Geometry.GetNearest(s, _pos), 5, Color.Blue);
            }
        }

        private void AddShape(Shape shape)
        {
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
                    AddShape(new Segment(_lastPoint, _pos));
                }
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            //_pos = Camera.Current.GetRay(Mouse.GetState().Position.ToVector2());
            sb.BeginAA();
            _pos = Mouse.GetState().Position.ToVector2();
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
            sb.End();
        }
    }
}
