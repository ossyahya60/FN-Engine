using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
using System.Linq;

namespace MyEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ResolutionIndependentRenderer RIR;
        Camera2D Camera;
        ////////<Variables>/////
        public static SpriteFont spriteFont;
        ////////////////////////

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            RIR = new ResolutionIndependentRenderer(this);

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
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            RIR.VirtualWidth = graphics.PreferredBackBufferWidth;
            RIR.VirtualHeight = graphics.PreferredBackBufferHeight;
            /////////Camera And Resolution Independent Renderer/////// -> Mandatory
            Camera = new Camera2D();
            Camera.Zoom = 1f;
            Camera.Position = new Vector2(RIR.VirtualWidth / 2, RIR.VirtualHeight / 2);

            Setup.Initialize(graphics, Content, spriteBatch, RIR, Window, Camera, this);

            RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // This bit of code handles the directories from a PC to another
            string WorkingDirectory = "";
            foreach (string S in Environment.CurrentDirectory.Split('\\'))
            {
                if (S == "bin")
                {
                    WorkingDirectory = WorkingDirectory.Remove(WorkingDirectory.Length-1, 1);
                    break;
                }

                WorkingDirectory += S + '\\';
            }
            Environment.CurrentDirectory = WorkingDirectory + @"\Content";
            Setup.SourceFilePath = Environment.CurrentDirectory;
            Setup.IntermediateFilePath = Setup.SourceFilePath + @"\obj\Windows";
            Setup.OutputFilePath = WorkingDirectory + @"\bin\Windows\x86\Debug\Content";
            ///////////////////////////////////

            ImportantIntialization();
            RIR.BackgroundColor = new Color(0, 0, 0, 0);

            SceneManager.Start();

            //SceneManager.LoadScene_Serialization("MainScene");
            SceneManager.AddInitializer(MainScene, 0);
            //////////////////////////////////////////////////////////
            SceneManager.LoadScene(new Scene("MainScene", 0)); //Main Scene
        }

        private void MainScene()
        {
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Font");

            //SceneManager.LoadScene_Serialization("MainScene");

            //Light.CastShadows = true;
            GameObject Test = new GameObject();
            Test.Tag = "Test";
            Test.AddComponent<Transform>(new Transform());
            Test.AddComponent<SpriteRenderer>(new SpriteRenderer());
            //Test.AddComponent<Light>(new Light());
            //Test.AddComponent<ShadowCaster>(new ShadowCaster());

            GameObject Test6 = new GameObject();
            Test6.Name = "Test 6";
            Test6.Tag = "Test6";
            Test6.AddComponent<Transform>(new Transform());
            Test6.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Test6.AddComponent<Light>(new Light());
            //Test6.AddComponent<TrailRenderer>(new TrailRenderer());
            //Test6.AddComponent<ShadowCaster>(new ShadowCaster());

            GameObject Test3 = new GameObject();
            Test3.Tag = "Test3";
            Test3.AddComponent<Transform>(new Transform());
            Test3.AddComponent<Light>(new Light());

            GameObject Test2 = new GameObject();
            Test2.Name = "Test2";
            Test2.Tag = "Test2";
            Test2.AddComponent<Transform>(new Transform());
            Test2.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Test2.AddComponent<AudioSource>(new AudioSource("FireWorks_M"));

            GameObject GameObjectsTab = new GameObject(true);
            GameObjectsTab.Name = "GameObjectsTab";
            GameObjectsTab.AddComponent(new FN_Editor.GameObjects_Tab());

            GameObject InspectorWindow = new GameObject(true);
            GameObjectsTab.Name = "InspectorWindow";
            GameObjectsTab.AddComponent(new FN_Editor.InspectorWindow());

            GameObject ContentManager = new GameObject(true);
            GameObjectsTab.Name = "ContentManager";
            GameObjectsTab.AddComponent(new FN_Editor.ContentWindow());

            //SceneManager.ActiveScene.AddGameObject_Recursive(Test);
            SceneManager.ActiveScene.AddGameObject_Recursive(Test2);
            //SceneManager.ActiveScene.AddGameObject_Recursive(Test3);
            SceneManager.ActiveScene.AddGameObject_Recursive(Test6);
            SceneManager.ActiveScene.AddGameObject_Recursive(GameObjectsTab);
            SceneManager.ActiveScene.AddGameObject_Recursive(InspectorWindow);
            SceneManager.ActiveScene.AddGameObject_Recursive(ContentManager);

            SceneManager.ActiveScene.Start();

            //Initialization here
            //Use matrices to make transformations!!!!
            //Type FI = typeof(GameObject).GetMember("IsActive")[0];

            //GameObjectsTab.Transform.Scale = Vector2.One * 200;
            Test6.GetComponent<SpriteRenderer>().Sprite.Texture = HitBoxDebuger.RectTexture(Color.Yellow);
            Test6.Transform.Scale = 100 * Vector2.One;
            Test6.Layer = 0.1f;
            Test6.Transform.Position = new Vector2(graphics.PreferredBackBufferWidth / 2, 1.2f * graphics.PreferredBackBufferHeight / 2);

            GameObject Test6_Inst = GameObject.Instantiate(Test6);
            Test6_Inst.Name = "Test6_Inst";
            Test6.AddChild(Test6_Inst);

            //Test.GetComponent<SpriteRenderer>().Sprite.Texture = HitBoxDebuger.RectTexture(Color.Red);
            //Test.Transform.Scale = 100 * Vector2.One;
            Test2.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Temp");

            //GameObject Test6_Inst2 = GameObject.Instantiate(Test2);
            //Test6_Inst2.Name = "Test6_Inst2";

            //GameObject Test6_Inst3 = GameObject.Instantiate(Test2);
            //Test6_Inst.AddChild(Test6_Inst3);

            //Make Serialization and Deserialization Generic

            //var FIELD_FLOAT = Utility.GetInstance(typeof(float).FullName);

            //Test6.Active = false;

            ////Test3.GetComponent<Light>().Attenuation = 3;
            ////Test3.GetComponent<Light>().OuterRadius = 0.2f;
            ////Test.GetComponent<Light>().Type = LightTypes.Directional;
            ////Test.GetComponent<Light>().DirectionalIntensity = 0.4f;
            //Test2.Layer = 0.5f;

            ////Test2.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            //Test2.Transform.Scale = Vector2.One;

            //Test.Layer = 0.1f;
            //Test.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            //Test.Transform.Position = new Vector2(1.5f * graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            //Camera.Position = new Vector2(0, 0);

            //GameObject Test4 = GameObject.Instantiate(Test);
            //Test4.RemoveComponent<Light>(Test4.GetComponent<Light>());
            //Test4.RemoveComponent<ShadowCaster>(Test4.GetComponent<ShadowCaster>());
            //Test4.Tag = "Test4";
            //Test4.Transform.MoveX(-100);
            //Test4.Active = true;

            //GameObject Test5 = GameObject.Instantiate(Test3);
            //Test5.Tag = "Test5";

            //GameObject.Instantiate(Test2).Name = "Test2_1";

            Test6.GetComponent<SpriteRenderer>().Effect = Content.Load<Effect>("ShadowCasting");

            SceneManager.ActiveScene.SortGameObjectsWithLayer();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.GetState(); //This has to be called at the start of update method!!

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ///////////////////////////////////////
            if (Input.GetKey(Keys.Z, KeyboardFlags.SHIFT))
                Camera.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (Input.GetKey(Keys.X, KeyboardFlags.SHIFT))
                Camera.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //passing a property as a refrence using delegates
            //Arrow.GetComponent<PropertiesAnimator>().GetKeyFrame("Rotate360").GetFeedback(value => Arrow.Transform.Rotation = value);
            if (Input.GetKey(Keys.W, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test 6").Transform.MoveY(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.S, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test 6").Transform.MoveY((float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.A, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test 6").Transform.MoveX(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.D, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test 6").Transform.MoveX((float)gameTime.ElapsedGameTime.TotalSeconds * 120);

            if (Input.GetKeyUp(Keys.O, KeyboardFlags.CTRL))
                SceneManager.ActiveScene.Serialize(true);

            if (Input.GetKeyUp(Keys.R, KeyboardFlags.CTRL))
                SceneManager.LoadScene_Serialization("MainScene");

            if (Input.GetKey(Keys.Up, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test6_Inst").Transform.MoveY(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.Down, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test6_Inst").Transform.MoveY((float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.Left, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test6_Inst").Transform.MoveX(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.Right, KeyboardFlags.SHIFT))
                SceneManager.ActiveScene.FindGameObjectWithName("Test6_Inst").Transform.MoveX((float)gameTime.ElapsedGameTime.TotalSeconds * 120);

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