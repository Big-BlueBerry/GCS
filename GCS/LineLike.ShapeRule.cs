namespace GCS
{
    public partial class LineLike
    {
        public class LineLikeOnTwoDotsRule : ShapeRule
        {
            public LineLikeOnTwoDotsRule(LineLike line, Dot d1, Dot d2) : base(line)
            {
                d1.Childs.Add(line);
                d2.Childs.Add(line);
                line.Parents.Add(d1);
                line.Parents.Add(d2);

                Fix();
            }

            public override void OnMoved()
            {
                if (IsHandling) return;
                IsHandling = true;

                var line = Shape as LineLike;

                Shape.Parents[0].MoveTo(line.Point1);
                Shape.Parents[1].MoveTo(line.Point2);

                Fix(); // 부모 점이 도형에 의존하여 움직임이 규제된다면 다시 옮겨줘야 함

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
                var line = Shape as LineLike;
                line.Point1 = (line.Parents[0] as Dot).Coord;
                line.Point2 = (line.Parents[1] as Dot).Coord;
            }
        }
    }
}
