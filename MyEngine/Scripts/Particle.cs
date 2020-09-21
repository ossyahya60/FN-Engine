using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class Particle
    {
        public Vector2 Position;
        public int Size = 1;
        public float LifeTime = 1f;
        public Color Color = Color.White;
        public float Layer = 0f;
        public bool Expired = false;
        public bool ShrinkMode = true;
        public float Rotation = 0;

        //Specific for trail renderer
        public int Length;
        public int Height
        {
            set
            {
                height = (value >= 0) ? value : 0;
                OriginalSize = height;
            }
            get
            {
                return height;
            }
        }

        public bool ConstantVelocity = false; //For Particle effect
        public Vector2 Direction;
        public float MinimumSize = 1f;
        public float Speed = 5f;
        public bool Accelerate = false;
        public float AccelerationMagnitude = 2f;

        private float LifeTimeCounter = 0;
        private int OriginalSize;
        private bool OneTime = false;
        private bool ForParticleEffect = false;
        private float AccelerationCounter = 0;
        private int height;

        public Particle(bool ForParticleEffect)
        {
            OriginalSize = Size;
            this.ForParticleEffect = ForParticleEffect;
        }

        public void Update(GameTime gameTime)
        {
            if(!OneTime)
            {
                if (ForParticleEffect)
                    OriginalSize = Size;
                else
                    OriginalSize = height;
                OneTime = true;
            }

            if (!Expired)
            {
                if(ForParticleEffect)
                {
                    if (ConstantVelocity)
                        Position += Speed * (Vector2.Normalize(Direction));
                    else
                    {
                        AccelerationCounter = (Accelerate)? AccelerationCounter + AccelerationMagnitude: AccelerationCounter - AccelerationMagnitude;
                        Position += (MathCompanion.Clamp(Speed + AccelerationCounter, 0, Speed + AccelerationCounter)) * Vector2.Normalize(Direction);
                    }
                }

                if (ShrinkMode)
                {
                    if(ForParticleEffect)
                        Size = (int)(((LifeTime - LifeTimeCounter * MinimumSize) / LifeTime) * OriginalSize);
                    else
                        height = (int)(((LifeTime - LifeTimeCounter * MinimumSize) / LifeTime) * OriginalSize);
                }

                if (LifeTimeCounter >= LifeTime)
                    Expired = true;
                else
                    LifeTimeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Draw()
        {
            if(!Expired)
            {
                HitBoxDebuger.DrawRectangle(new Rectangle((int)Position.X, (int)Position.Y, Size, Size), Color, Rotation, Layer);
            }
        }

        public void Draw(Texture2D texture)
        {
            if (!Expired)
            {
                HitBoxDebuger.DrawRectangle(new Rectangle((int)Position.X, (int)Position.Y, Size, Size), Color, Rotation, texture, Layer, Vector2.Zero);
            }
        }

        ///For Trail Renderer, Segments not particles
        public void DrawSegment()
        {
            if (!Expired)
            {
                HitBoxDebuger.DrawLine(new Rectangle((int)Position.X, (int)Position.Y, Length, height), Color, Rotation, Layer, Vector2.One * 0.5f);
            }
        }
    }
}
