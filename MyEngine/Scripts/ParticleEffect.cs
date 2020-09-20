using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;

namespace MyEngine
{
    public class ParticleEffect: GameObjectComponent
    {
        public float Layer = 0;
        public Texture2D CustomTexture = null;
        public Transform Transform;
        public Vector2 FireDirection;
        public float Speed = 5f;
        public Color Color = Color.White;
        public bool RandomDirection = true;
        public bool RandomColor = false;
        public bool RandomRotation = true;
        public bool IsLooping = false;
        public float VanishAfter = 1f;
        public float TimeBetweenFiring = 0.25f;
        public float Rotation //Angle
        {
            set
            {
                rotation = MathHelper.ToDegrees(MathHelper.WrapAngle(MathHelper.ToRadians(value)));
            }
            get
            {
                return rotation;
            }
        }
        public int MaxParticles = 15;
        public int ParticleSize = 20;
        public bool ConstantVelocity = false;
        public bool ShrinkWithTime = false;
        public float MnimumShrinkScale
        {
            set
            {
                minimumShrinkScale = MathCompanion.Clamp(value, 0, 1);
            }
            get
            {
                return minimumShrinkScale;
            }
        }

        private float minimumShrinkScale;
        private Queue Particles;
        private bool FinishedEffect = false;
        private float FireCounter;
        private int ParticleCounter = 0;
        private Random random;
        private Color Color1, Color2, Color3, ColorDefault = Color.White;
        private float rotation = 0;

        public ParticleEffect()
        {
            Particles = new Queue();
            random = new Random();
        }

        public override void Start()
        {
            Transform = gameObject.GetComponent<Transform>();
            FireCounter = TimeBetweenFiring;
            FireDirection = new Vector2(0, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if(!FinishedEffect)
            {
                FireCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(FireCounter >= TimeBetweenFiring)
                {
                    FireCounter = 0;
                    if (Particles.Count != 0)
                    {
                        if ((Particles.Peek() as Particle).Expired)
                        {
                            Particles.Dequeue();
                            if (ParticleCounter == MaxParticles)
                                FinishedEffect = true;

                            if (IsLooping)
                            {
                                FinishedEffect = false;
                                ParticleCounter = 0;
                            }
                        }
                    }

                    if(ParticleCounter < MaxParticles)
                    {
                        Particle particle = new Particle(true);
                        if (RandomColor)
                        {
                            Color1 = Color.Multiply(ColorDefault, (float)random.NextDouble());
                            Color2 = Color.Multiply(ColorDefault, (float)random.NextDouble());
                            Color3 = Color.Multiply(ColorDefault, (float)random.NextDouble());
                            particle.Color.R = Color1.R;
                            particle.Color.G = Color2.G;
                            particle.Color.B = Color3.B;
                        }
                        else
                            particle.Color = Color;
                        particle.Size = ParticleSize;
                        particle.Position = Transform.Position * Transform.PixelsPerUnit;
                        particle.LifeTime = VanishAfter;
                        particle.ShrinkMode = ShrinkWithTime;
                        particle.MinimumSize = minimumShrinkScale;
                        particle.Speed = Speed;
                        particle.ConstantVelocity = false;
                        particle.Accelerate = false;
                        particle.AccelerationMagnitude = 0.1f;
                        particle.ShrinkMode = true;
                        particle.MinimumSize = 0.5f;
                        particle.Layer = Layer;
                        if (RandomRotation)
                        {
                            Rotation = (float)random.NextDouble() * 360;
                            particle.Rotation = rotation;
                        }
                        else
                            particle.Rotation = rotation;
                        if (RandomDirection)
                            particle.Direction = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
                        else
                            particle.Direction = FireDirection;

                        Particles.Enqueue(particle);
                        ParticleCounter++;
                    }
                }
            }

            foreach (Particle P in Particles)
                P.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CustomTexture == null)
                foreach (Particle P in Particles)
                    P.Draw();
            else
                foreach (Particle P in Particles)
                    P.Draw(CustomTexture);
        }
    }
}
