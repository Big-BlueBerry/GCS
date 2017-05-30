using Microsoft.Xna.Framework.Graphics;

namespace GCS
{
    public static class SpriteBatchAntiAliasing
    {
        public static void BeginAA(this SpriteBatch sb)
        {
            var state = new RasterizerState { MultiSampleAntiAlias = true };
            sb.Begin(rasterizerState: state);
        }
    }
}
