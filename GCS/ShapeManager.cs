using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCS
{
    public class ShapeManager
    {
        private List<Shape> _shapes;

        public ShapeManager()
        {
            _shapes = new List<Shape>();
        }

        public void DeleteShape(Shape shape)
        {
            if (!_shapes.Contains(shape)) return;

            _shapes.Remove(shape);
        }

        public void DeleteShapes(IEnumerable<Shape> shapes)
        {
            foreach (var s in shapes)
                _shapes.Remove(s);
        }
    }
}
