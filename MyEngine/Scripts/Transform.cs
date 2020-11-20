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
            //LastParent = gameObject.Parent;
            LastPosition = position;
            if (LastParent == null)
            {
                LocalPosition = Position;
                LocalRotation = Rotation;
                LocalScale = Scale;
            }
            else
                LocalScale = (LocalScale != Vector2.Zero)? LocalScale : Vector2.One;
        }

        //public static int PixelsPerUnit = 100; //100 pixel = 1 unit = 1 meter
        public float LocalRotation
        {
            set
            {
                LocalRotationJustChanged = true;
                localRotation = value;
                if (gameObject.Parent == null)
                    rotation = localRotation;
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
                LocalScaleJustChanged = true;
                localScale = (value.X >= 0 && value.X <= 1 && value.Y >= 0 && value.Y <= 1) ? value : localScale;
                if (gameObject.Parent == null)
                    scale = localScale;
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
                LocalPositionJustChanged = true;
                localPosition = value;
                if (gameObject.Parent == null)
                    position = localPosition;
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
                PositionJustChanged = true;
                LastPosition = Position;
                position = value;
            }
            get
            {
                return position;
            }
        }
        public float Rotation //Gets rotation in radians
        {
            set
            {
                LastRotation = rotation;
                rotation = value;
                RotationJustChanged = true;
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
                LastScale = Scale;
                ScaleJustChanged = true;
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
        public float LastRotation = 0;
        public Vector2 LastScale;

        private Vector2 scale = Vector2.One;
        private Vector2 position = Vector2.Zero;
        private Vector2 Value;
        private GameObject LastParent;
        private Vector2 localPosition;
        private float localRotation;
        private Vector2 localScale;
        private float rotation = 0;
        private bool RotationJustChanged = false;
        private bool PositionJustChanged = false;
        private bool ScaleJustChanged = false;
        private bool LocalRotationJustChanged = false;
        private bool LocalPositionJustChanged = false;
        private bool LocalScaleJustChanged = false;

        public void Move(float x, float y) //Move a gameobject a certain distance in x and y axis
        {
            LastPosition = Position;
            if (gameObject.Parent != null)
            {
                Value.X = x;
                Value.Y = y;
                LocalPosition += Value;
            }
            position.X += x;
            position.Y += y;
        }

        public void Move(Vector2 Movement) //Move a gameobject a certain distance in x and y axis
        {
            LastPosition = Position;
            if (gameObject.Parent != null)
                LocalPosition += Movement;
            position += Movement;
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            Transform Clone = this.MemberwiseClone() as Transform;

            Clone.position = new Vector2(position.X, position.Y);
            Clone.scale = new Vector2(scale.X, scale.Y);
            Clone.rotation = rotation;
            Clone.LastPosition = new Vector2(LastPosition.X, LastPosition.Y);
            Clone.LastScale = new Vector2(LastScale.X, LastScale.Y);
            Clone.Value = new Vector2(Value.X, Value.Y);
            Clone.localPosition = new Vector2(localPosition.X, localPosition.Y);
            Clone.localScale = new Vector2(localScale.X, localScale.Y);
            Clone.localRotation = localRotation;
            Clone.RotationJustChanged = false;
            Clone.ScaleJustChanged = false;
            Clone.PositionJustChanged = false;

            return Clone;
        }

        public override void Update(GameTime gameTime)
        {
            if (gameObject.Parent != null)
            {
                if (LastParent != gameObject.Parent)
                {
                    if(!LocalPositionJustChanged)
                        LocalPosition = Position - gameObject.Parent.GetComponent<Transform>().LastPosition;
                    if(!LocalRotationJustChanged)
                        LocalRotation = Rotation - gameObject.Parent.GetComponent<Transform>().LastRotation;
                    if(!LocalScaleJustChanged)
                        LocalScale = Scale - gameObject.Parent.GetComponent<Transform>().LastScale;
                }

                //Here I basically force the changes made in World coordinates
                if (!PositionJustChanged)
                    Position = LocalPosition + gameObject.Parent.GetComponent<Transform>().Position;
                if (!RotationJustChanged)
                    Rotation = LocalRotation + gameObject.Parent.GetComponent<Transform>().Rotation;
                if (!ScaleJustChanged)
                    Scale = LocalScale + gameObject.Parent.GetComponent<Transform>().Scale;
            }

            LastParent = gameObject.Parent;

            RotationJustChanged = false;
            ScaleJustChanged = false;
            PositionJustChanged = false;

            LocalRotationJustChanged = false;
            LocalPositionJustChanged = false;
            LocalScaleJustChanged = false;
        }
    }
}
