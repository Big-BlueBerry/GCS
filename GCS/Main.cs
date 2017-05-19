using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grid.Framework;
using Grid.Framework.GUIs;

namespace GCS
{
    public class Main : Scene
    {
        private Button _compassBtn, _rulerBtn, _clearBtn;
        private DrawState _drawState = DrawState.NONE;
        private bool _wasDrawing = false;
        private Vector2 _lastPoint = new Vector2();
        private List<Shape> _shapes;
        private List<Vector2> _keypoints;

        protected override void InitSize()
        {
            base.InitSize();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GUIManager.DefaultFont = LoadContent<SpriteFont>("basicfont");
            _compassBtn = new Button(10, 10, 120, 80, "Compass");
            _rulerBtn = new Button(140, 10, 120, 80, "Ruler");
            _clearBtn = new Button(270, 10, 120, 80, "Clear") { Color = Color.Azure };
            guiManagerComponent.GUIs.Add(_compassBtn);
            guiManagerComponent.GUIs.Add(_rulerBtn);
            guiManagerComponent.GUIs.Add(_clearBtn);

            _shapes = new List<Shape>();
            _keypoints = new List<Vector2>();
        }

        private void UpdateDrawState()
        {
            if (_compassBtn.IsMouseUp)
                _drawState = DrawState.CIRCLE;
            if (_rulerBtn.IsMouseUp)
                _drawState = DrawState.LINE;
        }

        private void UpdateLists()
        {
            if(_clearBtn.IsMouseUp)
            {
                _shapes.Clear();
            }
            foreach(var s in _shapes)
            {
                s.Draw(_spriteBatch, 2, Color.Black);
            }
            foreach(var k in _keypoints)
            {
                GUI.DrawPoint(_spriteBatch, k, 2, Color.Red);
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

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if(_drawState == DrawState.CIRCLE || _drawState == DrawState.LINE)
                {
                    if (!_wasDrawing)
                    {
                        _lastPoint = Mouse.GetState().Position.ToVector2();
                        _wasDrawing = true;
                    }
                }
            }
            if(_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_drawState == DrawState.CIRCLE)
                {
                    float radius = Vector2.Distance(Mouse.GetState().Position.ToVector2(), _lastPoint);
                    AddShape(new Circle(_lastPoint, radius));
                }
                else if (_drawState == DrawState.LINE)
                {
                    AddShape(new Line(_lastPoint, Mouse.GetState().Position.ToVector2()));
                }
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }

            UpdateDrawState();
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (_wasDrawing && _drawState == DrawState.CIRCLE)
            {
                float radius = (Mouse.GetState().Position.ToVector2() - _lastPoint).Length();
                GUI.DrawCircle(_spriteBatch, _lastPoint, radius, 2, Color.DarkGray, 100);
            }
            else if(_wasDrawing && _drawState == DrawState.LINE)
            {
                GUI.DrawLine(_spriteBatch, _lastPoint, Mouse.GetState().Position.ToVector2(), 2, Color.DarkGray);
            }

            UpdateLists();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
