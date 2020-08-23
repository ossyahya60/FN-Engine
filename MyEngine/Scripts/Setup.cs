using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class Setup
    {
        public static GraphicsDeviceManager graphics { get; private set; }
        public static ContentManager Content { get; private set; }

        public static void Initialize(GraphicsDeviceManager GDM, ContentManager CM)
        {
            graphics = GDM;
            Content = CM;
        }
    }
}
