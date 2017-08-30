using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.targetshapes = Target;
            this.type = Type;
        }
        
        public void WriteMoveRecode(Vector2 diff)
        {
            this.moveRecode = -diff;
        }
    }
}
