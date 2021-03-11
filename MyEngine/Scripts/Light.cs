using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MyEngine
{
    public enum LightTypes { Point, Spot, Directional}

    public class Light: GameObjectComponent
    {
        public static bool CastShadows_Global = true;

        private static bool ShaderLoaded = false;
        private static int MAX_LIGHT_COUNT = 8;
        private static List<Light> LIGHTS;
        private static RenderTarget2D RenderTarget2D;
        private static Effect LightEffect;
        private static RenderTarget2D ShadowMap;
        private static List<LineOccluder> HandyList;
        private static List<Vector2> Points;
        private static List<Vector2> PointsTriangle;
        private static LineOccluder[] BorderOccluders;

        private static EffectParameter ShadowMap_param;
        private static EffectParameter DirectionalIntensity_param;
        private static EffectParameter LightCount_param;
        private static EffectParameter AngularRadius_param;
        private static EffectParameter X_Bias_param;
        private static EffectParameter Y_Bias_param;
        private static EffectParameter YOverX_param;
        private static EffectParameter Color_param;
        private static EffectParameter Radius_param;
        private static EffectParameter InnerRadius_param;
        private static EffectParameter InnerIntensity_param;
        private static EffectParameter Attenuation_param;
        private static EffectParameter CastShadows_param;
        private static EffectParameter CastShadow_param;
        private static EffectParameter ShadowConstant_param;

        public bool CastShadow = false;
        public LightTypes Type = LightTypes.Point;
        public Color color = Color.White;
        public float AngularRadius = 360;
        public float OuterRadius = 0.25f; //This is less intense than the Inner one
        public float InnerRadius = 0.025f; //10% of OuterRadius
        public float InnerInensity = 1.5f; // 1 is the same as the outer radius Intensity
        public float Attenuation = 1;
        public float DirectionalIntensity = 0.2f;
        public float ShadowIntensity = 0.5f;

        private Transform Transform;
        private float YOVERX;

        public override void Start()
        {
            if (!ShaderLoaded || LightEffect.IsDisposed)
            {
                PointsTriangle = new List<Vector2>();
                Points = new List<Vector2>();
                HandyList = new List<LineOccluder>();
                RenderTarget2D = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                LIGHTS = new List<Light>();
                LightEffect = Setup.Content.Load<Effect>("LightTest");
                ShaderLoaded = true;
                ShadowMap = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None); //Depth is not needed
                BorderOccluders = new LineOccluder[4];
                BorderOccluders[0] = new LineOccluder();
                BorderOccluders[1] = new LineOccluder();
                BorderOccluders[2] = new LineOccluder();
                BorderOccluders[3] = new LineOccluder();

                ShadowMap_param = LightEffect.Parameters["ShadowMap"];
                DirectionalIntensity_param = LightEffect.Parameters["DirectionalIntensity"];
                LightCount_param = LightEffect.Parameters["LightCount"];
                AngularRadius_param = LightEffect.Parameters["AngularRadius"];
                X_Bias_param = LightEffect.Parameters["X_Bias"];
                Y_Bias_param = LightEffect.Parameters["Y_Bias"];
                YOverX_param = LightEffect.Parameters["YOverX"];
                Color_param = LightEffect.Parameters["Color"];
                Radius_param = LightEffect.Parameters["Radius"];
                InnerRadius_param = LightEffect.Parameters["InnerRadius"];
                InnerIntensity_param = LightEffect.Parameters["InnerIntensity"];
                Attenuation_param = LightEffect.Parameters["Attenuation"];
                CastShadows_param = LightEffect.Parameters["CastShadows"];
                CastShadow_param = LightEffect.Parameters["CastShadow"];
                ShadowConstant_param = LightEffect.Parameters["ShadowConstant"];
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
            float[] ShadowIntensity = new float[LightCount];
            float[] CastShadow = new float[LightCount];

            HandyList.Clear();
            Points.Clear();

            //1- Needs some optimization, like if light is out of bounds of screen, it shouldn't be updated or sent to the light shader

            if (CastShadows_Global)
            {
                //Rendering ShadowMap Here
                Setup.GraphicsDevice.SetRenderTarget(ShadowMap);

                ShadowCaster[] Occluders = SceneManager.ActiveScene.FindGameObjectComponents<ShadowCaster>();
                Ray ray = new Ray();

                if (Occluders != null)
                {
                    Points.Add(Vector2.Zero);
                    Points.Add(Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth);
                    Points.Add(Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth + Vector2.UnitY * Setup.graphics.PreferredBackBufferHeight);
                    Points.Add(Vector2.UnitY * Setup.graphics.PreferredBackBufferHeight);

                    BorderOccluders[0].SetOccluder(Vector2.Zero, Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth);
                    BorderOccluders[1].SetOccluder(Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth, new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight));
                    BorderOccluders[2].SetOccluder(Vector2.Zero, new Vector2(0, Setup.graphics.PreferredBackBufferHeight));
                    BorderOccluders[3].SetOccluder(new Vector2(0, Setup.graphics.PreferredBackBufferHeight), new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight));

                    HandyList.Add(BorderOccluders[0]);
                    HandyList.Add(BorderOccluders[1]);
                    HandyList.Add(BorderOccluders[2]);
                    HandyList.Add(BorderOccluders[3]);

                    Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Setup.Camera.GetViewTransformationMatrix()); // -> Mandatory

                    foreach (ShadowCaster SC in Occluders)
                    {
                        if (!SC.Enabled)
                            continue;

                        HitBoxDebuger.DrawRectangle(SC.gameObject.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect());

                        LineOccluder[] LOCS = SC.ConvertToLineOccluders();
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 OccluderDirection = Vector2.Normalize(LOCS[i].EndPoint - LOCS[i].StartPoint);
                            Points.Add(LOCS[i].StartPoint);
                            Points.Add(LOCS[i].StartPoint - OccluderDirection * 0.5f); //Casting rays from the side of an object
                            HandyList.Add(LOCS[i]);
                        }
                    }
                }

                if (HandyList.Count != 0)
                {
                    for(int k=0; k<LIGHTS.Count; k++)
                    {
                        PointsTriangle.Clear();
                        if (!LIGHTS[k].gameObject.Active || !LIGHTS[k].Enabled)
                            continue;

                        foreach (Vector2 P in Points)
                        {
                            float ClosestIntersection = 100000000; //Dummy Number
                            Vector2 FoundClosestPoint = Vector2.Zero;

                            float Distance = -1;
                            ray.Origin = LIGHTS[k].Transform.Position;
                            ray.Direction = P - ray.Origin; //Don't Normalize!

                            foreach (LineOccluder LOC in HandyList) //Needs Optimization
                            {
                                Distance = ray.GetRayToLineSegmentIntersection(LOC);

                                if (Distance != -1 && Distance < ClosestIntersection)
                                {
                                    ClosestIntersection = Distance;
                                    FoundClosestPoint = ray.GetAPointAlongRay(Distance);
                                }
                            }

                            if (ClosestIntersection != 100000000) //INTERSECTION
                                PointsTriangle.Add(FoundClosestPoint);
                        }

                        PointsTriangle.Sort((x, y) => MathCompanion.GetAngle(x, LIGHTS[k].Transform.Position).CompareTo(MathCompanion.GetAngle(y, LIGHTS[k].Transform.Position)));

                        //DrawTriangles
                        for (int i = 0; i < PointsTriangle.Count - 1; i++)
                        {
                            HitBoxDebuger.DrawTriangle(PointsTriangle[i], PointsTriangle[i + 1], LIGHTS[k].Transform.Position); //White Color
                        }
                        HitBoxDebuger.DrawTriangle(PointsTriangle[PointsTriangle.Count - 1], PointsTriangle[0], LIGHTS[k].Transform.Position);
                    }
                }

                ShadowMap_param.SetValue(ShadowMap);

                Setup.spriteBatch.End();
            }

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
                ShadowIntensity[i] = 1 - LIGHTS[i].ShadowIntensity;
                CastShadow[i] = LIGHTS[i].CastShadow? 1:0;

                if (LIGHTS[i].Type == LightTypes.Directional)
                {
                    DirectionalIntensity_param.SetValue(LIGHTS[i].DirectionalIntensity);
                    InnerRadius[i] = 0;
                    Radius[i] = 0;
                }
            }

            LightCount_param.SetValue(LightCount);
            AngularRadius_param.SetValue(AngularRadius);
            X_Bias_param.SetValue(X_Bias);
            Y_Bias_param.SetValue(Y_Bias);
            YOverX_param.SetValue(LIGHTS[0].YOVERX);
            Color_param.SetValue(COLOR);
            Radius_param.SetValue(Radius);
            InnerRadius_param.SetValue(InnerRadius);
            InnerIntensity_param.SetValue(InnerIntensity);
            Attenuation_param.SetValue(Attenuation);
            CastShadows_param.SetValue(CastShadows_Global);
            CastShadow_param.SetValue(CastShadow);
            ShadowConstant_param.SetValue(ShadowIntensity);

            Setup.GraphicsDevice.SetRenderTarget(null);
            Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, LightEffect, Setup.Camera.GetViewTransformationMatrix());
            Setup.spriteBatch.Draw(RenderTarget2D, Vector2.Zero, new Rectangle(0, 0, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight), Color.White);
            Setup.spriteBatch.End();
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            Light clone = this.MemberwiseClone() as Light;

            clone.Transform = Clone.Transform;

            LIGHTS.Add(clone);

            return clone;
        }
    }
}
