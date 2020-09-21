using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public interface UIComponent
    {
        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);

        string GetName();
    }
}
