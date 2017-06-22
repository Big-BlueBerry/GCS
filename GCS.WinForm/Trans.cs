using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace GCS.WinForm
{
    public static class Trans
    {
        public static Microsoft.Xna.Framework.Point ToXNA(this Point point)
            => new Microsoft.Xna.Framework.Point(point.X, point.Y);

        public static Microsoft.Xna.Framework.Vector2 ToVector2(this Point point)
                    => new Microsoft.Xna.Framework.Vector2(point.X, point.Y);
    }
}
