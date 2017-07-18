using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GCS
{
    public abstract class ShapeRule
    {
        public Shape Shape { get; }
        protected bool IsHandling = false;

        public ShapeRule(Shape shape)
        {
            Shape = shape;
            shape._rule = this;
        }

        protected abstract void Fix();

        public abstract void OnMoved();
        public abstract void OnParentMoved();
    }
}
