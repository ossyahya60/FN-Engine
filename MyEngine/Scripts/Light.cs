using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MyEngine
{
    public enum LightTypes { Point, Spot, Directional}

    public class Light: GameObjectComponent
    {
        public static bool CastShadows = true;
        public static float ShadowIntensity = 0.5f;

        private static bool ShaderLoaded = false;
        private static int MAX_LIGHT_COUNT = 8;
        private static List<Light> LIGHTS;
        private static RenderTarget2D RenderTarget2D;
        private static Effect LightEffect;
        private static RenderTarget2D ShadowMap;
        private static List<LineOccluder> HandyList;
        private static List<Vector2> Points;
        private static List<Vector2> PointsTriangle;

        public LightTypes Type = LightTypes.Point;
        public Color color = Color.White;
        public float AngularRadius = 360;
        public float OuterRadius = 0.25f; //This is less intense than the Inner one
        public float InnerRadius = 0.025f; //10% of OuterRadius
        public float InnerInensity = 1.5f; // 1 is the same as the outer radius Intensity
        public float Attenuation = 1;
        public float DirectionalIntensity = 0.2f;

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

            HandyList.Clear();
            Points.Clear();

            //1- Needs some optimization, like if light is out of bounds of screen, it shouldn't be updated or sent to the light shader

            if (CastShadows)
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

                    HandyList.Add(new LineOccluder(Vector2.Zero, Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth));
                    HandyList.Add(new LineOccluder(Vector2.UnitX * Setup.graphics.PreferredBackBufferWidth, new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight)));
                    HandyList.Add(new LineOccluder(Vector2.Zero, new Vector2(0, Setup.graphics.PreferredBackBufferHeight)));
                    HandyList.Add(new LineOccluder(new Vector2(0, Setup.graphics.PreferredBackBufferHeight), new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight)));

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
                                //Rectangle LineBoundingBox = Rectangle.Empty;
                                //LineBoundingBox.X = (int)Math.Min(LOC.StartPoint.X, LOC.EndPoint.X);
                                //LineBoundingBox.Y = (int)Math.Min(LOC.StartPoint.Y, LOC.EndPoint.Y);
                                //LineBoundingBox.Width = (int)Math.Abs(LOC.StartPoint.X - LOC.EndPoint.X);
                                //LineBoundingBox.Height = (int)Math.Abs(LOC.StartPoint.Y - LOC.EndPoint.Y);

                                //bool X1 = MathCompanion.Abs(LIGHTS[k].Transform.Position.X - LineBoundingBox.Center.X) > LIGHTS[k].OuterRadius + LineBoundingBox.Width / 2;

                                //bool X2 = MathCompanion.Abs(LIGHTS[k].Transform.Position.Y - LineBoundingBox.Center.Y) > LIGHTS[k].OuterRadius + LineBoundingBox.Height / 2;

                                //if (X1 && X2)
                                //    continue;

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

                LightEffect.Parameters["ShadowMap"].SetValue(ShadowMap);

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

                if (LIGHTS[i].Type == LightTypes.Directional)
                {
                    LightEffect.Parameters["DirectionalIntensity"].SetValue(LIGHTS[i].DirectionalIntensity);
                    InnerRadius[i] = 0;
                    Radius[i] = 0;
                }
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
            LightEffect.Parameters["CastShadows"].SetValue(CastShadows);
            LightEffect.Parameters["ShadowConstant"].SetValue(1 - ShadowIntensity);

            Setup.GraphicsDevice.SetRenderTarget(null);
            Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, LightEffect, Setup.Camera.GetViewTransformationMatrix());
            Setup.spriteBatch.Draw(RenderTarget2D, new Vector2(0, 0), new Rectangle(0, 0, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight), Color.White);
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
