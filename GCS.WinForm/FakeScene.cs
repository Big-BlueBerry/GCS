using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Grid.Framework;

namespace GCS.WinForm
{
    public class FakeScene : Scene
    {
        public new GraphicsDevice GraphicsDevice;
        public FakeScene()
        {
            CurrentScene = this;
        }
    }
}
