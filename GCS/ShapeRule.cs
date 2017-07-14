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
        public class LineTransformBuilder : ShapeTransformBuilder
        {
            public override Shape Parallel(Shape shape, float distance, float angle)
            {
                throw new WorkWoorimException("라인을 리턴함");
            }
        }
    }
}
