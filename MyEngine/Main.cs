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
        GameObject Image, Image2, Arrow, Clone, mouse;
        Animation idle, run;
        Animator AM;
        Vector2 MousePos;
        GameObject canvas;
        SpriteFont spriteFont;


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
            base.Initialize();
        }

        private void ImportantIntialization()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Setup.Initialize(graphics, Content, spriteBatch, RIR, Window);
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
            /////////Camera And Resolution Independent Renderer/////// -> Mandatory
            Camera = new Camera2D(RIR);
            Camera.Zoom = 1f;
            Camera.Position = new Vector2(RIR.VirtualWidth / 2, RIR.VirtualHeight / 2);

            RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ImportantIntialization();
            //////////////////////////////////////////////////////////
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Font");

            Image = new GameObject();
            Image.AddComponent<Transform>(new Transform());
            Image.GetComponent<Transform>().Scale = Vector2.One * 0.25f;
            Image.AddComponent<SpriteRenderer>(new SpriteRenderer());

            mouse = new GameObject();
            MousePos = Vector2.Zero;
            mouse.AddComponent<Transform>(new Transform());
            mouse.AddComponent<TrailRenderer>(new TrailRenderer());

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
            Image.AddComponent<AudioSource>(new AudioSource("Jump"));
            Image.AddComponent<TrailRenderer>(new TrailRenderer());
            Image.GetComponent<Rigidbody2D>().IsKinematic = false;
            //Image.GetComponent<Rigidbody2D>().Velocity = new Vector2(0, 1);
            MediaSource.LoadTrack("AMB_WR_BirdWind");
            //MediaSource.Play();
            MediaSource.IsLooping = true;

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
            backgroundSprite.Layer = 1f;

            Arrow = new GameObject();
            Arrow.AddComponent<Transform>(new Transform());
            Arrow.Parent = Image;
            Arrow.Tag = "Arrow";
            Arrow.GetComponent<Transform>().Scale = Vector2.One * 1f;
            Arrow.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Arrow.GetComponent<SpriteRenderer>().Sprite = new Sprite(Arrow.GetComponent<Transform>());
            Arrow.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Arrow");
            Arrow.GetComponent<SpriteRenderer>().Sprite.Origin = new Vector2(Arrow.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Width / 2, Arrow.GetComponent<SpriteRenderer>().Sprite.SourceRectangle.Height/2);
            Arrow.GetComponent<Transform>().Position = new Vector2(RIR.VirtualWidth / 2, RIR.VirtualHeight/2) / Transform.PixelsPerUnit;
            Arrow.AddComponent<ParticleEffect>(new ParticleEffect());
            Arrow.GetComponent<ParticleEffect>().CustomTexture = null;
            Arrow.GetComponent<ParticleEffect>().ParticleSize = 50;
            Arrow.GetComponent<ParticleEffect>().Rotation = 390;
            Arrow.AddComponent<PropertiesAnimator>(new PropertiesAnimator());
            Arrow.GetComponent<PropertiesAnimator>().AddKeyFrame(new KeyFrame(0, MathHelper.ToRadians(360), 2, "Rotate360"), true);
            /////////////////////////////
            //Clone = GameObject.Instantiate(Image);
            canvas = new GameObject();
            canvas.AddComponent<Transform>(new Transform());
            //canvas.AddComponent<Canvas>(new Canvas());

            TempScene = new Scene("Temp", 0);
            TempScene.AddGameObject(Image);
            //TempScene.AddGameObject(Clone);
            TempScene.AddGameObject(Image2);
            TempScene.AddGameObject(Background);
            TempScene.AddGameObject(Arrow);
            TempScene.AddGameObject(mouse);
            //TempScene.AddGameObject(canvas);
            SceneManager.Start();
            SceneManager.AddScene(TempScene);
            SceneManager.LoadScene(0);

            SceneManager.GetActiveScene().Start();
            //Arrow.GetComponent<SpriteRenderer>().Sprite.Origin = Arrow.GetComponent<SpriteRenderer>().Sprite.Texture.Bounds.Center.ToVector2();
            //Image.GetComponent<TrailRenderer>().OffsetPosition = new Vector2(Image.GetComponent<BoxCollider2D>().Bounds.Width / 2, Image.GetComponent<BoxCollider2D>().Bounds.Height / 2) / Transform.PixelsPerUnit;
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

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                Arrow.GetComponent<Transform>().Move(Vector2.One * (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                Arrow.GetComponent<Transform>().Move(-Vector2.One * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Image.GetComponent<Transform>().Scale += (float)gameTime.ElapsedGameTime.TotalSeconds * Vector2.One;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                Image.GetComponent<Transform>().Scale -= (float)gameTime.ElapsedGameTime.TotalSeconds * Vector2.One;

            if (Input.GetMouseClickUp(MouseButtons.LeftClick))
                Arrow.Parent = null;

            if (Input.GetKeyDown(Keys.C))
                Arrow.Transform.LocalScale = Vector2.One;

            if (Input.GetKeyDown(Keys.Space))
                Arrow.Parent = Image;

            //Arrow.GetComponent<PropertiesAnimator>().GetKeyFrame("Rotate360").GetFeedback(ref Arrow.Transform.Rotation);

            TempScene.Update(gameTime);

            mouse.GetComponent<Transform>().Position = Input.GetMousePosition() / Transform.PixelsPerUnit;

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

            TempScene.Draw(spriteBatch);
            //spriteBatch.DrawString(spriteFont, ((int)(1/(float)gameTime.ElapsedGameTime.TotalSeconds)).ToString(), Vector2.Zero, Color.Red);
            spriteBatch.DrawString(spriteFont, Image.GetComponent<Transform>().Position.ToString(), Vector2.Zero, Color.Red);
            //HitBoxDebuger.DrawRectangle(Image.GetComponent<BoxCollider2D>().GetDynamicCollider());
            HitBoxDebuger.DrawNonFilledRectangle(Image2.GetComponent<BoxCollider2D>().GetDynamicCollider());
            //HitBoxDebuger.DrawLine(new Rectangle((int)(Image.GetComponent<Transform>().Position.X * Transform.PixelsPerUnit), (int)(Image.GetComponent<Transform>().Position.Y * Transform.PixelsPerUnit), (int)(Arrow.GetComponent<Transform>().Position.Length() * Transform.PixelsPerUnit), 10), Color.White, MathHelper.ToDegrees((float)MathCompanion.Atan(Arrow.GetComponent<Transform>().Position.Y - Image.GetComponent<Transform>().Position.Y, Arrow.GetComponent<Transform>().Position.X - Image.GetComponent<Transform>().Position.X)));

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}