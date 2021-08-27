using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

            Exiting += SerializeBeforeExit;
            //Window.ClientSizeChanged += ScreenSizeChanged;
        }

        //private void ScreenSizeChanged(object sender, EventArgs args)
        //{
        //    Setup.graphics.PreferredBackBufferWidth = Setup.GameWindow.ClientBounds.Width;
        //    Setup.graphics.PreferredBackBufferHeight = Setup.GameWindow.ClientBounds.Height;
        //    Setup.graphics.ApplyChanges();
        //    ResolutionIndependentRenderer.Init(ref graphics);
        //}

        private void SerializeBeforeExit(object sender, EventArgs args)
        {
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

            Setup.Content.Load<SpriteFont>("Font");

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
                SceneManager.ActiveScene = new Scene("DefaultScene", 0);

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
                Camera.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
            else if (Input.GetKey(Keys.X, KeyboardFlags.SHIFT))
                Camera.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

            //passing a property as a refrence using delegates
            //Arrow.GetComponent<PropertiesAnimator>().GetKeyFrame("Rotate360").GetFeedback(value => Arrow.Transform.Rotation = value);

            if (Input.GetKeyUp(Keys.O, KeyboardFlags.CTRL))
                SceneManager.SerializeScene("Scenes\\DefaultScene");

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