using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class ParticleSimulator: GameObjectComponent
    {
        private List<UnitParticle> unitParticles; //One list for now

        public ParticleSimulator()
        {
            unitParticles = new List<UnitParticle>();
        }

        public override void Start()
        {
            unitParticles.Clear();   
        }

        public override void Update(GameTime gameTime)
        {
            foreach (UnitParticle UP in unitParticles)
            {
                bool Below = true;
                bool BelowRight = true;
                bool BelowLeft = true;
                bool Right = true;
                bool Left = true;
                switch (UP.particleType)
                {
                    case ParticleType.Sand:
                        foreach (UnitParticle UP2 in unitParticles)
                        {
                            if(Below)
                                Below = Below & !(UP.Position.Y + 1 == UP2.Position.Y);
                            if(BelowRight)
                                BelowRight = BelowRight & !((UP.Position + Vector2.One) == UP2.Position);
                            if(BelowLeft)
                                BelowLeft = BelowLeft & !((UP.Position + Vector2.UnitY - Vector2.UnitX) == UP2.Position);
                        }
                        break;
                    case ParticleType.Water:
                        foreach (UnitParticle UP2 in unitParticles)
                        {
                            if (Below)
                                Below = Below & !(UP.Position + Vector2.UnitY == UP2.Position);
                            if (BelowRight)
                                BelowRight = BelowRight & !((UP.Position + Vector2.One) == UP2.Position);
                            if (BelowLeft)
                                BelowLeft = BelowLeft & !((UP.Position + Vector2.UnitY - Vector2.UnitX) == UP2.Position);
                            if (Right)
                                Right = Right & !(UP.Position + Vector2.UnitX == UP2.Position);
                            if (Left)
                                Left = Left & !(UP.Position - Vector2.UnitX == UP2.Position);
                        }
                        break;
                    default:
                        break;
                }

                if (Below)
                    UP.IncreaseUnitY();
                else if (BelowRight)
                {
                    UP.IncreaseUnitX();
                    UP.IncreaseUnitY();
                }
                else if (BelowLeft)
                {
                    UP.DecreaseUnitX();
                    UP.IncreaseUnitY();
                }
                else if (Right && UP.particleType == ParticleType.Water)
                    UP.IncreaseUnitX();
                else if (Left && UP.particleType == ParticleType.Water)
                    UP.DecreaseUnitX();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (UnitParticle UP in unitParticles)
                UP.Draw();
        }

        public void AddParticle(UnitParticle particle)
        {
            unitParticles.Add(particle);
        }

        public int GetParticlesCount()
        {
            return unitParticles.Count;
        }
    }
}
