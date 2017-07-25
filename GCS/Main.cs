using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grid.Framework;
using Grid.Framework.GUIs;

using Button = Grid.Framework.GUIs.Button;
using MenuStrip = System.Windows.Forms.MenuStrip;
using Color = Microsoft.Xna.Framework.Color;

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
            //BackColor = new Color(133, 182, 203);
            BackColor = new Color(221, 255, 221);
            BackColor = Color.White;
            Debugger.Color = Color.Black;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            _graphics.PreferMultiSampling = true;
            GameObject con = new GameObject("construct");
            Instantiate(con);
            _construct = con.AddComponent<ConstructComponent>();
            _construct.Enabled = true;

            Resources.LoadAll();

            /*
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
            */
            InitGUI();
            MainCamera.AddComponent<Grid.Framework.Components.Movable2DCamera>();

            //GameObject test = new GameObject("test");
            //test.AddComponent<GeometryTestComponent>();
            //test.Enabled = false;
            //Instantiate(test);

        }
        
        private void InitGUI()
        {
            MenuStrip shapeStrip = new MenuStrip()
            {
                BackColor = System.Drawing.Color.White,
                Dock = DockStyle.Left
            };

            AddStripItem(shapeStrip, @"D:\Image\design\GCS\circle.png").Click += (b, d) => _construct.ChangeState(DrawState.CIRCLE);
            AddStripItem(shapeStrip, @"D:\Image\design\GCS\segment.png").Click += (b, d) => _construct.ChangeState(DrawState.SEGMENT);
            AddStripItem(shapeStrip, @"D:\Image\design\GCS\line.png").Click += (b, d) => _construct.ChangeState(DrawState.LINE);
            AddStripItem(shapeStrip, @"D:\Image\design\GCS\vector.png").Click += (b, d) => _construct.ChangeState(DrawState.VECTOR);
            AddStripItem(shapeStrip, @"D:\Image\design\GCS\dot.png").Click += (b, d) => _construct.ChangeState(DrawState.DOT);

            VScrollBar vscroll = new VScrollBar();
            vscroll.Dock = DockStyle.Right;
            HScrollBar hscroll = new HScrollBar();
            hscroll.Dock = DockStyle.Bottom;

            var construct = GameObject.Find("construct");

            var move = construct.AddComponent<MoveConstructComponent>();
            move.Comp = construct.GetComponent<ConstructComponent>();
            move.Hscroll = hscroll;
            move.Vscroll = vscroll;

            var menu = new MenuStrip()
            {
                BackColor = System.Drawing.Color.White
            };

            menu.Items.Add("파일(&F)");
            menu.Items.Add("편집(&E)");
            menu.Items.Add("보기(&D)");
            menu.Items.Add("작도(&C)");

            var control = Control.FromHandle(Window.Handle);
            control.Controls.Add(shapeStrip);
            control.Controls.Add(menu);
            control.Controls.Add(vscroll);
            control.Controls.Add(hscroll);
        }

        private ToolStripMenuItem AddStripItem(MenuStrip strip, string imgPath)
        {
            var imageSize = new Size(60, 60);
            var item = new ToolStripMenuItem(new Bitmap(Image.FromFile(imgPath), imageSize));
            item.ImageAlign = ContentAlignment.MiddleCenter;
            item.ImageScaling = ToolStripItemImageScaling.None;
            item.AutoSize = true;
            strip.Items.Add(item);
            return item;
        }

        private void UpdateDrawState()
        {
            /*
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
                */
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateDrawState();
        }
    }
}
