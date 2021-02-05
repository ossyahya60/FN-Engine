using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FN_Engine
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
        Vector2 Resolution;
        SpriteFont spriteFont;
        Effect LightTest;
        RenderTarget2D RenderTarget2D;
        ////////////////////////

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            RIR = new ResolutionIndependentRenderer(this);

            graphics.PreferMultiSampling = true;
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
            base.Initialize();
        }

        private void ImportantIntialization()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            RIR.VirtualWidth = 1366;
            RIR.VirtualHeight = 768;
            graphics.ApplyChanges();
            /////////Camera And Resolution Independent Renderer/////// -> Mandatory
            Camera = new Camera2D(RIR);
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
            ImportantIntialization();

            RenderTarget2D = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            SceneManager.Start();

            SceneManager.AddInitializer(MainScene, 0);
            //////////////////////////////////////////////////////////
            SceneManager.LoadScene(new Scene("MainScene", 0)); //Main Scene
        }

        private void MainScene()
        {
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Font");

            //GameObject Arrow1 = new GameObject();
            //Arrow1.AddComponent<Transform>(new Transform());
            //Arrow1.AddComponent<SpriteRenderer>(new SpriteRenderer());

            GameObject Test = new GameObject();
            Test.Name = "Test";
            Test.AddComponent<Transform>(new Transform());
            Test.AddComponent<SpriteRenderer>(new SpriteRenderer());

            GameObject Test2 = new GameObject();
            Test2.Name = "Test";
            Test2.AddComponent<Transform>(new Transform());
            Test2.AddComponent<SpriteRenderer>(new SpriteRenderer());

            //SceneManager.ActiveScene.AddGameObject(Arrow1);
            SceneManager.ActiveScene.AddGameObject(Test);
            //SceneManager.ActiveScene.AddGameObject(Test2);

            SceneManager.ActiveScene.Start();

            //Initialization here
            //Use matrices to make transformations!!!!

            //Arrow1.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Arrow");
            //Arrow1.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            //Arrow1.Name = "Arrow1";

            //GameObject Arrow2 = GameObject.Instantiate(Arrow1);
            //Arrow2.Transform.Position = Vector2.UnitX * 100;
            //Arrow2.Name = "Arrow2";

            //Arrow1.AddChild(Arrow2);
            //Arrow2.GetComponent<SpriteRenderer>().Sprite.Origin = -Arrow1.Transform.Position;

            Test.GetComponent<SpriteRenderer>().Sprite.LoadTexture("SubmittingGame");
            Test.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            LightTest = Content.Load<Effect>("LightTest");
            //Test.GetComponent<SpriteRenderer>().Effect = LightTest;

            //Test2.GetComponent<SpriteRenderer>().Sprite.Texture = HitBoxDebuger.CreateCircleTexture(50, Color.White);
            //Test2.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            //Test2.GetComponent<SpriteRenderer>().Effect = LightTest;

            Camera.Position = new Vector2(0, 0);

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

            /////////Resolution related//////////// -> Mandatory
            if (Resolution != new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight))
                RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);

            Resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ///////////////////////////////////////
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                Camera.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (Keyboard.GetState().IsKeyDown(Keys.X))
                Camera.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //passing a property as a refrence using delegates
            //Arrow.GetComponent<PropertiesAnimator>().GetKeyFrame("Rotate360").GetFeedback(value => Arrow.Transform.Rotation = value);
            if (Input.GetKeyUp(Keys.R))
                SceneManager.LoadScene(SceneManager.ActiveScene);

            //if (Input.GetKey(Keys.Left))
            //    SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").Transform.Rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if (Input.GetKey(Keys.Right))
            //    SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").Transform.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;

            SceneManager.ActiveScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget2D); //Render Target
            // TODO: Add your drawing code here
            RIR.BeginDraw(); //Resolution related -> Mandatory
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.GetViewTransformationMatrix()); // -> Mandatory
            SpriteRenderer.LastEffect = null; // This should be the same effect as in the begin method above

            SceneManager.ActiveScene.Draw(spriteBatch);
            //spriteBatch.DrawString(spriteFont, SceneManager.ActiveScene.FindGameObjectWithName("Arrow2").Transform.LocalPosition.ToString(), -Vector2.UnitX * graphics.PreferredBackBufferWidth/2 - Vector2.UnitY * graphics.PreferredBackBufferHeight/2, Color.Red);

            //spriteBatch.DrawString(spriteFont, ((int)(1/this.TargetElapsedTime.TotalSeconds)).ToString(), Vector2.Zero, Color.Red); =>FPS

            spriteBatch.End();

            //Render Targets
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, LightTest, Camera.GetViewTransformationMatrix());
            spriteBatch.Draw(RenderTarget2D, new Vector2(-graphics.PreferredBackBufferWidth / 2, -graphics.PreferredBackBufferHeight / 2), new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}