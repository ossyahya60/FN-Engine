using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public class TileMap : GameObjectComponent
    {
        public Texture2D TileSet;
        public Vector2 StartingPoint;
        public List<GameObject> Tiles;

        private TileMapLoader.Layer Layer;

        public TileMap()
        {
            StartingPoint = Vector2.Zero;
            Tiles = new List<GameObject>();
        }

        public void LoadMap(string FileName, Texture2D tileSet, string LayerName)
        {
            TileMapLoader.LoadJson(FileName);
            TileSet = tileSet;
            Layer = null;
            foreach (TileMapLoader.Layer layer in TileMapLoader.GetLayers())
                if (LayerName == layer.name)
                    Layer = layer;

            List<int> Data = Layer.data;
            int Width = Layer.gridCellWidth;
            int Height = Layer.gridCellHeight;

            for (int j = 0; j < Layer.gridCellsY; j++)
            {
                for (int k = 0; k < Layer.gridCellsX; k++)
                {
                    if (Data[j * Layer.gridCellsX + k] != -1)
                    {
                        GameObject T = new GameObject();
                        T.AddComponent<Transform>(new Transform());
                        T.AddComponent<SpriteRenderer>(new SpriteRenderer());
                        T.AddComponent<Tile>(new Tile()); //???
                        T.Parent = gameObject;
                        T.Layer = gameObject.Layer;
                        T.Start();
                        //T.AddComponent<Animator>(new Animator());
                        SpriteRenderer SR = T.GetComponent<SpriteRenderer>();
                        T.Transform.LocalPosition = StartingPoint + new Vector2(Width * k, Height * j);
                        SR.Sprite.Texture = TileSet;
                        SR.Sprite.SourceRectangle = new Rectangle(Width * (Data[j * Layer.gridCellsX + k] % Layer.gridCellsX), Height * (Data[j * Layer.gridCellsX + k] / Layer.gridCellsY), Width, Height);

                        Tiles.Add(T);
                        SceneManager.ActiveScene.AddGameObject(T);
                    }
                }
            }
        }

        public void Clean()
        {
            foreach (GameObject T in Tiles)
                SceneManager.ActiveScene.RemoveGameObject(T);

            Tiles.Clear();
        }
    }
}
