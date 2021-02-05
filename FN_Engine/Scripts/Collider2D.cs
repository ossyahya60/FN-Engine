using Microsoft.Xna.Framework;

namespace FN_Engine
{
    public interface Collider2D
    {
        bool IsTouching(Collider2D collider);

        bool Contains(Vector2 Point);

        void OnCollisionEnter2D();

        void OnTriggerEnter2D();

        void OnCollisionExit2D();

        void OnTriggerExit2D();

        bool IsTrigger();
    }
}
