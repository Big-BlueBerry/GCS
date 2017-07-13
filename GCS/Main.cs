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
        private Button _compassBtn, _segmentBtn, _lineBtn, _dotBtn, _vectorBtn, _clearBtn, _deleteBtn, _undoBtn;
        private ConstructComponent _construct;
        protected override void Initialize()
        {
            base.Initialize();
            // 윈도우 리사이즈는 모노게임자체에 버그가 있다고 함
            /*
            Window.ClientSizeChanged += (s, e) =>
            {
                _windowResized = !_windowResized;
                if (_windowResized)
                {
                    _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                    _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                    _graphics.ApplyChanges();
                }
            };
            Window.AllowUserResizing = true;
            */
        }
        protected override void InitSize()
        {
            base.InitSize();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            _graphics.PreferMultiSampling = true;
            GameObject con = new GameObject("construct");
            _construct = con.AddComponent<ConstructComponent>();
            _construct.Enabled = true;
            Instantiate(con);

            Resources.LoadAll();

            _compassBtn = new ImageButton(10, 10, 80, 80, LoadContent<Texture2D>("icon\\circle"));
            _segmentBtn = new ImageButton(100, 10, 80, 80, LoadContent<Texture2D>("icon\\segment"));
            _vectorBtn = new ImageButton(190, 10, 80, 80, LoadContent<Texture2D>("icon\\vector"));
            _lineBtn = new ImageButton(280, 10, 80, 80, LoadContent<Texture2D>("icon\\line"));
            _dotBtn = new ImageButton(370, 10, 80, 80, LoadContent<Texture2D>("icon\\dot"));
            _clearBtn = new Button(460, 10, 80, 80, "Clear") { BackColor = Color.Azure };
            _deleteBtn = new Button(550, 10, 80, 80, "Delete") { BackColor = Color.Honeydew};
            _undoBtn = new Button(640, 10, 80, 80, "Undo") { BackColor = Color.Violet};
            guiManagerComponent.GUIs.Add(_compassBtn);
            guiManagerComponent.GUIs.Add(_segmentBtn);
            guiManagerComponent.GUIs.Add(_vectorBtn);
            guiManagerComponent.GUIs.Add(_lineBtn);
            guiManagerComponent.GUIs.Add(_dotBtn);
            guiManagerComponent.GUIs.Add(_clearBtn);
            guiManagerComponent.GUIs.Add(_deleteBtn);
            guiManagerComponent.GUIs.Add(_undoBtn);

            MainCamera.AddComponent<Grid.Framework.Components.Movable2DCamera>();

            GameObject test = new GameObject("test");
            test.AddComponent<GeometryTestComponent>();
            test.Enabled = false;
            Instantiate(test);

        }

        private void UpdateDrawState()
        {
            if (_compassBtn.IsMouseUp)
                _construct.ChangeState(DrawState.CIRCLE);
            if (_segmentBtn.IsMouseUp)
                _construct.ChangeState(DrawState.SEGMENT);
            if (_lineBtn.IsMouseUp)
                _construct.ChangeState(DrawState.LINE);
            if (_dotBtn.IsMouseUp)
                _construct.ChangeState(DrawState.DOT);
            if (_vectorBtn.IsMouseUp)
                _construct.ChangeState(DrawState.VECTOR);
            if (_clearBtn.IsMouseUp)
                _construct.Clear();
            if (_deleteBtn.IsMouseUp)
                _construct.DeleteSelected();
            if (_undoBtn.IsMouseUp)
                _construct.Undo();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateDrawState();
        }
    }
}
