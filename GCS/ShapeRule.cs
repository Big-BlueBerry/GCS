using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCS
{
    public abstract class ShapeRule
    {
        public Shape Shape { get; }

        public ShapeRule(Shape shape)
        {
            Shape = shape;
        }

        public virtual void OnSelfMoved() { }
        public virtual void OnParentMoved() { }
        public virtual void OnChildMoved() { }
    }

    public abstract partial class Shape
    {
        public abstract class ShapeTransformBuilder
        {
            public abstract Shape Parallel(Shape shape, float distance, float angle);
        }
        public static ShapeTransformBuilder FromTransform;
    }

    public partial class Line
    {
        public static new LineTransformBuilder FromTransform;

        public class ParallelLineRule : ShapeRule
        {
            private float _distance, _angle;

            public ParallelLineRule(Line line, Line parent, float dist, float angle) : base(line)
            {
                _distance = dist;
                _angle = angle;

                line._rule = this;

                line.Parents.Clear();
                line.Parents[0] = parent;
            }

            public override void OnParentMoved()
            {
                base.OnParentMoved();
                throw new WorkWoorimException("자기 자신을 움직여야 함");
            }

            public override void OnSelfMoved()
            {
                base.OnSelfMoved();
                throw new WorkWoorimException("부모를 움직여야 함");
            }

            public override void OnChildMoved()
            {
                base.OnChildMoved();
                /*
                 * 여기가 엄청 헷갈리는데, 점이 선에 의존할 때 점이 움직였을 때 이 메서드가 호출이 안되지?
                 * 그 말은 이 메서드는 자식이 움직였을 때 자기 자신이 움직일 필요가 있을 때만 불러진다는 건가?
                 * 즉 자식이 어떻게 움직였는지 여기서 그걸 전부 신경써야 하나?? 어떤 경우가 있지? 다른 변환이동?
                 * 
                 * 제발 설계를 제대로 하고 코딩을 하자 빡대가리야..
                 */
            }
        }

        public class LineTransformBuilder : ShapeTransformBuilder
        {
            public override Shape Parallel(Shape shape, float distance, float angle)
            {
                throw new WorkWoorimException("라인을 리턴함");

                // 라인하르트! 대령했소이다
            }
        }
    }
}
