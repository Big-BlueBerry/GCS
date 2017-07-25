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
        private ConstructComponent _comp;
        private VScrollBar _vscroll;
        private HScrollBar _hscroll;

        public MoveConstructComponent(ConstructComponent comp, VScrollBar vscroll, HScrollBar hscroll)
        {
            _comp = comp;
            _vscroll = vscroll;
            _hscroll = hscroll;

            vscroll.Minimum = 0;
            vscroll.Maximum = comp.Size.Y - comp.Bound.Height;

            hscroll.Minimum = 0;
            hscroll.Maximum = comp.Size.X - comp.Bound.Width;

            _vscroll.Scroll += scroll_Scroll;
            _hscroll.Scroll += scroll_Scroll;
        }

        private void scroll_Scroll(object sender, ScrollEventArgs e)
        {
            _comp.Location = new Point(_hscroll.Value, _vscroll.Value);
        }
    }
}
