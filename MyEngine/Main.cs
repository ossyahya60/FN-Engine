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
        Scene TempScene;
        Vector2 Resolution;
        GameObject Image, Image2, Arrow, Clone;
        Animation idle, run;
        Animator AM;

        Sprite IDLE, RUN;
        ////////////////////////

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            RIR = new ResolutionIndependentRenderer(this);

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
            Setup.Initialize(graphics, Content);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
            /////////Camera And Resolution Independent Renderer/////// -> Mandatory
            Camera = new Camera2D(RIR);
            Camera.Zoom = 1f;
            Camera.Position = new Vector2(RIR.VirtualWidth / 2, RIR.VirtualHeight / 2);

            RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);
            //////////////////////////////////////////////////////////
            // TODO: use this.Content to load your game content here
            Image = new GameObject();
            Image.AddComponent<Transform>(new Transform());
            Image.GetComponent<Transform>().Scale = Vector2.One * 0.25f;
            Image.AddComponent<SpriteRenderer>(new SpriteRenderer());

            IDLE = new Sprite(Image.GetComponent<Transform>());
            IDLE.LoadTexture("CharacterIdleAnimation");
            RUN = new Sprite(Image.GetComponent<Transform>());
            RUN.LoadTexture("CharacterRunningAnimation");
            IDLE.SourceRectangle.Width = IDLE.SourceRectangle.Width / 2;
            RUN.SourceRectangle.Width = RUN.SourceRectangle.Width / 2;
            Image.GetComponent<SpriteRenderer>().Sprite = IDLE;

            idle = new Animation(IDLE, 2);
            idle.Tag = "Idle";
            run = new Animation(RUN, 2);
            run.Tag = "Run";

            AM = new Animator();
            AM.AnimationClips.Add(idle);
            AM.AnimationClips.Add(run);
            AM.PlayWithTag("Run");
            run.Speed = 0.5f;

            Image.AddComponent<Animator>(AM);

            Image.AddComponent<Rigidbody2D>(new Rigidbody2D());
            Image.GetComponent<Rigidbody2D>().AffectedByGravity = false;
            Image.GetComponent<Rigidbody2D>().AffectedByLinearDrag = true;
            Image.AddComponent<BoxCollider2D>(new BoxCollider2D());

            ////////////////////////////
            Image2 = new GameObject();
            Image2.AddComponent<Transform>(new Transform());
            Image2.GetComponent<Transform>().Scale = Vector2.One * 0.5f;
            Image2.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Image2.GetComponent<SpriteRenderer>().Sprite = IDLE;
            Image2.GetComponent<Transform>().Position = new Vector2(9, 0);
            Image2.AddComponent<BoxCollider2D>(new BoxCollider2D());


            GameObject Background = new GameObject();
            Background.AddComponent<Transform>(new Transform());
            Background.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Sprite backgroundSprite = new Sprite(Background.GetComponent<Transform>());
            backgroundSprite.LoadTexture("SkyAndClouds");
            backgroundSprite.SourceRectangle = new Rectangle(0, 0, backgroundSprite.Texture.Width / 2, backgroundSprite.Texture.Height);
            Background.GetComponent<SpriteRenderer>().Sprite = backgroundSprite;
            backgroundSprite.Layer = 0.1f;

            Arrow = new GameObject();
            Arrow.AddComponent<Transform>(new Transform());
            //Arrow.GetComponent<Transform>().Scale = Vector2.One * 0.5f;
            Arrow.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Arrow.GetComponent<SpriteRenderer>().Sprite = new Sprite(Arrow.GetComponent<Transform>());
            Arrow.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Arrow");
            Arrow.GetComponent<SpriteRenderer>().Sprite.Origin = new Vector2(Arrow.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Width / 2, Arrow.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Height);
            Arrow.GetComponent<Transform>().Position = new Vector2(RIR.VirtualWidth / 2, RIR.VirtualHeight) / Transform.PixelsPerUnit;
            /////////////////////////////
            //Clone = GameObject.Instantiate(Image);

            TempScene = new Scene("Temp", 0);
            TempScene.AddGameObject(Image);
            //TempScene.AddGameObject(Clone);
            TempScene.AddGameObject(Image2);
            TempScene.AddGameObject(Background);
            TempScene.AddGameObject(Arrow);
            SceneManager.Start();
            SceneManager.AddScene(TempScene);
            SceneManager.LoadScene(0);

            TempScene.Start();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            /////////Resolution related//////////// -> Mandatory
            if (Resolution != new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight))
                RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);

            Resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ///////////////////////////////////////
            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Image.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.2f, 0), ForceMode2D.Impulse);
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
                Image.GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.2f, 0), ForceMode2D.Impulse);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Image.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -0.2f), ForceMode2D.Impulse);
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
                Image.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 0.2f), ForceMode2D.Impulse);

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                Camera.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (Keyboard.GetState().IsKeyDown(Keys.X))
                Camera.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            TempScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            RIR.BeginDraw(); //Resolution related -> Mandatory
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Camera.GetViewTransformationMatrix()); // -> Mandatory

            //HitBoxDebuger.DrawRectangle(GraphicsDevice, spriteBatch, Image.GetComponent<BoxCollider2D>().GetDynamicCollider());
            //HitBoxDebuger.DrawRectangle(GraphicsDevice, spriteBatch, Image2.GetComponent<BoxCollider2D>().GetDynamicCollider());
            TempScene.Draw(spriteBatch);
            SpriteFont spriteFont = Content.Load<SpriteFont>("Font");
            spriteBatch.DrawString(spriteFont, Image.GetComponent<Transform>().Position.X.ToString(), Vector2.Zero, Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}