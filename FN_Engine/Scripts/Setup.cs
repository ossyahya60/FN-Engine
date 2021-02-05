using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FN_Engine
{
    public class Setup
    {
        public static GraphicsDeviceManager graphics { get; private set; }
        public static ContentManager Content { get; private set; }
        public static GraphicsDevice GraphicsDevice { get; private set; }
        public static SpriteBatch spriteBatch { get; private set; }
        public static ResolutionIndependentRenderer resolutionIndependentRenderer { get; private set; }
        public static GameWindow GameWindow { get; private set; }
        public static Camera2D Camera { get; private set; }
        public static Game Game { get; private set; }

    public static void Initialize(GraphicsDeviceManager GDM, ContentManager CM, SpriteBatch SB, ResolutionIndependentRenderer RIR, GameWindow GW, Camera2D camera, Game game)
        {
            graphics = GDM;
            Content = CM;
            GraphicsDevice = GDM.GraphicsDevice;
            spriteBatch = SB;
            resolutionIndependentRenderer = RIR;
            GameWindow = GW;
            Camera = camera;
            Game = game;
        }
    }
}
