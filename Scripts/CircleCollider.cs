using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace FN_Engine
{
    public class CircleCollider : GameObjectComponent, Collider2D
    {
        public bool isTrigger = true;
        public int Radius = 50;
        public Vector2 Center;
        public bool SlideCollision = true;

        private Vector2 vRayToNearest;
        private float fOverlap;

        public CircleCollider(int Radius)
        {
            this.Radius = Radius;
        }

        public CircleCollider()
        {
            Radius = 50;
        }

        public override void Start()
        {
            var SR = gameObject.GetComponent<SpriteRenderer>();
            if (SR != null && Center.X == 0)  //Initializing Collider bounds with the sprite bounds if exists
            {
                if (SR.Sprite != null)
                {
                    Center = Vector2.Zero;
                    Radius = (int)(SR.Sprite.SourceRectangle.Size.X * 0.5f * gameObject.Transform.Scale.X);
                }
            }
        }

        public bool Contains(Vector2 Point)
        {
            Vector2 Output = gameObject.Transform.Position + Center - Point;
            if (Output.LengthSquared() > Radius * Radius)
                return false;

            return true;
        }

        public bool IsTouching(Collider2D collider)
        {
            if(collider is CircleCollider) //Assuming Center as Origin
            {
                Vector2 Center1 = gameObject.Transform.Position + Center;
                CircleCollider CR2 = collider as CircleCollider;
                Vector2 Center2 = CR2.gameObject.Transform.Position + CR2.Center;

                if ((Center1 - Center2).LengthSquared() > (Radius + CR2.Radius) * (Radius + CR2.Radius))
                    return false;
            }
            else if(collider is BoxCollider2D) //Assuming Center as Origin
            {
                Rectangle IncomingCollider = (collider as BoxCollider2D).GetDynamicCollider();

                /////////////////////////This code is heavily inspired by "OneLoneCoder" the youtuber///////////////////////

                Vector2 vNearestPoint;
                vNearestPoint.X = Math.Clamp(gameObject.Transform.Position.X, IncomingCollider.Left, IncomingCollider.Right);
                vNearestPoint.Y = Math.Clamp(gameObject.Transform.Position.Y, IncomingCollider.Top, IncomingCollider.Bottom);

                vRayToNearest = vNearestPoint - gameObject.Transform.Position;
                fOverlap = Radius - vRayToNearest.Length();
                //if (std::isnan(fOverlap)) fOverlap = 0;

                // If overlap is positive, then a collision has occurred, so we displace backwards by the 
                // overlap amount. The potential position is then tested against other tiles in the area
                // therefore "statically" resolving the collision
                if (fOverlap <= 0)
                    return false;
            }

            return true;
        }

        public bool CollisionDetection(Collider2D collider, bool Continous)
        {
            return IsTouching(collider);
        }

        public void CollisionResponse(Rigidbody2D RB, Collider2D collider, float DeltaTime, ref List<GameObjectComponent> CDs, Vector2 CollisionPos, bool ResetVelocity) //change this
        {
            if (collider is CircleCollider)
            {
                CircleCollider circle2 = collider as CircleCollider;
                Vector2 C2 = circle2.Center + circle2.gameObject.Transform.Position;

                gameObject.Transform.Position = C2 + (Radius + circle2.Radius) * -Vector2.Normalize(C2 - Center - CollisionPos);
                Vector2 Diff = MathCompanion.Abs(C2 - Center - gameObject.Transform.Position) / (Radius + circle2.Radius);
                RB.Velocity = new Vector2(RB.Velocity.X * Diff.Y, RB.Velocity.Y * Diff.X);
            }
            else if(collider is BoxCollider2D)
            {
                Rectangle IncomingCollider = (collider as BoxCollider2D).GetDynamicCollider();

                /////////////////////////This code is heavily inspired by "OneLoneCoder" the youtuber///////////////////////

                gameObject.Transform.Position = CollisionPos;
                // Statically resolve the collision
                gameObject.Transform.Position -= Vector2.Normalize(vRayToNearest) * fOverlap;

                if(gameObject.Transform.Position.Y + Center.Y > IncomingCollider.Top && gameObject.Transform.Position.Y + Center.Y < IncomingCollider.Bottom)
                    RB.Velocity.X = 0;
                if (gameObject.Transform.Position.X + Center.X > IncomingCollider.Left && gameObject.Transform.Position.X + Center.X < IncomingCollider.Right)
                    RB.Velocity.Y = 0;
            }
        }

        public bool IsTrigger()
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
                HitBoxDebuger.DrawCircleNonFilled(Center + gameObject.Transform.Position + new Vector2(X_Bias, Y_Bias), Radius, (int)(Radius * 0.9f), Color.Yellow, 0, gameObject.Transform.Scale.X);
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            CircleCollider clone = this.MemberwiseClone() as CircleCollider;
            clone.gameObject = Clone;

            return clone;
        }

        public override void Serialize(StreamWriter SW) //Pass transform in deserialization
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("IsTrigger:\t" + isTrigger.ToString() + "\n");
            SW.Write("Radius:\t" + Radius.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}
