using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Vector2 Resolution;
        SpriteFont spriteFont;
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

            SceneManager.Start();

            SceneManager.AddInitializer(MainScene, 0);
            //////////////////////////////////////////////////////////
            SceneManager.LoadScene(new Scene("MainScene", 0)); //Main Scene
        }

        private void MainScene()
        {
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Font");

            GameObject Arrow1 = new GameObject();
            Arrow1.AddComponent<Transform>(new Transform());
            Arrow1.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Arrow1.AddComponent<Rigidbody2D>(new Rigidbody2D());
            Arrow1.GetComponent<SpriteRenderer>().Sprite = new Sprite(Arrow1.Transform);
            Arrow1.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Arrow");
            Arrow1.Name = "Arrow1";

            GameObject Circle = new GameObject();
            Circle.AddComponent<Transform>(new Transform());
            Circle.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Circle.GetComponent<SpriteRenderer>().Sprite = new Sprite(Circle.Transform);
            Circle.GetComponent<SpriteRenderer>().Sprite.Texture = HitBoxDebuger.CreateCircleTextureShell(graphics.PreferredBackBufferHeight/2, (int)(0.9f*graphics.PreferredBackBufferHeight / 2), Color.Red);

            //SceneManager.ActiveScene.AddGameObject(Circle);
            SceneManager.ActiveScene.AddGameObject(Arrow1);

            SceneManager.ActiveScene.Start();

            //Initialization here
            Arrow1.GetComponent<Rigidbody2D>().AffectedByGravity = false;
            GameObject Arrow2 = GameObject.Instantiate(Arrow1);
            Arrow2.Name = "Arrow2";

            Arrow1.AddChild(Arrow2);

            Arrow2.Transform.Position = Vector2.One * 100;
            Arrow2.Transform.LocalScale = Vector2.One;

            Arrow1.GetComponent<Rigidbody2D>().LinearDragScale = 2;

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
            if (Input.GetKey(Keys.W))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").Transform.Scale += Vector2.One * (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
            if (Input.GetKey(Keys.S))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").Transform.Scale -= Vector2.One * (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

            if (Input.GetKeyDown(Keys.P))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow2").Transform.Scale = 0.5f * Vector2.One;

            if (Input.GetKeyUp(Keys.R))
                SceneManager.LoadScene(SceneManager.ActiveScene);

            if(Input.GetKey(Keys.F))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").GetComponent<Rigidbody2D>().AddForce(Vector2.UnitY * 60 * 4, ForceMode2D.Force);

            SceneManager.ActiveScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            RIR.BeginDraw(); //Resolution related -> Mandatory
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.GetViewTransformationMatrix()); // -> Mandatory

            SceneManager.ActiveScene.Draw(spriteBatch);
            spriteBatch.DrawString(spriteFont, SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").GetComponent<Rigidbody2D>().Velocity.ToString(), Vector2.Zero, Color.Red);

            //spriteBatch.DrawString(spriteFont, ((int)(1/this.TargetElapsedTime.TotalSeconds)).ToString(), Vector2.Zero, Color.Red); =>FPS

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}