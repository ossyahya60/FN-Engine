using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace FN_Engine
{
    public class CircleCollider : GameObjectComponent, Collider2D
    {
        public bool CenteredOrigin = true;
        public bool isTrigger = true;
        public int Radius = 50;
        public Vector2 Center
        {
            set
            {
                center = value;
            }
            get
            {
                if (CenteredOrigin)
                {
                    var SR = gameObject.GetComponent<SpriteRenderer>();
                    if (SR == null)
                        return gameObject.Transform.Position;
                    else
                        return gameObject.Transform.Position;
                }

                return center;
            }
        }

        private Vector2 center = Vector2.Zero;

        public CircleCollider(int Radius)
        {
            this.Radius = Radius;
        }

        public CircleCollider()
        {
            this.Radius = 50;
        }

        public bool Contains(Vector2 Point)
        {//Support uniform scale only (For Now)
            Vector2 Output = gameObject.Transform.Position - Point;
            if (MathCompanion.Abs(Output.Length()) > Radius*gameObject.Transform.Scale.X)
                return false;

            return true;
        }

        public bool IsTouching(Collider2D collider)
        {
            if(collider is CircleCollider) //Assuming Center as Origin
            {
                Vector2 Output = gameObject.Transform.Position - (collider as CircleCollider).gameObject.Transform.Position;
                if ((int)Output.Length() > Radius + (collider as CircleCollider).Radius)
                    return false;
            }
            else if(collider is BoxCollider2D) //Assuming Center as Origin
            {
                Rectangle TempCollider = (collider as BoxCollider2D).GetDynamicCollider();

                if (MathCompanion.Abs(Center.X - TempCollider.Center.X) > Radius + TempCollider.Width/2)
                    return false;

                if (MathCompanion.Abs(Center.Y - TempCollider.Center.Y) > Radius + TempCollider.Height/2)
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
            if (!isTrigger)
            {
                gameObject.Transform.Move(-RB.Velocity.X * DeltaTime, -RB.Velocity.Y * DeltaTime);
                RB.Velocity = Vector2.Zero;
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
                HitBoxDebuger.DrawCircle(Center + new Vector2(X_Bias, Y_Bias), Radius);
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
