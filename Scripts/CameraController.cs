using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FN_Engine
{
    public class CameraController : GameObjectComponent
    {
        public bool Visualize = true;
        public bool IsFullScreen
        {
            set
            {
                Dirty = true;
                fullScreen = value;
            }
            get
            {
                return fullScreen;
            }
        }
        public float CameraZoom
        {
            set
            {
                Dirty = true;
                cameraZoom = value;
            }
            get
            {
                return cameraZoom;
            }
        }
        public Point ScreenSize
        {
            set
            {
                Dirty = true;
                screenSize = value;
            }
            get
            {
                return screenSize;
            }
        }
        public Point TargetResolution
        {
            set
            {
                Dirty = true;
                targetResolution = value;
            }
            get
            {
                return targetResolution;
            }
        }

        private bool Dirty = true;
        private Point screenSize = new Point(1366, 768);
        private Point targetResolution = new Point(1366, 768);
        private bool fullScreen = false;
        private float cameraZoom = 1;

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
            gameObject.Transform.Scale = Vector2.One;
            Setup.Camera.Zoom = CameraZoom;

            if (Dirty && !FN_Editor.EditorScene.IsThisTheEditor)
            {
                Dirty = false;
                ResolutionIndependentRenderer.SetVirtualResolution(TargetResolution.X, TargetResolution.Y);
                ResolutionIndependentRenderer.SetResolution(ScreenSize.X, ScreenSize.Y, IsFullScreen);
                ResolutionIndependentRenderer.Init(ref Setup.graphics);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visualize)
                HitBoxDebuger.DrawNonFilledRectangle(new Rectangle(gameObject.Transform.Position.ToPoint() - new Point((int)(TargetResolution.X * 0.5f), (int)(TargetResolution.Y * 0.5f)), TargetResolution), 2);
        }
    }
}
