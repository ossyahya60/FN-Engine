using Microsoft.Xna.Framework;

namespace MyEngine
{
    public enum ParticleType {Sand, Water, Fire, Smoke, Acid, Wood};

    public class UnitParticle
    {
        public float Velocity;
        public ParticleType particleType;
        public Vector2 Position;
        public Color Color;
        public bool HasBeenUpdated = false;

        public UnitParticle(ParticleType particleType, Vector2 position)
        {
            Velocity = 1;
            this.particleType = particleType;
            Position = position;
            Utility.Vector2Int(ref Position);

            switch (particleType)
            {
                case ParticleType.Sand:
                    Color = Color.Orange;
                    break;
                case ParticleType.Water:
                    Color = Color.CornflowerBlue;
                    break;
                case ParticleType.Fire:
                    Color = Color.Yellow;
                    break;
                case ParticleType.Acid:
                    Color = Color.LightGreen;
                    break;
                case ParticleType.Smoke:
                    Color = Color.LightGray * 0.5f;
                    break;
                case ParticleType.Wood:
                    Color = Color.Gray;
                    break;
                default:
                    break;
            }
        }

        public void IncreaseUnitX()
        {
            Position.X = Position.X + 1;
        }

        public void DecreaseUnitX()
        {
            Position.X = Position.X - 1;
        }

        public void IncreaseUnitY()
        {
            if (Position.Y < Setup.graphics.PreferredBackBufferHeight - 1)
                Position.Y = Position.Y + 1;
        }

        public void DecreaseUnitY()
        {
            Position.Y = Position.Y - 1;
        }

        public void Draw()
        {
            HitBoxDebuger.DrawRectangle(Position, Color);
        }
    }
}
