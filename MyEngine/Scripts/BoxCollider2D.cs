﻿using Microsoft.Xna.Framework;

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
        private double GameTime = 1.0f / 60;
        private Rectangle bounds;
        private Rectangle HandyRectangle; //To avoid stack allocating a lot if memory in a short time
        //private float DisplaceMagnitude = 0.01f; //1 pixel

        public Rectangle GetDynamicCollider()
        {
            HandyRectangle.X = (int)(Transform.Position.X + Bounds.X);
            HandyRectangle.Y = (int)(Transform.Position.Y + Bounds.Y);
            HandyRectangle.Width = Bounds.Width;
            HandyRectangle.Height = Bounds.Height;

            return HandyRectangle;
        }
        
        public override void Start()
        {
            Transform = gameObject.Transform;
            HandyRectangle = new Rectangle();

            if (gameObject.GetComponent<SpriteRenderer>() != null && bounds.Width == 0)  //Initializing Collider bounds with the sprite bounds if exists
            {
                if (gameObject.GetComponent<SpriteRenderer>().Sprite != null)
                    Bounds = new Rectangle(0, 0, gameObject.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Size.X, gameObject.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Size.Y);
                else
                    Bounds = new Rectangle(0, 0, 100, 100);
            }
            else if(bounds.Width == 0)
                Bounds = new Rectangle(0, 0, 100, 100);
        }

        public bool IsTouching(Collider2D collider)  //Are the two colliders currently touching?
        {
            BoxCollider2D boxCollider = collider as BoxCollider2D;

            return GetDynamicCollider().Intersects(boxCollider.GetDynamicCollider());
        }

        bool CollisionDetection(Collider2D collider) //AABB collision detection =>We should check if the two "Bounding Boxes are touching, then make SAT Collision detection
        {
            BoxCollider2D boxCollider = collider as BoxCollider2D;

            return GetDynamicCollider().Intersects(boxCollider.GetDynamicCollider());
        }

        //SAT Collision response is good, you will possess the vector to push the two objects from each other
        void CollisionResponse(Collider2D collider, bool Continous)
        {
            if (!Continous && !isTrigger)
            {
                BoxCollider2D boxCollider = collider as BoxCollider2D;
                Rigidbody2D RB = gameObject.GetComponent<Rigidbody2D>();

                Transform.Move(-RB.Velocity.X * (float)GameTime, -RB.Velocity.Y * (float)GameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            GameTime = gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            BoxCollider2D Clone = this.MemberwiseClone() as BoxCollider2D;
            Clone.Transform = clone.Transform;
            Clone.bounds = new Rectangle(bounds.Location, bounds.Size);

            return Clone;
        }

        public bool Contains(Vector2 Point)
        {
            return GetDynamicCollider().Contains(Point);
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
