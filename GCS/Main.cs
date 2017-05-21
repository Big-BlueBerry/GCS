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
        private ConstructComponent _construct;

        protected override void InitSize()
        {
            base.InitSize();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GameObject con = new GameObject("construct");
            _construct = con.AddComponent<ConstructComponent>();
            Instantiate(con);

            GUIManager.DefaultFont = LoadContent<SpriteFont>("basicfont");
            _compassBtn = new Button(10, 10, 120, 80, "Compass");
            _rulerBtn = new Button(140, 10, 120, 80, "Ruler");
            _clearBtn = new Button(270, 10, 120, 80, "Clear") { Color = Color.Azure };
            guiManagerComponent.GUIs.Add(_compassBtn);
            guiManagerComponent.GUIs.Add(_rulerBtn);
            guiManagerComponent.GUIs.Add(_clearBtn);
        }

        private void UpdateDrawState()
        {
            if (_compassBtn.IsMouseUp)
                _construct.ChangeState(DrawState.CIRCLE);
            if (_rulerBtn.IsMouseUp)
                _construct.ChangeState(DrawState.LINE);
            if (_clearBtn.IsMouseUp)
                _construct.Clear();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateDrawState();
        }
    }
}
