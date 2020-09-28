using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class Button : UIComponent
    {
        public string Name;
        public Rectangle Bounds
        {
            set
            {
                bounds = value;
                GetRectangle = value;
                bounds.Offset(bounds.Size.ToVector2() / -2);
            }
            get
            {
                return GetRectangle;
            }
        }
        public Text Text;
        public Color IdleColor;
        public Color HighlightColor;
        public float Layer;
        public SpriteFont TextFont = null;

        private Color ActiveColor;
        private Vector2 Origin;
        private Rectangle bounds;
        private Rectangle GetRectangle;

        public Button(string name)
        {
            bounds = Rectangle.Empty;
            GetRectangle = Rectangle.Empty;
            Bounds = new Rectangle(Setup.graphics.PreferredBackBufferWidth / 2, Setup.graphics.PreferredBackBufferHeight / 2, 60, 30);
            Origin = Vector2.One * 0.5f;
            Name = name;
            if(TextFont != null)
                Text = new Text("ButtonText") { text = "Text", Color = Color.Black, Font = TextFont };
            IdleColor = Color.White;
            HighlightColor = IdleColor * 0.4f;
            ActiveColor = IdleColor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            HitBoxDebuger.DrawRectangle(GetRectangle, ActiveColor, 0, HitBoxDebuger._textureFilled, Layer, Origin);
            if (Text != null)
            {
                Text.Position = Bounds.Location.ToVector2();
                Text.Draw(spriteBatch);
            }
        }

        public string GetName()
        {
            return Name;
        }

        public void Update(GameTime gameTime)
        {
            if (IsCursorInRange())
                ActiveColor = HighlightColor;
            else
                ActiveColor = IdleColor;
        }

        public bool IsCursorInRange()
        {
            return bounds.Contains(Input.GetMousePosition());
        }

        public bool ClickedOnButton()
        {
            if (Input.GetMouseClickUp(MouseButtons.LeftClick))
                return IsCursorInRange();

            return false;
        }
    }
}
