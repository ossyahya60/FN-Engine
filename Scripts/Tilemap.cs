using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FN_Engine
{
    public class Tilemap: GameObjectComponent
    {
        public bool ShowGrid = false;
        public float Opacity = 0.5f;
        public int Thickness = 2;
        public Point GridSize
        {
            set
            {
                gridSize.X = Math.Clamp(value.X, 1, 1000);
                gridSize.Y = Math.Clamp(value.Y, 1, 1000);

                if (GridSize.X % 2 != 0)
                    GridSize += new Point(1, 0);

                if (GridSize.Y % 2 != 0)
                    GridSize += new Point(0, 1);
            }
            get
            {
                return gridSize;
            }
        }
        public Point TileSize
        {
            set
            {
                tileSize.X = Math.Clamp(value.X, 1, 1000);
                tileSize.Y = Math.Clamp(value.Y, 1, 1000);
            }
            get
            {
                return tileSize;
            }
        }

        private Point tileSize;
        private Point gridSize;
        private float Layer;

        public Tilemap()
        {
            tileSize = new Point(32, 32);
            gridSize = new Point(10, 10);
        }

        public override void Start()
        {
            Layer = gameObject.Layer;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Layer != gameObject.Layer)
            {
                Layer = gameObject.Layer;

                foreach (GameObject GO in gameObject.Children)
                    GO.Layer = Layer;
            }

            if (ShowGrid)
                HitBoxDebuger.DrawGrid(GridSize.Y, GridSize.X, gameObject.Transform.Position, (TileSize.ToVector2() * gameObject.Transform.Scale).ToPoint(), Color.Yellow * Opacity, Thickness);
        } 
    }
}
