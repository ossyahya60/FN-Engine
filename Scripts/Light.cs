using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace FN_Engine
{
    public enum LightTypes { Point, Spot, Directional }

    public class Light : GameObjectComponent
    {
        public static bool CastShadows_Global = false;

        internal static RenderTarget2D RenderTarget2D;
        internal static RenderTarget2D ShadowMap;

        private static bool ShaderLoaded = false;
        private static int MAX_LIGHT_COUNT = 15;
        private static Effect LightEffect;
        private static List<LineOccluder> HandyList;
        private static List<Vector2> Points;
        private static List<Vector2> PointsTriangle;
        private static LineOccluder[] BorderOccluders;
        //private static List<Light> LIGHTS;

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
        private static float YOVERX;
        //private static EffectParameter CameraPosNorm_param;

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

        public static void Reset()
        {
            ShaderLoaded = false;
        }

        public override void Start()
        {
            
        }

        public override void Destroy()
        {
            //LIGHTS.Remove(this);
        }

        //public void Rebuild() //Not for high level users
        //{
        //    //LIGHTS.Add(this);
        //}

        public static void Init_Light()
        {
            if (!ShaderLoaded || LightEffect.IsDisposed)
            {
                PointsTriangle = new List<Vector2>();
                Points = new List<Vector2>();
                HandyList = new List<LineOccluder>();
                RenderTarget2D = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                SceneManager.SceneTexPtr = Scene.GuiRenderer.BindTexture(RenderTarget2D);
                //LIGHTS = new List<Light>();
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
                //CameraPosNorm_param = LightEffect.Parameters["CameraPosNorm"];
            }

            //LIGHTS.Add(this);
            YOVERX = (float)Setup.graphics.PreferredBackBufferHeight / Setup.graphics.PreferredBackBufferWidth;

            if (RenderTarget2D == null)
            {
                RenderTarget2D = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                SceneManager.SceneTexPtr = Scene.GuiRenderer.BindTexture(RenderTarget2D);
            }

            Setup.GraphicsDevice.SetRenderTarget(RenderTarget2D); //Render Target
        }

        public static void ApplyLighting()
        {
            //You probably need to add they XY coordinates of BiasScene in editor mode too (below two lines) //Ignore this for now
            Rectangle BiasScene = new Rectangle((int)(Setup.Camera.Position.X - Setup.graphics.PreferredBackBufferWidth * 0.5f), (int)(Setup.Camera.Position.Y - Setup.graphics.PreferredBackBufferHeight * 0.5f), Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight);
            if (FN_Editor.EditorScene.IsThisTheEditor)
                BiasScene = new Rectangle((int)(0 + -Setup.graphics.PreferredBackBufferWidth * 0.5f + Setup.Camera.Position.X), (int)(0 - Setup.graphics.PreferredBackBufferHeight * 0.5f + Setup.Camera.Position.Y), (int)FN_Editor.GizmosVisualizer.SceneWindow.Z, (int)FN_Editor.GizmosVisualizer.SceneWindow.W);

            Light[] LIGHTS = SceneManager.ActiveScene.FindGameObjectComponents<Light>();
            if (LIGHTS == null || LIGHTS.Length == 0)
            {
                //Setup.GraphicsDevice.SetRenderTarget(null);
                //Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Setup.Camera.GetViewTransformationMatrix());
                //Setup.spriteBatch.Draw(RenderTarget2D, BiasScene.Location.ToVector2(), new Rectangle(Point.Zero, BiasScene.Size), Color.White);
                //Setup.spriteBatch.End();

                return;
            }

            int LightCount = LIGHTS.Length;

            if (LightCount == 0)
                return;

            int CappedLightCount = LightCount > MAX_LIGHT_COUNT ? MAX_LIGHT_COUNT : LightCount;
            Vector3[] COLOR = new Vector3[CappedLightCount];
            float[] InnerRadius = new float[CappedLightCount];
            float[] Radius = new float[CappedLightCount];
            float[] Attenuation = new float[CappedLightCount];
            float[] X_Bias = new float[CappedLightCount];
            float[] Y_Bias = new float[CappedLightCount];
            float[] AngularRadius = new float[CappedLightCount];
            float[] InnerIntensity = new float[CappedLightCount];
            float[] ShadowIntensity = new float[CappedLightCount];
            float[] CastShadow = new float[CappedLightCount];

            HandyList.Clear();
            Points.Clear();

            //1- Needs some optimization, like if light is out of bounds of screen, it shouldn't be updated or sent to the light shader

            if (CastShadows_Global)
            {
                //Rendering ShadowMap Here
                Setup.GraphicsDevice.SetRenderTarget(ShadowMap);

                ShadowCaster[] Occluders = SceneManager.ActiveScene.FindGameObjectComponents<ShadowCaster>();
                Ray ray = new Ray();


                Vector2 Bias = -new Vector2(Setup.graphics.PreferredBackBufferWidth * 0.5f - Setup.Camera.Position.X, Setup.graphics.PreferredBackBufferHeight * 0.5f - Setup.Camera.Position.Y);

                if (Occluders != null)
                {
                    Points.Add(Bias);
                    Points.Add(Bias + Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth);
                    Points.Add(Bias + Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth + Vector2.UnitY * Setup.graphics.PreferredBackBufferHeight);
                    Points.Add(Bias + Vector2.UnitY * Setup.graphics.PreferredBackBufferHeight);

                    BorderOccluders[0].SetOccluder(Bias, Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth + Bias);
                    BorderOccluders[1].SetOccluder(Bias + Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth, new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight) + Bias);
                    BorderOccluders[2].SetOccluder(Bias + Vector2.Zero, new Vector2(0, Setup.graphics.PreferredBackBufferHeight) + Bias);
                    BorderOccluders[3].SetOccluder(Bias + new Vector2(0, Setup.graphics.PreferredBackBufferHeight), new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight) + Bias);

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
                        //HitBoxDebuger.DrawRectangle_Effect(SC.gameObject.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect());

                        LineOccluder[] LOCS = SC.ConvertToLineOccluders();
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 OccluderDirection = Vector2.Normalize(LOCS[i].EndPoint - LOCS[i].StartPoint);
                            Points.Add(LOCS[i].StartPoint);
                            Points.Add(LOCS[i].StartPoint - OccluderDirection * 0.5f); //Casting rays from the side of an object
                            HandyList.Add(LOCS[i]);
                        }
                    }

                    Setup.spriteBatch.End();
                }

                if (HandyList.Count != 0)
                {
                    for (int k = 0; k < LIGHTS.Length; k++)
                    {
                        PointsTriangle.Clear();
                        if (!LIGHTS[k].gameObject.IsActive() || !LIGHTS[k].Enabled)
                            continue;

                        foreach (Vector2 P in Points)
                        {
                            float ClosestIntersection = 100000000; //Dummy Number
                            Vector2 FoundClosestPoint = Vector2.Zero;

                            float Distance = -1;
                            ray.Origin = LIGHTS[k].gameObject.Transform.Position;
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

                        PointsTriangle.Sort((x, y) => MathCompanion.GetAngle(x, LIGHTS[k].gameObject.Transform.Position).CompareTo(MathCompanion.GetAngle(y, LIGHTS[k].gameObject.Transform.Position)));

                        Bias = -Bias;
                        //DrawTriangles
                        for (int i = 0; i < PointsTriangle.Count - 1; i++)
                        {
                            HitBoxDebuger.DrawTriangle(Bias + PointsTriangle[i], Bias + PointsTriangle[i + 1], Bias + LIGHTS[k].gameObject.Transform.Position); //White Color
                        }
                        HitBoxDebuger.DrawTriangle(Bias + PointsTriangle[PointsTriangle.Count - 1], Bias + PointsTriangle[0], Bias + LIGHTS[k].gameObject.Transform.Position);
                    }
                }
                
                ShadowMap_param.SetValue(ShadowMap);
                Setup.GraphicsDevice.SetRenderTarget(RenderTarget2D);
            }

            for (int i = 0; i < CappedLightCount; i++)
            {
                if (LIGHTS[i].Enabled == false || LIGHTS[i].gameObject.IsActive() == false)
                    continue;

                LIGHTS[i].gameObject.Transform.AdjustTransformation();
                COLOR[i] = LIGHTS[i].color.ToVector3();
                InnerRadius[i] = MathHelper.Clamp(LIGHTS[i].InnerRadius, 0, LIGHTS[i].OuterRadius) * Setup.Camera.Zoom;
                Radius[i] = MathHelper.Clamp(LIGHTS[i].OuterRadius, 0, 1) * Setup.Camera.Zoom;
                Attenuation[i] = MathHelper.Clamp(LIGHTS[i].Attenuation, 0, float.MaxValue);
                X_Bias[i] = (LIGHTS[i].gameObject.Transform.Position.X - Setup.Camera.Position.X) / Setup.graphics.PreferredBackBufferWidth;
                Y_Bias[i] = (LIGHTS[i].gameObject.Transform.Position.Y - Setup.Camera.Position.Y) / Setup.graphics.PreferredBackBufferHeight;
                AngularRadius[i] = MathHelper.Clamp(LIGHTS[i].AngularRadius, 0, 360);
                InnerIntensity[i] = MathHelper.Clamp(LIGHTS[i].InnerInensity, 1, float.MaxValue);
                ShadowIntensity[i] = MathHelper.Clamp(1 - LIGHTS[i].ShadowIntensity, 0, 1);
                CastShadow[i] = LIGHTS[i].CastShadow ? 1 : 0;

                if (LIGHTS[i].Type == LightTypes.Directional) //Bug here where directional intensity is set forever
                {
                    DirectionalIntensity_param.SetValue(MathHelper.Clamp(LIGHTS[i].DirectionalIntensity, 0, 1));
                    InnerRadius[i] = 0;
                    Radius[i] = 0;
                }
            }

            LightCount_param.SetValue(CappedLightCount);
            //CameraPosNorm_param.SetValue(new Vector2(Setup.Camera.Position.X / Setup.resolutionIndependentRenderer.VirtualWidth, Setup.Camera.Position.Y / Setup.resolutionIndependentRenderer.VirtualHeight));
            AngularRadius_param.SetValue(AngularRadius);
            AngularRadius_param.SetValue(AngularRadius);
            X_Bias_param.SetValue(X_Bias);
            Y_Bias_param.SetValue(Y_Bias);
            YOverX_param.SetValue(YOVERX);
            Color_param.SetValue(COLOR);
            Radius_param.SetValue(Radius);
            InnerRadius_param.SetValue(InnerRadius);
            InnerIntensity_param.SetValue(InnerIntensity);
            Attenuation_param.SetValue(Attenuation);
            CastShadows_param.SetValue(CastShadows_Global);
            CastShadow_param.SetValue(CastShadow);
            ShadowConstant_param.SetValue(ShadowIntensity);

            //Setup.GraphicsDevice.SetRenderTarget(null);
            //Setup.GraphicsDevice.SetRenderTarget(RenderTarget2D);
            Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, LightEffect, Setup.Camera.GetViewTransformationMatrix());
            Setup.spriteBatch.Draw(RenderTarget2D, BiasScene.Location.ToVector2(), Color.White);
            Setup.spriteBatch.End();
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            Light clone = MemberwiseClone() as Light;
            clone.gameObject = Clone;

            //LIGHTS.Add(clone);

            return clone;
        }

        public override void Serialize(StreamWriter SW) //get the transform in deserialization
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            //Call Start in Deserialize
            SW.Write("CastShadows_Global:\t" + CastShadows_Global.ToString() + "\n");
            SW.Write("CastShadow:\t" + CastShadow.ToString() + "\n");
            SW.Write("Type:\t" + Type.ToString() + "\n");
            SW.Write("color:\t" + color.R.ToString() + "\t" + color.G.ToString() + "\t" + color.B.ToString() + "\t" + color.A.ToString() + "\n");
            SW.Write("AngularRadius:\t" + AngularRadius.ToString() + "\n");
            SW.Write("OuterRadius:\t" + OuterRadius.ToString() + "\n");
            SW.Write("InnerRadius:\t" + InnerRadius.ToString() + "\n");
            SW.Write("InnerInensity:\t" + InnerInensity.ToString() + "\n");
            SW.Write("Attenuation:\t" + Attenuation.ToString() + "\n");
            SW.Write("DirectionalIntensity:\t" + DirectionalIntensity.ToString() + "\n");
            SW.Write("ShadowIntensity:\t" + ShadowIntensity.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }

        public override void Deserialize(StreamReader SR)
        {
            //SR.ReadLine();

            base.Deserialize(SR);
            CastShadows_Global = bool.Parse(SR.ReadLine().Split('\t')[1]);
            CastShadow = bool.Parse(SR.ReadLine().Split('\t')[1]);
            Type = (LightTypes)Enum.Parse(Type.GetType(), SR.ReadLine().Split('\t')[1]);
            string[] COLOR = SR.ReadLine().Split('\t');
            color = new Color(byte.Parse(COLOR[1]), byte.Parse(COLOR[2]), byte.Parse(COLOR[3]), byte.Parse(COLOR[4]));
            AngularRadius = float.Parse(SR.ReadLine().Split('\t')[1]);
            OuterRadius = float.Parse(SR.ReadLine().Split('\t')[1]);
            InnerRadius = float.Parse(SR.ReadLine().Split('\t')[1]);
            InnerInensity = float.Parse(SR.ReadLine().Split('\t')[1]);
            Attenuation = float.Parse(SR.ReadLine().Split('\t')[1]);
            DirectionalIntensity = float.Parse(SR.ReadLine().Split('\t')[1]);
            ShadowIntensity = float.Parse(SR.ReadLine().Split('\t')[1]);

            SR.ReadLine();
        }
    }
}