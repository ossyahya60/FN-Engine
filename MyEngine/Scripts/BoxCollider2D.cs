using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class BoxCollider2D : GameObjectComponent, Collider2D
    {
        public BoxCollider2D()
        {
            bounds = new Rectangle(0, 0, 0, 0);
        }

        public Rectangle Bounds
        {
            set
            {
                bounds = value;
            }
            get
            {
                return new Rectangle(bounds.X, bounds.Y, (int)(bounds.Width * Transform.Scale.X), (int)(bounds.Height * Transform.Scale.Y));
            }
        }
        public bool isTrigger = false;

        private Transform Transform;
        private float GameTime = 0.01f;
        private Rectangle bounds;
        //private float DisplaceMagnitude = 0.01f; //1 pixel

        public Rectangle GetDynamicCollider()
        {
            return new Rectangle((int)(Transform.Position.X + Bounds.X), (int)(Transform.Position.Y + Bounds.Y), Bounds.Width, Bounds.Height);
        }

        public override void Start()
        {
            Transform = gameObject.Transform;

            if (gameObject.GetComponent<SpriteRenderer>() != null && bounds.Width == 0)  //Initializing Collider bounds with the sprite bounds if exists
            {
                if (gameObject.GetComponent<SpriteRenderer>().Sprite != null)
                    Bounds = gameObject.GetComponent<SpriteRenderer>().Sprite.SourceRectangle;
                else
                    Bounds = new Rectangle(0, 0, 100, 100);
            }
            else if(bounds.Width == 0)
                Bounds = new Rectangle(0, 0, 100, 100);
        }

        public bool IsTouching(Collider2D collider)  //Are the two colliders currently touching?
        {
            BoxCollider2D boxCollider = collider as BoxCollider2D;

            if (GetDynamicCollider().Intersects(boxCollider.GetDynamicCollider()))
                return true;
            return false;
        }

        bool CollisionDetection(Collider2D collider, bool Continous)
        {
            BoxCollider2D boxCollider = collider as BoxCollider2D;

            if (GetDynamicCollider().Intersects(boxCollider.GetDynamicCollider()))
                return true;
            return false;
        }

        void CollisionResponse(Collider2D collider, bool Continous)
        {
            if (!Continous && !isTrigger)
            {
                BoxCollider2D boxCollider = collider as BoxCollider2D;
                Rigidbody2D RB = gameObject.GetComponent<Rigidbody2D>();

                RB.Move(-RB.Velocity.X * GameTime, -RB.Velocity.Y * GameTime);
                
                if ((GetDynamicCollider().Right <= boxCollider.GetDynamicCollider().Left || GetDynamicCollider().Left >= boxCollider.GetDynamicCollider().Right) && GetDynamicCollider().Bottom >= boxCollider.GetDynamicCollider().Top && GetDynamicCollider().Top <= boxCollider.GetDynamicCollider().Bottom)
                    RB.ResetHorizVelocity();
                else
                    RB.ResetVerticalVelocity();
            }
        }

        public override void Update(GameTime gameTime)
        {
            GameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rigidbody2D RB = gameObject.GetComponent<Rigidbody2D>();

            if (RB != null)
            {
                if(RB.Enabled && !RB.IsKinematic && RB.Velocity != Vector2.Zero)
                {
                    foreach (GameObject GO in SceneManager.GetActiveScene().GameObjects)
                    {
                        BoxCollider2D Box = GO.GetComponent<BoxCollider2D>();
                        if (Box != null)
                        {
                            if (Box.Enabled && Box != this && !Box.isTrigger)
                            {
                                if(CollisionDetection(Box, false))
                                    CollisionResponse(Box, false);
                            }
                        }
                    }
                }
            }
        }

        public bool IsTrigger()  //Is this collider marked as trigger? (trigger means no collision or physics is applied on this collider
        {
            return isTrigger;
        }

        public void OnCollisionEnter2D()
        {
            throw new System.NotImplementedException();
        }

        public void OnCollisionExit2D()
        {
            throw new System.NotImplementedException();
        }

        public void OnTriggerEnter2D()
        {
            throw new System.NotImplementedException();
        }

        public void OnTriggerExit2D()
        {
            throw new System.NotImplementedException();
        }
    }
}
