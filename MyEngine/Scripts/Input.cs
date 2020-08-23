using Microsoft.Xna.Framework.Input;

namespace MyEngine
{
    public class Input: GameObjectComponent
    {
        public Keys Left;
        public Keys Right;
        public Keys Up;
        public Keys Down;

        public Input()
        {
            gameObject.AddComponent<Input>(this);

            Left = Keys.A;
            Right = Keys.D;
            Up = Keys.W;
            Down = Keys.S;
        }
    }
}
