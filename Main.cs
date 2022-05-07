using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.IO;

namespace FN_Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera2D Camera;
        ////////<Variables>/////
        public static SpriteFont spriteFont;

        private Vector2 PrevRes = Vector2.Zero;
        ////////////////////////

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // use reflection to figure out if Shader.Profile is OpenGL (0) or DirectX (1)
            //var mgAssembly = Assembly.GetAssembly(typeof(Game));
            //var shaderType = mgAssembly.GetType("Microsoft.Xna.Framework.Graphics.Shader");
            //var profileProperty = shaderType.GetProperty("Profile");
            //var value = (int)profileProperty.GetValue(null);
            //var extension = value == 1 ? "dx11" : "ogl";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = true;
            GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            graphics.ApplyChanges();

            base.Initialize();
        }

        private void ImportantIntialization()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            /////////Camera And Resolution Independent Renderer/////// -> Mandatory
            Camera = new Camera2D();
            Camera.Zoom = 1f;

            Setup.Initialize(graphics, Content, spriteBatch, Window, Camera, this);

            ResolutionIndependentRenderer.Init(ref graphics);
            ResolutionIndependentRenderer.SetVirtualResolution(1366, 768);
            ResolutionIndependentRenderer.SetResolution(1366, 768, false);

            PrevRes = new Vector2(1366, 768);

            Exiting += SerializeBeforeExit;
            AppDomain.CurrentDomain.UnhandledException += CleanUp;
            Window.ClientSizeChanged += ScreenSizeChanged;
        }

        private void ScreenSizeChanged(object sender, EventArgs args)
        {
            ResolutionIndependentRenderer.SetResolution(Setup.GameWindow.ClientBounds.Width, Setup.GameWindow.ClientBounds.Height, false);
            ResolutionIndependentRenderer.SetVirtualResolution(Setup.GameWindow.ClientBounds.Width, Setup.GameWindow.ClientBounds.Height);
            //ResolutionIndependentRenderer.Init(ref graphics);

            Scene.ImGUI_RenderTarget = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            Light.RenderTarget2D = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            Light.ShadowMap = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None); //Depth is not needed
            Light.RecalculateAspectRatio();
            SceneManager.SceneTexPtr = Scene.GuiRenderer.BindTexture(Light.RenderTarget2D);
        }

        private void SerializeBeforeExit(object sender, EventArgs args)
        {
            SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
        }

        //Clean and save upon crashing
        private void CleanUp(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText(Environment.CurrentDirectory + "\\CrashLog.txt", "Message => " + ((Exception)e.ExceptionObject).Message + "\n\n" + "Stack Trace => " + ((Exception)e.ExceptionObject).StackTrace);

            if(SceneManager.ActiveScene != null)
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SceneManager.ShouldUpdate = false;
            ImportantIntialization();

            FN_Editor.EditorScene.IsThisTheEditor = true;

            GameObject EngineStartUp = new GameObject(true);
            EngineStartUp.AddComponent(new FN_Project.VisualizeEngineStartup());

            EngineStartUp.Start();

            // This bit of code handles the directories from a PC to another
            string WorkingDirectory = FN_Project.VisualizeEngineStartup.GamePath;
            //foreach (string S in Environment.CurrentDirectory.Split('\\'))
            //{
            //    if (S == "bin")
            //    {
            //        WorkingDirectory = WorkingDirectory.Remove(WorkingDirectory.Length - 1, 1);
            //        break;
            //    }

            //    WorkingDirectory += S + '\\';
            //}
            Environment.CurrentDirectory = WorkingDirectory + @"\Content";
            Setup.SourceFilePath = Environment.CurrentDirectory;
            Setup.IntermediateFilePath = Setup.SourceFilePath + @"\obj\DesktopGL";
            Setup.OutputFilePath = WorkingDirectory + @"\bin\Debug\netcoreapp3.1\Content";
            Content.RootDirectory = Setup.OutputFilePath;

            Setup.ConfigurePipelineMG();

            if (FN_Project.VisualizeEngineStartup.NewProject)
            {
                Utility.BuildAllContent(Setup.SourceFilePath);
                SceneManager.ActiveScene = new Scene("DefaultScene");

                GameObject GO = new GameObject(true);
                GO.Name = "EditorGameObject";
                GO.Layer = -1;
                GO.AddComponent(new FN_Editor.GameObjects_Tab());
                GO.AddComponent(new FN_Editor.InspectorWindow());
                GO.AddComponent(new FN_Editor.ContentWindow());
                GO.AddComponent(new FN_Editor.GizmosVisualizer());
                GO.AddComponent(new FN_Editor.EditorScene());
                GO.AddComponent(new FN_Editor.AnimationEditor());
                GO.AddComponent(new FN_Editor.TilemapEditor());

                GameObject CamerContr = new GameObject();
                CamerContr.Name = "Camera Controller";
                CamerContr.AddComponent(new Transform());
                CamerContr.AddComponent(new CameraController());

                SceneManager.ActiveScene.AddGameObject_Recursive(GO);
                SceneManager.ActiveScene.AddGameObject_Recursive(CamerContr);

                SceneManager.Initialize();

                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
            }
            ///////////////////////////////////

            //Setup.Content.RootDirectory = Environment.CurrentDirectory;

            SceneManager.LoadScene_Serialization("Scenes\\" + "DefaultScene");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.GetState(); //This has to be called at the start of update method!!

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            ///////////////////////////////////////

            if (Input.GetKey(Keys.Z, KeyboardFlags.SHIFT))
                Camera.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (Input.GetKey(Keys.X, KeyboardFlags.SHIFT))
                Camera.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //passing a property as a refrence using delegates
            //Arrow.GetComponent<PropertiesAnimator>().GetKeyFrame("Rotate360").GetFeedback(value => Arrow.Transform.Rotation = value);

            if (Input.GetKeyUp(Keys.O, KeyboardFlags.CTRL))
                SceneManager.SerializeScene("Scenes\\DefaultScene");

            if (Input.GetMouseClickUp(MouseButtons.MouseWheelClick))
                Setup.Camera.Zoom = 1;

            if (Input.GetKeyUp(Keys.R, KeyboardFlags.CTRL))
                SceneManager.LoadScene_Serialization("Scenes\\DefaultScene");

            SceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SceneManager.Draw(gameTime);
            
            base.Draw(gameTime);
        }
    }
}