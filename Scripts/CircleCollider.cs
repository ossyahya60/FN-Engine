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
                    Center = new Vector2(SR.Sprite.SourceRectangle.Size.X * 0.5f);
                    Radius = (int)Center.X;
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

                if (Math.Abs(Center1.X - Center2.X) > Radius + CR2.Radius)
                    return false;

                if (Math.Abs(Center1.Y - Center2.Y) > Radius + CR2.Radius)
                    return false;

                //if ((Center1 - Center2).LengthSquared() > (Radius + CR2.Radius) * (Radius + CR2.Radius))
                //    return false;
            }
            else if(collider is BoxCollider2D) //Assuming Center as Origin
            {
                Rectangle TempCollider = (collider as BoxCollider2D).GetDynamicCollider();

                if (Math.Abs(Center.X + gameObject.Transform.Position.X - TempCollider.Center.X) > Radius + TempCollider.Width/2)
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
                HitBoxDebuger.DrawCircle(Center + gameObject.Transform.Position + new Vector2(X_Bias, Y_Bias), Radius);
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
