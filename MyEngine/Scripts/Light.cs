using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public enum LightTypes { Point, Spot, Directional}

    public class Light: GameObjectComponent
    {
        private static bool ShaderLoaded = false;
        private static int MAX_LIGHT_COUNT = 8;
        private static List<Light> LIGHTS;
        private static RenderTarget2D RenderTarget2D;
        private static Effect LightEffect;

        public LightTypes Type = LightTypes.Point;
        public Color color = Color.White;
        public float AngularRadius = 360;
        public float OuterRadius = 0.25f; //This is less intense than the Inner one
        public float InnerRadius = 0.025f; //10% of OuterRadius
        public float InnerInensity = 1.5f; // 1 is the same as the outer radius Intensity
        public float Attenuation = 1;

        private Transform Transform;
        private float YOVERX;

        public override void Start()
        {
            if (!ShaderLoaded || LightEffect.IsDisposed)
            {
                RenderTarget2D = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                LIGHTS = new List<Light>();
                LightEffect = Setup.Content.Load<Effect>("LightTest");
                ShaderLoaded = true;
            }

            LIGHTS.Add(this);
            Transform = gameObject.Transform;
            YOVERX = (float)Setup.graphics.PreferredBackBufferHeight / Setup.graphics.PreferredBackBufferWidth;
        }

        public override void Destroy()
        {
            LIGHTS.Remove(this);
        }

        public static void Init_Light()
        {
            if(LIGHTS != null && LIGHTS.Count != 0)
                Setup.GraphicsDevice.SetRenderTarget(RenderTarget2D); //Render Target
        }

        public static void ApplyLighting()
        {
            if (LIGHTS == null)
                return;

            int LightCount = LIGHTS.Count;

            if (LightCount == 0)
                return;

            Vector3[] COLOR = new Vector3[LightCount];
            float[] InnerRadius = new float[LightCount];
            float[] Radius = new float[LightCount];
            float[] Attenuation = new float[LightCount];
            float[] X_Bias = new float[LightCount];
            float[] Y_Bias = new float[LightCount];
            float[] AngularRadius = new float[LightCount];
            float[] InnerIntensity = new float[LightCount];

            for (int i = 0; i < LightCount; i++)
            {
                COLOR[i] = LIGHTS[i].color.ToVector3();
                InnerRadius[i] = LIGHTS[i].InnerRadius;
                Radius[i] = LIGHTS[i].OuterRadius;
                Attenuation[i] = LIGHTS[i].Attenuation;
                X_Bias[i] = (LIGHTS[i].Transform.Position.X - Setup.graphics.PreferredBackBufferWidth * 0.5f) / Setup.graphics.PreferredBackBufferWidth;
                Y_Bias[i] = (LIGHTS[i].Transform.Position.Y - Setup.graphics.PreferredBackBufferHeight * 0.5f) / Setup.graphics.PreferredBackBufferHeight;
                AngularRadius[i] = LIGHTS[i].AngularRadius;
                InnerIntensity[i] = LIGHTS[i].InnerInensity;
            }

            LightEffect.Parameters["LightCount"].SetValue(LightCount);
            LightEffect.Parameters["AngularRadius"].SetValue(AngularRadius);
            LightEffect.Parameters["X_Bias"].SetValue(X_Bias);
            LightEffect.Parameters["Y_Bias"].SetValue(Y_Bias);
            LightEffect.Parameters["YOverX"].SetValue(LIGHTS[0].YOVERX);
            LightEffect.Parameters["Color"].SetValue(COLOR);
            LightEffect.Parameters["Radius"].SetValue(Radius);
            LightEffect.Parameters["InnerRadius"].SetValue(InnerRadius);
            LightEffect.Parameters["InnerIntensity"].SetValue(InnerIntensity);
            LightEffect.Parameters["Attenuation"].SetValue(Attenuation);

            Setup.GraphicsDevice.SetRenderTarget(null);
            Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, LightEffect, Setup.Camera.GetViewTransformationMatrix());
            Setup.spriteBatch.Draw(RenderTarget2D, new Vector2(0, 0), new Rectangle(0, 0, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight), Color.White);
            Setup.spriteBatch.End();
        }
    }
}
