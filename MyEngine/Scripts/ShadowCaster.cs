using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MyEngine
{
    public class ShadowCaster: GameObjectComponent
    {
        //public static List<Rectangle> BBs; //Light Should Clear tgis after using it

        private LineOccluder[] lineOccluders;
        private SpriteRenderer SR = null;

        public override void Start()
        {
            SR = gameObject.GetComponent<SpriteRenderer>();
            lineOccluders = new LineOccluder[4];

            Rectangle Occluder = SR.Sprite.DynamicScaledRect();

            lineOccluders[0] = new LineOccluder(new Vector2(Occluder.Left, Occluder.Top), new Vector2(Occluder.Right, Occluder.Top));
            lineOccluders[1] = new LineOccluder(new Vector2(Occluder.Right, Occluder.Top), new Vector2(Occluder.Right, Occluder.Bottom));
            lineOccluders[2] = new LineOccluder(new Vector2(Occluder.Right, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Bottom));
            lineOccluders[3] = new LineOccluder(new Vector2(Occluder.Left, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Top));
        }

        //public override void Update(GameTime gameTime)
        //{
        //    BBs.Add(gameObject.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect());
        //}

        public LineOccluder[] ConvertToLineOccluders(Rectangle Rect)
        {
            lineOccluders[0].SetOccluder(new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Right, Rect.Top));
            lineOccluders[1].SetOccluder(new Vector2(Rect.Right, Rect.Top), new Vector2(Rect.Right, Rect.Bottom));
            lineOccluders[2].SetOccluder(new Vector2(Rect.Right, Rect.Bottom), new Vector2(Rect.Left, Rect.Bottom));
            lineOccluders[3].SetOccluder(new Vector2(Rect.Left, Rect.Bottom), new Vector2(Rect.Left, Rect.Top));

            return lineOccluders;
        }

        public LineOccluder[] ConvertToLineOccluders()
        {
            Rectangle Occluder = SR.Sprite.DynamicScaledRect();

            lineOccluders[0].SetOccluder(new Vector2(Occluder.Left, Occluder.Top), new Vector2(Occluder.Right, Occluder.Top));
            lineOccluders[1].SetOccluder(new Vector2(Occluder.Right, Occluder.Top), new Vector2(Occluder.Right, Occluder.Bottom));
            lineOccluders[2].SetOccluder(new Vector2(Occluder.Right, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Bottom));
            lineOccluders[3].SetOccluder(new Vector2(Occluder.Left, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Top));

            return lineOccluders;
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            ShadowCaster clone = this.MemberwiseClone() as ShadowCaster;

            clone.SR = Clone.GetComponent<SpriteRenderer>();
            clone.lineOccluders = new LineOccluder[4];

            Rectangle Occluder = clone.SR.Sprite.DynamicScaledRect();

            clone.lineOccluders[0] = new LineOccluder(new Vector2(Occluder.Left, Occluder.Top), new Vector2(Occluder.Right, Occluder.Top));
            clone.lineOccluders[1] = new LineOccluder(new Vector2(Occluder.Right, Occluder.Top), new Vector2(Occluder.Right, Occluder.Bottom));
            clone.lineOccluders[2] = new LineOccluder(new Vector2(Occluder.Right, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Bottom));
            clone.lineOccluders[3] = new LineOccluder(new Vector2(Occluder.Left, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Top));

            return clone;
        }
    }
}
