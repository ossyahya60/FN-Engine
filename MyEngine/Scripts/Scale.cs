using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public static class Scale //Suspended for now
    {
        public static Texture2D ScaleUp(Texture2D texture, int Factor)
        {
            Texture2D ScaledTexture = new Texture2D(Setup.GraphicsDevice, texture.Width * Factor, texture.Height * Factor);
            Color[] NewData = new Color[texture.Width * Factor * texture.Height * Factor];
            Color[] OldData = new Color[texture.Width* texture.Height];
            texture.GetData(OldData);

            for (int i=0; i< ScaledTexture.Height; i++)
                for (int j=0; j< ScaledTexture.Width; j++)
                    NewData[i * texture.Width + j] = OldData[i * texture.Width + j];

            ScaledTexture.SetData(NewData);

            return ScaledTexture;
        }
    }
}
