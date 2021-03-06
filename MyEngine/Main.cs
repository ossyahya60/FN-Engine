﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
        public static SpriteFont spriteFont;
        ////////////////////////

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            RIR = new ResolutionIndependentRenderer(this);

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
            RIR.BackgroundColor = new Color(0, 0, 0, 0);

            SceneManager.Start();

            SceneManager.AddInitializer(MainScene, 0);
            //////////////////////////////////////////////////////////
            SceneManager.LoadScene(new Scene("MainScene", 0)); //Main Scene
        }

        private void MainScene()
        {
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Font");

            //Light.CastShadows = true;
            GameObject Test = new GameObject();
            Test.Tag = "Test";
            Test.AddComponent<Transform>(new Transform());
            Test.AddComponent<SpriteRenderer>(new SpriteRenderer());
            Test.AddComponent<Light>(new Light());
            Test.AddComponent<ShadowCaster>(new ShadowCaster());

            GameObject Test6 = new GameObject();
            Test6.Tag = "Test6";
            Test6.AddComponent<Transform>(new Transform());
            Test6.AddComponent<SpriteRenderer>(new SpriteRenderer());
            //Test.AddComponent<Light>(new Light());
            Test6.AddComponent<ShadowCaster>(new ShadowCaster());

            GameObject Test3 = new GameObject();
            Test3.Tag = "Test3";
            Test3.AddComponent<Transform>(new Transform());
            Test3.AddComponent<Light>(new Light());

            GameObject Test2 = new GameObject();
            Test2.Tag = "Test2";
            Test2.AddComponent<Transform>(new Transform());
            Test2.AddComponent<SpriteRenderer>(new SpriteRenderer());

            SceneManager.ActiveScene.AddGameObject(Test);
            SceneManager.ActiveScene.AddGameObject(Test2);
            SceneManager.ActiveScene.AddGameObject(Test3);
            SceneManager.ActiveScene.AddGameObject(Test6);

            SceneManager.ActiveScene.Start();

            //Initialization here
            //Use matrices to make transformations!!!!

            Test6.GetComponent<SpriteRenderer>().Sprite.Texture = HitBoxDebuger.RectTexture(Color.Yellow);
            Test6.Transform.Scale = 100 * Vector2.One;
            Test6.Layer = 0.1f;
            Test6.Transform.Position = new Vector2(graphics.PreferredBackBufferWidth / 2, 1.2f * graphics.PreferredBackBufferHeight / 2);

            Test.GetComponent<SpriteRenderer>().Sprite.Texture = HitBoxDebuger.RectTexture(Color.Red);
            Test.Transform.Scale = 100 * Vector2.One;
            Test2.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Temp");
            Test3.GetComponent<Light>().Attenuation = 3;
            Test3.GetComponent<Light>().OuterRadius = 0.2f;
            Test.GetComponent<Light>().Type = LightTypes.Directional;
            Test.GetComponent<Light>().DirectionalIntensity = 0.4f;
            Test2.Layer = 0.5f;

            //Test2.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            Test2.Transform.Scale = Vector2.One;

            Test.Layer = 0.1f;
            Test.GetComponent<SpriteRenderer>().Sprite.SetCenterAsOrigin();
            Test.Transform.Position = new Vector2(1.5f * graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            //Camera.Position = new Vector2(0, 0);

            //GameObject Test4 = GameObject.Instantiate(Test);
            //Test4.RemoveComponent<Light>(Test4.GetComponent<Light>());
            //Test4.RemoveComponent<ShadowCaster>(Test4.GetComponent<ShadowCaster>());
            //Test4.Tag = "Test4";
            //Test4.Transform.MoveX(-100);
            //Test4.Active = true;

            //GameObject Test5 = GameObject.Instantiate(Test3);
            //Test5.Tag = "Test5";

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
            if (!this.IsActive) //Pause Game when minimized
                return;

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
                SceneManager.LoadScene(new Scene("MainScene", 0));

            if (Input.GetKey(Keys.W))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test3").Transform.MoveY(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.S))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test3").Transform.MoveY((float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.A))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test3").Transform.MoveX(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.D))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test3").Transform.MoveX((float)gameTime.ElapsedGameTime.TotalSeconds * 120);

            if (Input.GetKey(Keys.Up))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test5").Transform.MoveY(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.Down))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test5").Transform.MoveY((float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.Left))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test5").Transform.MoveX(-(float)gameTime.ElapsedGameTime.TotalSeconds * 120);
            if (Input.GetKey(Keys.Right))
                SceneManager.ActiveScene.FindGameObjectWithTag("Test5").Transform.MoveX((float)gameTime.ElapsedGameTime.TotalSeconds * 120);

            SceneManager.ActiveScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!this.IsActive) //Pause Game when minimized
                return;

            ///
            Light.Init_Light();
            RIR.BeginDraw(); //Resolution related -> Mandatory
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.GetViewTransformationMatrix()); // -> Mandatory
            SpriteRenderer.LastEffect = null; // This should be the same effect as in the begin method above
            ///
            // TODO: Add your drawing code here
            SceneManager.ActiveScene.Draw(spriteBatch);

            spriteBatch.End();

            //Render Targets //Light (Experimental)
            Light.ApplyLighting();

            //UI Rendering
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.GetViewTransformationMatrix()); // -> Mandatory

            //spriteBatch.DrawString(spriteFont, Input.GetMousePosition().ToString(), Vector2.Zero, Color.Red);
            spriteBatch.DrawString(spriteFont, ((int)(1 / this.TargetElapsedTime.TotalSeconds)).ToString(), Vector2.Zero, Color.Red); //=>FPS

            spriteBatch.End();

            base.Draw(gameTime);

            SceneManager.LoadSceneNow();
        }
    }
}