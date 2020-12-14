using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MyEngine
{
    public class Button : GameObjectComponent
    {
        public Point Size;
        public Color IdleColor;
        public Color HighlightColor;

        private Transform Transform;
        private Rectangle Bounds;
        private Color ActiveColor;
        private Vector2 Origin;
        private Rectangle ActualHitBox;

        public Button()
        {
            
        }

        static Button()
        {
            LayerUI.AddLayer("Buttons", 5);
        }

        public override void Start()
        {
            Size = new Point(60, 30);
            Transform = gameObject.Transform;
            gameObject.Layer = LayerUI.GetLayer("Buttons");
            Bounds = new Rectangle(Transform.Position.ToPoint(), Size);
            Origin = Vector2.One * 0.5f;
            IdleColor = Color.White;
            HighlightColor = IdleColor * 0.4f;
            ActiveColor = IdleColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Bounds.Location = Transform.Position.ToPoint();
            Bounds.Size = Size * Transform.Scale.ToPoint();
            HitBoxDebuger.DrawRectangle(Bounds, ActiveColor, 0, HitBoxDebuger._textureFilled, gameObject.Layer, Origin);
        }

        public override void Update(GameTime gameTime)
        {
            if (!TouchPanel.GetState().IsConnected)
            {
                if (IsCursorInRange())
                    ActiveColor = HighlightColor;
                else
                    ActiveColor = IdleColor;
            }
            else
                ActiveColor = IdleColor;
        }

        public bool IsCursorInRange()
        {
            if (!gameObject.Active)
                return false;

            Bounds.Location = Transform.Position.ToPoint();
            Bounds.Size = Size * Transform.Scale.ToPoint();
            ActualHitBox = Bounds;
            ActualHitBox.Offset(-Bounds.Size.ToVector2() * 0.5f);

            if (!TouchPanel.GetState().IsConnected)
                return ActualHitBox.Contains(Input.GetMousePosition());
            else
                return ActualHitBox.Contains(Setup.resolutionIndependentRenderer.ScaleMouseToScreenCoordinates(TouchPanel.GetState()[0].Position));
        }

        public bool ClickedOnButton()
        {
            if (!gameObject.Active)
                return false;

            try
            {
                if (TouchPanel.GetState().IsConnected)
                    if (Input.TouchDown())
                        return IsCursorInRange();
            }
            catch { }

            if (Input.GetMouseClickUp(MouseButtons.LeftClick))
                return IsCursorInRange();

            return false;
        }
    }
}
