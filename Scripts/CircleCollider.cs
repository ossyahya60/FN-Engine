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

                if (MathCompanion.Abs(Center1 - Center2).LengthSquared() >= (Radius + CR2.Radius) * (Radius + CR2.Radius))
                    return false;
            }
            else if(collider is BoxCollider2D) //Assuming Center as Origin
            {
                Rectangle TempCollider = (collider as BoxCollider2D).GetDynamicCollider();

                if (Math.Abs(Center.X + gameObject.Transform.Position.X - TempCollider.Center.X ) > Radius + TempCollider.Width/2)
                    return false;

                if (Math.Abs(Center.Y + gameObject.Transform.Position.Y - TempCollider.Center.Y) > Radius + TempCollider.Height/2)
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

                var OldPos = RB.gameObject.Transform.Position;
                gameObject.Transform.Position = C2 + (Radius + circle2.Radius) * -Vector2.Normalize(C2 - Center - CollisionPos);
                Vector2 Diff = MathCompanion.Abs(C2 - Center - gameObject.Transform.Position) / (Radius + circle2.Radius);

                RB.Velocity = new Vector2(RB.Velocity.X * Diff.Y, RB.Velocity.Y * Diff.X);
                //RB.Velocity = Vector2.Zero;
                //RB.gameObject.Transform.Position = CollisionPos;
                //RB.gameObject.Transform.Move(-MathCompanion.Sign(RB.Velocity) * ((Radius + circle2.Radius) * Vector2.One - DistanceBetweenColliders));
                //RB.Velocity = ResetVelocity ? RB.Velocity * new Vector2(DistanceBetweenColliders.Y, DistanceBetweenColliders.X) / circle2.Radius : RB.Velocity;
                //if (!collider.CollisionDetection(this, false))
                //    RB.gameObject.Transform.Position = CollisionPos;

                //foreach (Collider2D CD in CDs)
                //{
                //    if (!gameObject.Name.Equals((CD as GameObjectComponent).gameObject.Name) && !CD.IsTrigger() && CD.CollisionDetection(this, false))
                //    {
                //        RB.gameObject.Transform.Position = OldPos;
                //        RB.Velocity = Vector2.Zero;
                //        break;
                //    }
                //}
            }
            else if(collider is BoxCollider2D)
            {

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
