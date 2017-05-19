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
        private Point _lastPoint = new Point();
        private List<(Point center, float radius)> _circles;
        private List<(Point p1, Point p2)> _lines;

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

            _circles = new List<(Point, float)>();
            _lines = new List<(Point p1, Point p2)>();
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
                _circles.Clear();
                _lines.Clear();
            }
            foreach(var c in _circles)
            {
                GUI.DrawCircle(_spriteBatch, c.center, c.radius, 2, Color.Black, 100);
            }
            foreach(var l in _lines)
            {
                GUI.DrawLine(_spriteBatch, l.p1, l.p2, 2, Color.Black);
            }
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
                        _lastPoint = Mouse.GetState().Position;
                        _wasDrawing = true;
                    }
                }
            }
            if(_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_drawState == DrawState.CIRCLE)
                {
                    float radius = Vector2.Distance(Mouse.GetState().Position.ToVector2(), _lastPoint.ToVector2());
                    _circles.Add((_lastPoint, radius));
                }
                else if (_drawState == DrawState.LINE)
                {
                    _lines.Add((_lastPoint, Mouse.GetState().Position));
                }
                _wasDrawing = false;
                _drawState = DrawState.NONE;
            }

            UpdateDrawState();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _spriteBatch.Begin();
            if (_wasDrawing && _drawState == DrawState.CIRCLE)
            {
                float radius = (Mouse.GetState().Position.ToVector2() - _lastPoint.ToVector2()).Length();
                GUI.DrawCircle(_spriteBatch, _lastPoint, radius, 2, Color.DarkGray, 100);
            }
            else if(_wasDrawing && _drawState == DrawState.LINE)
            {
                GUI.DrawLine(_spriteBatch, _lastPoint, Mouse.GetState().Position, 2, Color.DarkGray);
            }

            UpdateLists();
            _spriteBatch.End();
        }
    }
}
