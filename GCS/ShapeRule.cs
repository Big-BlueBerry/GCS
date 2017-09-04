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

        public virtual void Detach()
        {
            foreach(var p in Shape.Parents)
            {
                p.Childs.Remove(Shape);
            }

            Shape.Parents.Clear();
            Shape._rule = null;
        }

        public void MoveChilds()
        {
            foreach (var c in Shape.Childs)
                c._rule.OnParentMoved();
        }
    }

    public class ReflectedShapeRule : ShapeRule
    {
        public ReflectedShapeRule(LineLike axis, Shape original, Shape shape) : base(shape)
        {
            shape.Parents.Add(axis);
            shape.Parents.Add(original);

            axis.Childs.Add(shape);
            original.Childs.Add(shape);

            Fix();
        }

        public override void OnMoved()
        {
            if (IsHandling) return;
            IsHandling = true;

            FixParent();
            MoveChilds();

            IsHandling = false;
        }

        public override void OnParentMoved()
        {
            if (IsHandling) return;
            Fix();
            MoveChilds();
        }

        protected void FixParent()
        {
            var axis = Shape.Parents[0] as LineLike;
            var original = Shape.Parents[1];

            int i = 0;
            foreach (var dot in Shape.DotList)
            {
                original.DotList[i] = Geometry.Reflect(dot, axis);
                i++;
            }
        }

        protected override void Fix()
        {
            var axis = Shape.Parents[0] as LineLike;
            var original = Shape.Parents[1];

            int i = 0;
            foreach (var dot in original.DotList)
            {
                Shape.DotList[i] = Geometry.Reflect(dot, axis);
                i++;
            }
        }
    }
}
