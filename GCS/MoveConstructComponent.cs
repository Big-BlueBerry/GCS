using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grid.Framework;
using Microsoft.Xna.Framework;

namespace GCS
{
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
    }
}
