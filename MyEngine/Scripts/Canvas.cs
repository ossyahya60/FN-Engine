using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class Canvas: GameObjectComponent
    {
        public bool ScreenSpace = true; // false is world
        public Rectangle DestinationRectangle;

        private Camera2D Camera;
        private Transform Transform;

        public Canvas(Camera2D camera)
        {
            Camera = camera;
        }

        public override void Start()
        {
            Transform = gameObject.Transform;
            DestinationRectangle = new Rectangle(0, 0, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight);
        }

        public override void Update(GameTime gameTime)
        {
            DestinationRectangle.Location = Transform.Position.ToPoint();
            DestinationRectangle.Size = DestinationRectangle.Size * Transform.Scale.ToPoint();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}
