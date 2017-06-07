using System;
using Microsoft.Xna.Framework;

namespace GCS
{
    public interface IParentRule
    {
        Dot Dot { get; }
        event Action<Vector2> MoveTo;
        Vector2 FixedCoord(Vector2 original);
    }
}
