using Microsoft.Xna.Framework;
using System.IO;

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
                return new Rectangle(bounds.X, bounds.Y, (int)(bounds.Width), (int)(bounds.Height));
            }
        }
        public bool isTrigger = false;

        private double GameTime = 1.0f / 60;
        private Rectangle bounds;
        private Vector2 PrevScale;
        //private float DisplaceMagnitude = 0.01f; //1 pixel

        public Rectangle GetDynamicCollider()
        {
            Rectangle HandyRectangle = Rectangle.Empty;
            HandyRectangle.X = (int)(gameObject.Transform.Position.X + Bounds.X);
            HandyRectangle.Y = (int)(gameObject.Transform.Position.Y + Bounds.Y);
            HandyRectangle.Width = Bounds.Width;
            HandyRectangle.Height = Bounds.Height;

            return HandyRectangle;
        }
        
        public override void Start()
        {
            if (gameObject.GetComponent<SpriteRenderer>() != null && bounds.Width == 0)  //Initializing Collider bounds with the sprite bounds if exists
            {
                if (gameObject.GetComponent<SpriteRenderer>().Sprite != null)
                    Bounds = new Rectangle(0, 0, (int)(gameObject.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Size.X * gameObject.Transform.Scale.X), (int)(gameObject.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Size.Y * gameObject.Transform.Scale.Y));
                else
                    Bounds = new Rectangle(0, 0, 100, 100);
            }
            else if(bounds.Width == 0)
                Bounds = new Rectangle(0, 0, 100, 100);

            PrevScale = gameObject.Transform.Scale;
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

                gameObject.Transform.Move(-RB.Velocity.X * (float)GameTime, -RB.Velocity.Y * (float)GameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Point Delta = (gameObject.Transform.Scale - PrevScale).ToPoint();
            bounds.Size += new Point(Delta.X, Delta.Y);

            GameTime = gameTime.ElapsedGameTime.TotalSeconds;
            PrevScale = gameObject.Transform.Scale;
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            BoxCollider2D Clone = this.MemberwiseClone() as BoxCollider2D;
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

        public override void Serialize(StreamWriter SW)
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("SourceRectangle:\t" + bounds.X.ToString() + "\t" + bounds.Y.ToString() + "\t" + bounds.Width.ToString() + "\t" + bounds.Height.ToString() + "\n");
            SW.Write("IsTrigger:\t" + isTrigger.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }

        public GameObjectComponent ReturnGOC()
        {
            return this;
        }

        public void Visualize()
        {
            //HitBoxDebuger.DrawNonFilledRectangle(GetDynamicCollider()); //This function eats resources, just find an alternative
            HitBoxDebuger.DrawNonFilledRectangle_Effect(GetDynamicCollider());
        }
    }
}
