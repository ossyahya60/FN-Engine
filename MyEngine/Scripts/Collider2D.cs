namespace MyEngine
{
    public interface Collider2D
    {
        bool IsTouching(Collider2D collider);

        void OnCollisionEnter2D();

        void OnTriggerEnter2D();

        void OnCollisionExit2D();

        void OnTriggerExit2D();

        bool IsTrigger();
    }
}
