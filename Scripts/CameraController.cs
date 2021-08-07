using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FN_Engine
{
    public class CameraController: GameObjectComponent
    {
        public bool Visualize = true;
        public bool IsFullScreen = false;
        public float CameraZoom = 1;
        public Point ScreenSize = new Point(1366, 768);
        public Point TargetResolution = new Point(1366, 768);

        public override void Start()
        {
            //gameObject.Transform.Position = Vector2.Zero;
            //gameObject.Transform.Rotation = 0;
            //gameObject.Transform.Scale = new Vector2(1280, 768);

            //Setup.resolutionIndependentRenderer.InitializeResolutionIndependence(ScreenSize.X, ScreenSize.Y, Setup.Camera);
        }

        public override void Update(GameTime gameTime)
        {
            gameObject.Name = "Camera Controller";
            gameObject.ShouldBeDeleted = false;
            gameObject.ShouldBeRemoved = false;

            Setup.Camera.Position = gameObject.Transform.Position;
            Setup.Camera.Rotation = gameObject.Transform.Rotation;
            Setup.Camera.Zoom = CameraZoom;

            ResolutionIndependentRenderer.SetVirtualResolution(TargetResolution.X, TargetResolution.Y);
            ResolutionIndependentRenderer.SetResolution(ScreenSize.X, ScreenSize.Y, IsFullScreen);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Visualize)
                HitBoxDebuger.DrawNonFilledRectangle(new Rectangle(gameObject.Transform.Position.ToPoint() - new Point((int)(TargetResolution.X * 0.5f), (int)(TargetResolution.Y * 0.5f)), TargetResolution), 2);
        }
    }
}
