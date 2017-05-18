using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grid.Framework;

namespace GCS
{
    public class Main : Scene
    {
        protected override void InitSize()
        {
            base.InitSize();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }
    }
}
