using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class Transform: GameObjectComponent
    {
        public Transform()
        {
            Rotation = 0;
            Scale = Vector2.One;
            LastPosition = Vector2.Zero;
        }

        public override void Start()
        {
            if (LastParent == null)
            {
                LocalPosition = Position;
                LocalRotation = Rotation;
                LocalScale = Scale;
            }
            else
                LocalScale = (LocalScale != Vector2.Zero)? LocalScale : Vector2.One;
        }

        public static int PixelsPerUnit = 100; //100 pixel = 1 unit = 1 meter
        public float LocalRotation
        {
            set
            {
                localRotation = value;
                if (gameObject.Parent == null)
                    Rotation = localRotation;
            }
            get
            {
                return localRotation;
            }
        }
        public Vector2 LocalScale
        {
            set
            {
                localScale = (value.X >= 0 && value.X <= 1 && value.Y >= 0 && value.Y <= 1) ? value : localScale;
                if (gameObject.Parent == null)
                    Scale = localScale;
            }
            get
            {
                return localScale;
            }
        }
        public Vector2 LocalPosition
        {
            set
            {
                localPosition = value;
                if (gameObject.Parent == null)
                    Position = localPosition;
            }
            get
            {
                return localPosition;
            }
        }
        public Vector2 Position
        {
            set
            {
                LastPosition = Position;
                position = value * PixelsPerUnit;
            }
            get
            {
                return position / PixelsPerUnit;
            }
        }
        public float Rotation //Gets rotation in radians
        { set; get; } = 0f;
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
        public Vector2 LastPosition;

        private Vector2 scale = Vector2.One;
        private Vector2 position = Vector2.Zero;
        private Vector2 Value;
        private GameObject LastParent;
        private Vector2 localPosition;
        private float localRotation;
        private Vector2 localScale;

        public void Move(float x, float y) //Move a gameobject a certain distance in x and y axis
        {
            LastPosition = Position;
            if (gameObject.Parent != null)
            {
                Value.X = x;
                Value.Y = y;
                LocalPosition += Value;
            }
            position.X += x * PixelsPerUnit;
            position.Y += y * PixelsPerUnit;
        }

        public void Move(Vector2 Movement) //Move a gameobject a certain distance in x and y axis
        {
            LastPosition = Position;
            if (gameObject.Parent != null)
                LocalPosition += Movement;
            position += Movement * PixelsPerUnit;
        }

        public override GameObjectComponent DeepCopy()
        {
            Transform Clone = this.MemberwiseClone() as Transform;

            Clone.position = new Vector2(position.X, position.Y);
            Clone.Scale = new Vector2(scale.X, scale.Y);

            return Clone;
        }

        public override void Update(GameTime gameTime)
        {
            if (gameObject.Parent != null)
            {
                if (LastParent != gameObject.Parent)
                {
                    LocalPosition = Position - gameObject.Parent.GetComponent<Transform>().Position;
                    LocalRotation = Rotation - gameObject.Parent.GetComponent<Transform>().Rotation;
                    LocalScale = Scale - gameObject.Parent.GetComponent<Transform>().Scale;
                }

                Position = LocalPosition + gameObject.Parent.GetComponent<Transform>().Position;
                Rotation = LocalRotation + gameObject.Parent.GetComponent<Transform>().Rotation;
                Scale = LocalScale + gameObject.Parent.GetComponent<Transform>().Scale;
            }

            LastParent = gameObject.Parent;
        }
    }
}
