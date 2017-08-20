using System.Linq;
using System.Collections.Generic;
using Grid.Framework;
using Grid.Framework.Components;
using Grid.Framework.GUIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;

namespace GCS
{
    class GeometrytestComponent : Renderable
    {
        Ellipse elp = Ellipse.FromThreeDots(Dot.FromCoord(500, 500), Dot.FromCoord(400, 600), Dot.FromCoord(300,300));
        
    }
}
