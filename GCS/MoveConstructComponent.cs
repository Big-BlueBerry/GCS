using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grid.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GCS
{
    using Keys = Microsoft.Xna.Framework.Input.Keys;

    public class MoveConstructComponent : Component
    {
        public ConstructComponent Comp;
        private VScrollBar _vscroll;
        private HScrollBar _hscroll;

        public VScrollBar Vscroll
        {
            get => _vscroll;
            set
            {
                if (_vscroll != null)
                    _vscroll.Scroll -= scroll_Scroll;
                _vscroll = value;
                _vscroll.Scroll += scroll_Scroll;
                value.Minimum = 0;
                value.Maximum = (Comp.Size.Y - Comp.Bound.Height) / 5;
            }
        }

        public HScrollBar Hscroll
        {
            get => _hscroll;
            set
            {
                if (_hscroll != null)
                    _hscroll.Scroll -= scroll_Scroll;
                _hscroll = value;
                _hscroll.Scroll += scroll_Scroll;
                value.Minimum = 0;
                value.Maximum = (Comp.Size.X - Comp.Bound.Width) / 5;
            }
        }
        

        public MoveConstructComponent()
        {

        }

        public override void Start()
        {
            base.Start();
            _vscroll.Value = _vscroll.Maximum / 2;
            _hscroll.Value = _hscroll.Maximum / 2;

            scroll_Scroll(null, null);
        }

        private void scroll_Scroll(object sender, ScrollEventArgs e)
        {
            Comp.Location = new Vector2(_hscroll.Value * 5, _vscroll.Value * 5);
        }

        public void Scroll(int x, int y)
        {
            _hscroll.Value = MathHelper.Clamp(_hscroll.Value + x, _hscroll.Minimum, _hscroll.Maximum);
            _vscroll.Value = MathHelper.Clamp(_vscroll.Value + y, _vscroll.Minimum, _vscroll.Maximum);
            scroll_Scroll(null, null);
        }

        public override void Update()
        {
            base.Update();

            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Down))
                Scroll(0, 5);
            if (state.IsKeyDown(Keys.Up))
                Scroll(0, -5);
            if (state.IsKeyDown(Keys.Left))
                Scroll(-5, 0);
            if (state.IsKeyDown(Keys.Right))
                Scroll(5, 0);
        }
    }
}
