using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FN_Engine
{
    public interface Collider2D
    {
        bool IsTouching(Collider2D collider);

        bool CollisionDetection(Collider2D collider, bool Continous);

        void CollisionResponse(Rigidbody2D YourRigidBody, Collider2D collider, float DeltaTime, ref List<GameObjectComponent> CDs, Vector2 CollisionPos, bool ResetVelocity);

        bool Contains(Vector2 Point);

        bool IsTrigger();

        internal void Visualize(float X_Bias = 0, float Y_Bias = 0);
    }
}
