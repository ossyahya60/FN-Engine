using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class Canvas: GameObjectComponent
    {
        public ResolutionIndependentRenderer RIR;

        private List<Panel> Panels;
        private Vector2 Size;
        private Vector2 OriginalSize;

        public Canvas(ResolutionIndependentRenderer RIR)
        {
            this.RIR = RIR;
            Panels = new List<Panel>();
            Size = Vector2.Zero;
            OriginalSize = new Vector2(RIR.ScreenWidth, RIR.ScreenHeight);
        }

        public void AddPanel(Panel panel)
        {
            Panels.Add(panel);
        }

        public Panel GetPanel(string Name)
        {
            foreach (Panel P in Panels)
                if (P.Name == Name)
                    return P;

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            Size.X = RIR.ScreenWidth;
            Size.Y = RIR.ScreenHeight;

            foreach (Panel P in Panels)
            {
                P.Size += Size - OriginalSize; //to scale UI components properly on canvas scaling
                P.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Panel P in Panels)
                P.Draw(spriteBatch);
        }
    }
}
