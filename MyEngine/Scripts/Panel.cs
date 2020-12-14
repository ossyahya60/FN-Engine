using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public class Panel: GameObjectComponent
    {
        public Vector2 Origin;
        public string Name;
        public Color Color;
        public Point Size;

        private Transform Transform;
        private Texture2D Texture;
        private Rectangle Bounds;

        public Panel(string name)
        {
            Name = name;
            Origin = new Vector2(0.5f, 0.5f);
            Size = new Point(600, 200);
            Bounds = new Rectangle();
            Texture = HitBoxDebuger._textureFilled;
            Color = Color.Black;
        }

        static Panel()
        {
            LayerUI.AddLayer("Panels", 0);
        }

        public override void Start()
        {
            gameObject.Layer = LayerUI.GetLayer("Panels");
            Transform = gameObject.Transform;
            if(Transform.Position == Vector2.Zero)
                Transform.Position = new Vector2(Setup.graphics.PreferredBackBufferWidth / 2, Setup.graphics.PreferredBackBufferHeight / 2);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Bounds.Location = Transform.Position.ToPoint();
            Bounds.Size = Size * Transform.Scale.ToPoint();

            HitBoxDebuger.DrawRectangle(Bounds, Color, 0, Texture, gameObject.Layer, Origin);
        }

        public void FillTheScreen()
        {
            Size.X = Setup.graphics.PreferredBackBufferWidth;
            Size.Y = Setup.graphics.PreferredBackBufferHeight;
        }
    }
}
