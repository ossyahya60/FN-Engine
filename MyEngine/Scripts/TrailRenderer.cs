using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System;
using System.IO;

//You can smooth edges by drawing additional segments between angles
namespace MyEngine
{
    public class TrailRenderer: GameObjectComponent
    {
        public Color FadeColor = Color.White;
        public float SpawnRate = 0f;  //Control this to make the trail Continous or discrete, 0 means Fully Continionus
        public Vector2 OffsetPosition;
        public int SegmentWidth = 10;
        public Color Color = Color.White;
        public float VanishAfter = 1f; //Time for Segments to start vanishing
        //public int ParticleSize = 20;
        //public bool RandomSize = true;
        public bool RandomColor = false;
        public bool ShrinkWithTime = true;
        public int MaxParticles
        {
            set
            {
                maxparticles = (int)MathCompanion.Clamp(value, 0, 10000);
            }
            get
            {
                return maxparticles;
            }
        }

        private int maxparticles = 100;
        private Queue Particles;
        private float SpawnRateCounter = 0;
        private Random random;
        private Color Color1, Color2, Color3, ColorDefault = Color.White;

        public TrailRenderer()
        {
            random = new Random();
        }

        public override void Start()
        {
            Particles = new Queue();
            SpawnRateCounter = SpawnRate;
            if(OffsetPosition == null)
                OffsetPosition = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            SpawnRateCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(SpawnRateCounter >= SpawnRate)
            {
                if (Particles.Count >= MaxParticles)
                    Particles.Dequeue();

                if (gameObject.Transform.Position - gameObject.Transform.LastPosition != Vector2.Zero)
                {
                    Particle particle = new Particle(false);
                    particle.FadeColor = FadeColor;
                    particle.Position = gameObject.Transform.LastPosition + OffsetPosition;
                    particle.LifeTime = VanishAfter;
                    //if (RandomSize)
                    //    particle.Size = (int)(ParticleSize * random.NextDouble());
                    //else
                    //    particle.Size = ParticleSize;
                    particle.ShrinkMode = ShrinkWithTime;
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

                    particle.Rotation = MathCompanion.GetAngle(gameObject.Transform.LastPosition, gameObject.Transform.Position);
                    particle.Length = 1 + (int)Math.Ceiling((gameObject.Transform.Position - gameObject.Transform.LastPosition).Length());
                    particle.Height = SegmentWidth;
                    particle.Layer = gameObject.Layer;

                    Particles.Enqueue(particle);
                }

                SpawnRateCounter = 0;
            }

            if (Particles.Count != 0)
                if ((Particles.Peek() as Particle).Expired)
                    Particles.Dequeue();

            foreach (Particle P in Particles)
                P.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //foreach (Particle P in Particles)
            //    P.Draw(spriteBatch);
            foreach (Particle P in Particles)
                P.DrawSegment();
        }

        private void FillParticlesBetweenTwoVectors()
        {
            //Vector2 Displacement = transform.Position - LastPosition;
            //Setup.spriteBatch.Draw(HitBoxDebuger._textureFilled, new Rectangle((int)(LastPosition.X * Transform.PixelsPerUnit), (int)(LastPosition.Y * Transform.PixelsPerUnit), (int)(Displacement.Length() * Transform.PixelsPerUnit), 20), Color.White);
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            TrailRenderer Clone = this.MemberwiseClone() as TrailRenderer;
            Clone.Particles = new Queue();
            Clone.SpawnRateCounter = 0;

            return Clone;
        }

        public override void Serialize(StreamWriter SW) //get the transform in deserialization
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("SpawnRate:\t" + SpawnRate.ToString() + "\n");
            SW.Write("OffsetPosition:\t" + OffsetPosition.X.ToString() + "\t" + OffsetPosition.Y.ToString() + "\n");
            SW.Write("SegmentWidth:\t" + SegmentWidth.ToString() + "\n");
            SW.Write("Color:\t" + Color.R.ToString() + "\t" + Color.G.ToString() + "\t" + Color.B.ToString() + "\t" + Color.A.ToString() + "\n");
            SW.Write("VanishAfter:\t" + VanishAfter.ToString() + "\n");
            SW.Write("RandomColor:\t" + RandomColor.ToString() + "\n");
            SW.Write("ShrinkWithTime:\t" + ShrinkWithTime.ToString() + "\n");
            SW.Write("maxparticles:\t" + maxparticles.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}
