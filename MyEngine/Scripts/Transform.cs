using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class Transform: GameObjectComponent
    {
        public Transform()
        {
            Rotation = 0;
        }

        public static int PixelsPerUnit = 100; //100 pixel = 1 unit = 1 meter
        public Vector2 Position
        {
            set
            {
                position = value * PixelsPerUnit;
            }
            get
            {
                return position / PixelsPerUnit;
            }
        }
        public float Rotation //You set the rotation in degrees, retrieve it in radians (Just for easiness)
        {
            set
            {
                //rotation = MathHelper.ToRadians(value);
                rotation = value;
            }
            get
            {
                return rotation;
            }
        }
        public Vector2 Scale //Scale of a gameobject in x-y coordinate system
        {
            set
            {
                scale = (value.X >= 0 && value.X <= 1 && value.Y >= 0 && value.Y <= 1) ? value : scale;
            }
            get
            {
                return scale;
            }
        }

        public Vector2 Left
        {
            get
            {
                return new Vector2(-1, 0);
            }
        }
        public Vector2 Right
        {
            get
            {
                return new Vector2(1, 0);
            }
        }
        public Vector2 Up
        {
            get
            {
                return new Vector2(0, -1);
            }
        }
        public Vector2 Down
        {
            get
            {
                return new Vector2(0, 1);
            }
        }

        private Vector2 scale = Vector2.One;
        private float rotation = 0f;
        private Vector2 position = Vector2.Zero;

        public void Move(float x, float y) //Move a gameobject a certain distance in x and y axis
        {
            position.X += x * PixelsPerUnit;
            position.Y += y * PixelsPerUnit;
        }

        public void Move(Vector2 Movement) //Move a gameobject a certain distance in x and y axis
        {
            position += Movement * PixelsPerUnit;
        }

        public override GameObjectComponent DeepCopy()
        {
            Transform Clone = this.MemberwiseClone() as Transform;

            Clone.position = new Vector2(position.X, position.Y);
            Clone.Scale = new Vector2(scale.X, scale.Y);

            return Clone;
        }
    }
}
