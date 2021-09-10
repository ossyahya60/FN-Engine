using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace FN_Engine
{
    public class BoxCollider2D : GameObjectComponent, Collider2D
    {
        public bool isTrigger = false;
        public bool SlideCollision = true;
        public Rectangle Bounds;

        public Rectangle GetDynamicCollider()
        {
            Rectangle HandyRectangle = Rectangle.Empty;
            HandyRectangle.X = (int)(gameObject.Transform.Position.X + Bounds.X - Bounds.Width * 0.5f * Math.Abs(gameObject.Transform.Scale.X));
            HandyRectangle.Y = (int)(gameObject.Transform.Position.Y + Bounds.Y - Bounds.Height * 0.5f * Math.Abs(gameObject.Transform.Scale.Y));
            HandyRectangle.Width = (int)(Bounds.Width * Math.Abs(gameObject.Transform.Scale.X));
            HandyRectangle.Height = (int)(Bounds.Height * Math.Abs(gameObject.Transform.Scale.Y));

            return HandyRectangle;
        }
        
        public override void Start()
        {
            var SR = gameObject.GetComponent<SpriteRenderer>();
            if (SR != null && Bounds.Width == 0)  //Initializing Collider bounds with the sprite bounds if exists
            {
                if (SR.Sprite != null)
                    Bounds = new Rectangle(0, 0, SR.Sprite.SourceRectangle.Size.X, SR.Sprite.SourceRectangle.Size.Y);
                else
                    Bounds = new Rectangle(0, 0, 100, 100);
            }
            else if(Bounds.Width == 0)
                Bounds = new Rectangle(0, 0, 100, 100);
        }

        public bool IsTouching(Collider2D collider)  //Are the two colliders currently touching?
        {
            if (collider is BoxCollider2D) //Assuming Center as Origin
            {
                return GetDynamicCollider().Intersects((collider as BoxCollider2D).GetDynamicCollider());
            }
            else if (collider is CircleCollider) //Assuming Center as Origin
            {
                CircleCollider TempCollider = collider as CircleCollider;
                var ThisCollider = GetDynamicCollider();

                if (MathCompanion.Abs(ThisCollider.Center.X - TempCollider.Center.X) > TempCollider.Radius + ThisCollider.Width / 2)
                    return false;

                if (MathCompanion.Abs(ThisCollider.Center.Y - TempCollider.Center.Y) > TempCollider.Radius + ThisCollider.Height / 2)
                    return false;
            }

            return true;
        }

        bool Collider2D.CollisionDetection(Collider2D collider, bool Continous) //AABB collision detection =>We should check if the two "Bounding Boxes are touching, then make SAT Collision detection
        {
            return collider.IsTouching(this);
        }

        //Note: This code is inspired by:
        //URL: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
        //SAT Collision response is good, you will possess the vector to push the two objects from each other
        void Collider2D.CollisionResponse(Rigidbody2D YourRigidBody, Collider2D collider, float DeltaTime, ref List<GameObjectComponent> CDs, Vector2 CollisionPos, bool ResetVelocity)
        {
            Rectangle DC1 = GetDynamicCollider();
            Rectangle DC2 = (collider as BoxCollider2D).GetDynamicCollider();

            float DistanceBetweenCollidersX = YourRigidBody.Velocity.X > 0 ? Math.Abs(DC1.Right - DC2.Left) : Math.Abs(DC1.Left - DC2.Right);
            float DistanceBetweenCollidersY = YourRigidBody.Velocity.Y > 0 ? Math.Abs(DC1.Bottom - DC2.Top) : Math.Abs(DC1.Top - DC2.Bottom);

            float TimeBetweenCollidersX = YourRigidBody.Velocity.X != 0 ? Math.Abs(DistanceBetweenCollidersX / YourRigidBody.Velocity.X) : 0;
            float TimeBetweenCollidersY = YourRigidBody.Velocity.Y != 0 ? Math.Abs(DistanceBetweenCollidersY / YourRigidBody.Velocity.Y) : 0;

            float ShortestTime;
            var OldPos = YourRigidBody.gameObject.Transform.Position;

            if (YourRigidBody.Velocity.X != 0 && YourRigidBody.Velocity.Y == 0)
            {
                // Colliison on X-axis only
                ShortestTime = TimeBetweenCollidersX;
                YourRigidBody.gameObject.Transform.MoveX(ShortestTime * YourRigidBody.Velocity.X);
                YourRigidBody.Velocity.X = ResetVelocity? 0 : YourRigidBody.Velocity.X;
            }
            else if (YourRigidBody.Velocity.X == 0 && YourRigidBody.Velocity.Y != 0)
            {
                // Colliison on Y-axis only
                ShortestTime = TimeBetweenCollidersY;
                YourRigidBody.gameObject.Transform.MoveY(ShortestTime * YourRigidBody.Velocity.Y);
                YourRigidBody.Velocity.Y = ResetVelocity? 0 : YourRigidBody.Velocity.Y;
            }
            else
            {
                // Colliison on both axis
                ShortestTime = Math.Min(TimeBetweenCollidersX, TimeBetweenCollidersY);
                YourRigidBody.gameObject.Transform.Move(ShortestTime * YourRigidBody.Velocity.X, ShortestTime * YourRigidBody.Velocity.Y);
                //YourRigidBody.Velocity = ResetVelocity? Vector2.Zero : YourRigidBody.Velocity;

                if (SlideCollision)
                {
                    if (ShortestTime == TimeBetweenCollidersX)
                    {
                        YourRigidBody.gameObject.Transform.Position.X = OldPos.X;
                        YourRigidBody.Velocity.X = ResetVelocity ? 0 : YourRigidBody.Velocity.X;

                        if (!collider.CollisionDetection(this, false))
                            YourRigidBody.gameObject.Transform.Position.Y = CollisionPos.Y;
                    }

                    if (ShortestTime == TimeBetweenCollidersY)
                    {
                        YourRigidBody.gameObject.Transform.Position.Y = OldPos.Y;
                        YourRigidBody.Velocity.Y = ResetVelocity ? 0 : YourRigidBody.Velocity.Y;

                        if (!collider.CollisionDetection(this, false))
                            YourRigidBody.gameObject.Transform.Position.X = CollisionPos.X;
                    }
                }
                else
                    YourRigidBody.Velocity = ResetVelocity ? Vector2.Zero : YourRigidBody.Velocity;
            }

            foreach (Collider2D CD in CDs)
            {
                if (!gameObject.Name.Equals((CD as GameObjectComponent).gameObject.Name) && CD.CollisionDetection(this, false))
                {
                    YourRigidBody.gameObject.Transform.Position = OldPos;
                    YourRigidBody.Velocity = Vector2.Zero;
                    break;
                }
            }
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            BoxCollider2D Clone = this.MemberwiseClone() as BoxCollider2D;
            Clone.gameObject = clone;
            Clone.Bounds = new Rectangle(Bounds.Location, Bounds.Size);

            return Clone;
        }

        public bool Contains(Vector2 Point)
        {
            return GetDynamicCollider().Contains(Point);
        }

        public bool IsTrigger()  //Is this collider marked as trigger? (trigger means no collision or physics is applied on this collider)
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

        void Collider2D.Visualize(float X_Bias, float Y_Bias)
        {
            if (Enabled)
            {
                //var SR = gameObject.GetComponent<SpriteRenderer>();
                //Point Bias = (SR != null && SR.Sprite.Texture != null) ? (-SR.Sprite.Origin).ToPoint() : Point.Zero;
                var DynCollid = GetDynamicCollider();
                DynCollid.Offset(new Point((int)X_Bias, (int)Y_Bias));

                HitBoxDebuger.DrawNonFilledRectangle_Effect(DynCollid);
            }
        }

        public override void Serialize(StreamWriter SW)
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("SourceRectangle:\t" + Bounds.X.ToString() + "\t" + Bounds.Y.ToString() + "\t" + Bounds.Width.ToString() + "\t" + Bounds.Height.ToString() + "\n");
            SW.Write("IsTrigger:\t" + isTrigger.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}
