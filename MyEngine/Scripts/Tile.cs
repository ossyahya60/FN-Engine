namespace MyEngine
{
    public class Tile : GameObjectComponent
    {
        public int Type;
        public int Effect;
        public bool IsBackground;

        public override void Start()
        {
            IsBackground = false;
            Type = 0;
            Effect = 0;
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            return this.MemberwiseClone() as Tile;
        }
    }
}
