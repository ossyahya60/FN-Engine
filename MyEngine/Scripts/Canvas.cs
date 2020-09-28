using System.Collections.Generic;
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
            Transform = gameObject.Transform;
            if (Transform == null)
                gameObject.AddComponent<Transform>(new Transform());
            Camera = camera;
            DestinationRectangle = new Rectangle(0, 0, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight);
        }

        public override void Start()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            DestinationRectangle.Size = DestinationRectangle.Size * Transform.Scale.ToPoint();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}
