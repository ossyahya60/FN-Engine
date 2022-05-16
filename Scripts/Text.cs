using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FN_Engine
{
    public class Text : GameObjectComponent
    {
        public string Name = "Default";
        public Color Color;
        public string text;
        public SpriteEffects spriteEffects;
        public Vector2 Origin;
        public bool CustomOrigin = false;

        private SpriteFont Font; //?


        public Text()
        {
            text = "Text";
            Origin = Vector2.Zero;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
        }

        public Text(string name)
        {
            text = "Text";
            Origin = Vector2.Zero;
            Name = name;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
        }

        public Text(string name, SpriteFont font)
        {
            text = "Text";
            Origin = Vector2.Zero;
            Name = name;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
            Font = font;
        }

        static Text()
        {
            LayerUI.AddLayer("Text", 10);
        }

        public override void Start()
        {
            gameObject.Layer = LayerUI.GetLayer("Text");

            if (Font == null)
                LoadFont("Font");

            //transform.Position = new Vector2(Setup.graphics.PreferredBackBufferWidth / 2, Setup.graphics.PreferredBackBufferHeight / 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Font == null) //handle font serialization
                LoadFont("Font");

            if (!CustomOrigin)
                Origin = Font.MeasureString(text) * 0.5f;

            spriteBatch.DrawString(Font, text, gameObject.Transform.Position, Color, MathHelper.ToRadians(gameObject.Transform.Rotation), Origin, gameObject.Transform.Scale, spriteEffects, gameObject.Layer);
        }

        public void LoadFont(string Name)
        {
            try { Font = Setup.Content.Load<SpriteFont>(Name); }
            catch (Exception E) { Utility.Log(E.Message); }
        }

        public string GetName()
        {
            return Name;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            Text clone = this.MemberwiseClone() as Text;
            clone.gameObject = Clone;
            Clone.Layer = LayerUI.GetLayer("Text");

            return clone;
        }
    }
}
