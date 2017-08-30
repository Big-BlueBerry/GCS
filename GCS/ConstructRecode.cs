using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GCS
{
    class ConstructRecode
    {
        public List<Shape> targetshapes;
        public Vector2 moveRecode;
        public RecodeType type;

        public ConstructRecode(RecodeType Type, List<Shape> Target)
        {
            targetshapes = Target;
            type = Type;
        }
        
        public void WriteMoveRecode(Vector2 diff)
        {
            moveRecode = -diff;
        }
    }
}
