using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsGraphicsDevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GCS;
using Grid.Framework;
using System.Windows.Forms;

namespace GCS.WinForm
{
    public class GCSControl : GraphicsDeviceControl
    {
        private SpriteBatch _spriteBatch;
        private FakeScene _fakeScene;

        public DrawState DrawState { get; set; } = DrawState.NONE;
        private Dot _firstDot;
        private Dot _secondDot;
        private bool _isDrawing = false;

        private List<Shape> _shapes;

        protected override void Initialize()
        {
            _shapes = new List<Shape>();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _fakeScene = new FakeScene();
            _fakeScene.GraphicsDevice = this.GraphicsDevice;
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            if(_isDrawing)
            {
                if (DrawState == DrawState.CIRCLE)
                {
                    float radius = (_secondDot.Coord - _firstDot.Coord).Length();
                    GUI.DrawCircle(_spriteBatch, _firstDot.Coord, radius, 2, Color.DarkGray, 100);
                }
                else if (DrawState == DrawState.SEGMENT)
                {
                    GUI.DrawLine(_spriteBatch, _firstDot.Coord, _secondDot.Coord, 2, Color.DarkGray);
                }
                else if (DrawState == DrawState.LINE)
                {
                    new Line(new Dot(_firstDot.Coord), new Dot(_secondDot.Coord)).Draw(_spriteBatch);
                }
                else if (DrawState == DrawState.DOT)
                {
                    _firstDot.Draw(_spriteBatch);
                }
            }
            
            foreach (var s in _shapes) s.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if(DrawState != DrawState.NONE)
            {
                _firstDot = GetDot(e.Location.ToVector2());
                _isDrawing = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (DrawState == DrawState.DOT)
                    _firstDot.MoveTo(e.Location.ToVector2());
                else if (DrawState != DrawState.NONE)
                    _secondDot = GetDot(e.Location.ToVector2());

                this.Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DrawState != DrawState.NONE) _isDrawing = false;
            if (DrawState == DrawState.DOT)
                AddShape(_firstDot);
            else if (DrawState == DrawState.CIRCLE)
                AddShape(new Circle(_firstDot, _secondDot));
            else if (DrawState == DrawState.SEGMENT)
                AddShape(new Segment(_firstDot, _secondDot));
            else if (DrawState == DrawState.LINE)
                AddShape(new Line(_firstDot, _secondDot));

            this.Invalidate();
            base.OnMouseUp(e);
        }

        private Dot GetDot(Vector2 coord)
        {
            return new Dot(coord);
        }

        private void AddShape(Shape shape)
        {
            _shapes.Add(shape);
        }
    }
}
