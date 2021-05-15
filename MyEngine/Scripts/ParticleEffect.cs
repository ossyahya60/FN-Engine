using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.IO;
//Implement Burst Mode
namespace MyEngine
{
    public enum ParticleMode {PopCorn, Burst}

    public class ParticleEffect: GameObjectComponent
    {
        public Color Color = Color.White;
        public bool NoLifeTime = false;
        public bool Accelerate = false;
        public bool LineParticle = false;
        public bool FaceDirection = false;
        public bool BurstMode = false;
        public bool RandomDirection = true;
        public bool RandomColor = false;
        public bool RandomAlpha = false;
        public bool RandomRotation = false;
        public bool IsLooping = false;
        public bool ConstantVelocity = false;
        public bool ShrinkWithTime = false;
        public int ParticlesAtaTime = 10;
        public Texture2D CustomTexture = null;
        public Vector2 FireDirection;
        public float MinimumSize = 0.5f;
        public float AccelerationMagnitude = 0.1f;
        public float Speed = 5f;
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
        public int MaxParticles = 1000;
        public int ParticleSize = 20;
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
            FireCounter = TimeBetweenFiring;
            FireDirection = new Vector2(0, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (!BurstMode)
                ParticlesAtaTime = 1;

            if (IsLooping)
                FinishedEffect = false;

            if (!FinishedEffect)
            {
                FireCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                for (int i = 0; i < ParticlesAtaTime; i++)
                {
                    if (FireCounter >= TimeBetweenFiring)
                    {
                        if (BurstMode)
                        {
                            if (i == ParticlesAtaTime - 1)
                                FireCounter = 0;
                        }
                        else
                            FireCounter = 0;

                        if (Particles.Count != 0)
                        {
                            if ((Particles.Peek() as Particle).Expired)
                            {
                                Particles.Dequeue();
                                ParticleCounter--;
                                if (ParticleCounter == MaxParticles)
                                    FinishedEffect = true;

                                if (IsLooping)
                                {
                                    FinishedEffect = false;
                                    ParticleCounter = 0;
                                }
                            }
                        }

                        if (ParticleCounter < MaxParticles)
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
                                particle.Color = Color * (RandomAlpha ? (float)random.NextDouble() : 1);
                            particle.Size = ParticleSize;
                            particle.Position = gameObject.Transform.Position;
                            particle.LifeTime = VanishAfter;
                            particle.ShrinkMode = ShrinkWithTime;
                            particle.MinimumSize = minimumShrinkScale;
                            particle.Speed = Speed;
                            particle.ConstantVelocity = ConstantVelocity;
                            particle.Accelerate = Accelerate;
                            particle.AccelerationMagnitude = AccelerationMagnitude;
                            particle.MinimumSize = MinimumSize;
                            particle.Layer = gameObject.Layer;
                            particle.NoLifeTime = NoLifeTime;
                            if (RandomRotation)
                            {
                                Rotation = (float)random.NextDouble() * 360;
                                particle.Rotation = rotation;
                            }
                            else if(FaceDirection)
                                particle.Rotation = rotation;
                            else
                                particle.Rotation = rotation;
                            if (RandomDirection)
                            {
                                Vector2 HandyDirection = Vector2.Zero;
                                HandyDirection.X = (float)(random.NextDouble() * 2 - 1);
                                HandyDirection.Y = (float)(random.NextDouble() * 2 - 1);
                                particle.Direction = HandyDirection;

                                if (!RandomRotation && FaceDirection)
                                    particle.Rotation = MathHelper.ToDegrees((float)Math.Atan2(HandyDirection.Y, HandyDirection.X)) + 90;
                            }
                            else
                                particle.Direction = FireDirection;

                            Particles.Enqueue(particle);
                            ParticleCounter++;
                        }
                    }
                    else
                        break;
                }
            }

            foreach (Particle P in Particles)
                P.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (LineParticle)
                foreach (Particle P in Particles)
                    P.DrawSegment();
            else if (CustomTexture == null)
                foreach (Particle P in Particles)
                    P.Draw();
            else
                foreach (Particle P in Particles)
                    P.Draw(CustomTexture);
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            ParticleEffect Clone = this.MemberwiseClone() as ParticleEffect;
            Clone.Particles = new Queue();
            Clone.FireCounter = TimeBetweenFiring;
            Clone.ParticleCounter = 0;
            Clone.FinishedEffect = false;

            return Clone;
        }

        public override void Serialize(StreamWriter SW) //
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("LineParticle:\t" + LineParticle.ToString() + "\n");
            SW.Write("FaceDirection:\t" + FaceDirection.ToString() + "\n");
            SW.Write("BurstMode:\t" + BurstMode.ToString() + "\n");
            SW.Write("ParticlesAtaTime:\t" + ParticlesAtaTime.ToString() + "\n");
            if(CustomTexture != null && CustomTexture.Name != null)
                SW.Write("CustomTexture:\t" + CustomTexture.Name + "\n");
            else
                SW.Write("CustomTexture:\t" + "null\n"); //This doesn't necessarily mean that there is no custom texture, but there might be one that wasn't loaded but assigned
            SW.Write("FireDirection:\t" + FireDirection.X.ToString() + "\t" + FireDirection.Y.ToString() + "\n");
            SW.Write("Speed:\t" + Speed.ToString() + "\n");
            SW.Write("Color:\t" + Color.R.ToString() + "\t" + Color.G.ToString() + "\t" + Color.B.ToString() + "\t" + Color.A.ToString() + "\n");
            SW.Write("RandomDirection:\t" + RandomDirection.ToString() + "\n");
            SW.Write("RandomColor:\t" + RandomColor.ToString() + "\n");
            SW.Write("RandomRotation:\t" + RandomRotation.ToString() + "\n");
            SW.Write("IsLooping:\t" + IsLooping.ToString() + "\n");
            SW.Write("VanishAfter:\t" + VanishAfter.ToString() + "\n");
            SW.Write("TimeBetweenFiring:\t" + TimeBetweenFiring.ToString() + "\n");
            SW.Write("rotation:\t" + rotation.ToString() + "\n");
            SW.Write("MaxParticles:\t" + MaxParticles.ToString() + "\n");
            SW.Write("ParticleSize:\t" + ParticleSize.ToString() + "\n");
            SW.Write("ConstantVelocity:\t" + ConstantVelocity.ToString() + "\n");
            SW.Write("ShrinkWithTime:\t" + ShrinkWithTime.ToString() + "\n");
            SW.Write("minimumShrinkScale:\t" + minimumShrinkScale.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}
