using System;

namespace GCS
{
    public partial class Dot
    {
        public class EmptyDotRule : ShapeRule
        {
            public EmptyDotRule(Dot dot) : base(dot) { }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                throw new NotSupportedException("부모도 없는게..");
            }

            protected override void Fix()
            {
                // Do nothing. Just like you.
            }
        }

        public class DotOnShapeRule : ShapeRule
        {
            public DotOnShapeRule(Dot dot, Shape parent) : base(dot)
            {
                dot.Parents.Add(parent);
                parent.Childs.Add(dot);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                Fix();

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();

                IsHandling = false;
            }

            public override void OnParentMoved()
            {
                if (IsHandling) return;
                Fix();

                foreach (var c in Shape.Childs)
                    c._rule.OnParentMoved();
            }

            protected override void Fix()
            {
                throw new WorkWoorimException("빨리 이전 룰 가져와라!!!");
            }
        }
    }
}
