using Microsoft.Xna.Framework;

namespace MyEngine
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

        void Visualize(float X_Bias = 0, float Y_Bias = 0);
    }
}
