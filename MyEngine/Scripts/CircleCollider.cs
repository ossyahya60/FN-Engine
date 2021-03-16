using Microsoft.Xna.Framework;
using System.IO;

namespace MyEngine
{
    public class CircleCollider : GameObjectComponent, Collider2D
    {
        public bool isTrigger = true;
        public int Radius = 50;

        public CircleCollider(int Radius)
        {
            this.Radius = Radius;
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

                if (MathCompanion.Abs(gameObject.Transform.Position.X - TempCollider.Center.X) > Radius + TempCollider.Width/2)
                    return false;

                if (MathCompanion.Abs(gameObject.Transform.Position.Y - TempCollider.Center.Y) > Radius + TempCollider.Height/2)
                    return false;
            }

            return true;
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

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            CircleCollider clone = this.MemberwiseClone() as CircleCollider;

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
