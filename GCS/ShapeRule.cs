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
            foreach(Shape p in Shape.Parents)
            {
                p.Childs.Remove(Shape);
            }

            Shape.Parents.Clear();
            Shape._rule = null;
        }

        public void MoveChilds()
        {
            foreach (Shape c in Shape.Childs)
                c._rule.OnParentMoved();
        }
    }
}
