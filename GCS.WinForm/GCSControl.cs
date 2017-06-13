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

namespace GCS.WinForm
{
    class GCSControl : GraphicsDeviceControl
    {
        protected override void Initialize()
        {
        }
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }
    }
}
