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
        private Button _compassBtn, _rulerBtn;
        private DrawState _drawState = DrawState.NONE;
        private bool _wasDrawing = false;
        private Point _lastPoint = new Point();
        private List<(Point center, float radius)> _circles;
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
            guiManagerComponent.GUIs.Add(_compassBtn);
            guiManagerComponent.GUIs.Add(_rulerBtn);

            _circles = new List<(Point, float)>();
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
            foreach(var c in _circles)
            {
                GUI.DrawCircle(_spriteBatch, c.center, c.radius, 2f, Color.Black, 100);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if(_drawState == DrawState.CIRCLE)
                {
                    if (!_wasDrawing)
                    {
                        _lastPoint = Mouse.GetState().Position;
                        _wasDrawing = true;
                    }

                }
                else if(_drawState == DrawState.LINE)
                {
                    _wasDrawing = true;
                }
            }
            if(_wasDrawing && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                float radius = Vector2.Distance(Mouse.GetState().Position.ToVector2(), _lastPoint.ToVector2());
                _wasDrawing = false;
                _circles.Add((_lastPoint, radius));
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

            UpdateLists();
            _spriteBatch.End();
        }
    }
}
