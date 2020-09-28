using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public class Panel
    {
        public Vector2 Origin;
        public Vector2 Position;
        public string Name;
        public Color Color;

        public Vector2 Size
        {
            set
            {
                size = value;
            }
            get
            {
                return size;
            }
        }

        private Vector2 size;
        private Texture2D Texture;
        private List<UIComponent> UIcomponents;
        private Rectangle Bounds;
        private float Layer = 0.1f;

        public void AddUIComponent(UIComponent component)
        {
            if (component != null)
                UIcomponents.Add(component);
        }

        public T GetUIComponent<T>(string Name) where T: UIComponent
        {
            foreach (UIComponent UIC in UIcomponents)
                if ((T)UIC != null)
                    if (UIC.GetName() == Name)
                        return (T)UIC;

            return default(T);
        }


        public Panel(string name)
        {
            Name = name;
            UIcomponents = new List<UIComponent>();
            size = new Vector2(600, 200);
            Origin = new Vector2(0.5f, 0.5f);
            Position = new Vector2(Setup.graphics.PreferredBackBufferWidth/2, Setup.graphics.PreferredBackBufferHeight / 2);
            Bounds = new Rectangle();
            Texture = HitBoxDebuger._textureFilled;
            Color = Color.Black;
        }

        public void Update(GameTime gameTime)
        {
            foreach (UIComponent UIC in UIcomponents)
                UIC.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Position.X = Setup.graphics.PreferredBackBufferWidth / 2;
            //Position.Y = Setup.graphics.PreferredBackBufferHeight / 2;

            Bounds.X = (int)Position.X;
            Bounds.Y = (int)Position.Y;
            Bounds.Width = (int)Size.X;
            Bounds.Height = (int)Size.Y;

            HitBoxDebuger.DrawRectangle(Bounds, Color, 0, Texture, Layer, Origin);

            foreach (UIComponent UIC in UIcomponents)
                UIC.Draw(spriteBatch);
        }
    }
}
