using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MyEngine.FN_Project
{
    internal class VisualizeEngineStartup : GameObjectComponent
    {
        private static bool FirstTimeOnThisMachine = false;
        private string SelectedPath = "";
        private static bool FirstBootUp = true;

        public override void Start()
        {
            // Download the right template version of Monogame
            // Download the necessary nuget packages
            // Replace Game1.cs file with a one you create on runtime altering the name of namespace (Text here then dump it to a .cs file)
            if (FirstBootUp)
            {
                if (FirstTimeOnThisMachine)
                    DownloadPrerequisites();

                FirstBootUp = false;
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        if (Directory.GetFiles(fbd.SelectedPath).Length == 0 && Directory.GetDirectories(fbd.SelectedPath).Length == 0) // Make New Project
                            MakeNewProject(fbd.SelectedPath);
                        else
                            OpenExistingProject(fbd.SelectedPath);

                        SelectedPath = fbd.SelectedPath;
                    }
                    else if (result == DialogResult.Cancel)
                        Setup.Game.Exit();

                    //Check if DialogResult is cancel and act accordingly
                }
            }
        }

        public override void DrawUI()
        {
            ImGui.Begin("Manage Projects");
            ImGui.Text(SelectedPath);
            ImGui.End();
        }

        private void DownloadPrerequisites()
        {
            string[] Commands = new string[]
            {
                "dotnet new --install MonoGame.Templates.CSharp::3.8.0.1641"
            };

            ExecuteCommand(Commands, "");
        }

        private void MakeNewProject(string Path)
        {
            string ProjName = Path.Split('\\')[(Path.Split('\\').Length - 1)] + ".csproj";
            string[] MakeDesktopGLProj = new string[]
            {
                //"dotnet new -i MonoGame.Templates.CSharp::3.8.0.1641", //Download Template
                "cd " + Path, //Go to the new directory
                "dotnet new mgdesktopgl", //DesktopGL
                "dotnet new sln", //Make a new visual studio solution
                "dotnet sln add " + ProjName, //Add the project to the solution
                "mgcb-editor --register", //Register the content in MGCB
                "mgcb-editor", //Open MGCB Editor (Optional)
            };

            ExecuteCommand(MakeDesktopGLProj, Path);

            string NameSpaceName = Path.Substring(Path.LastIndexOf('\\') + 1);
            File.WriteAllText(Path + @"\" + NameSpaceName + ".csproj", GetTemplateProjFile());
            File.WriteAllText(Path + @"\" + "Game1.cs", GetTemplateMainFile(NameSpaceName));
        }

        private void OpenExistingProject(string Path)
        {
            string SlnName = Path.Split('\\')[(Path.Split('\\').Length - 1)] + ".sln";
            string[] OpenProj = new string[]
            {
                //"dotnet new -i MonoGame.Templates.CSharp::3.8.0.1641", //Download Template
                "cd " + Path, //Go to the existing directory
                "start " + SlnName,
                "mgcb-editor --register", //Register the content in MGCB
                "mgcb-editor", //Open MGCB Editor (Optional)
            };

            ExecuteCommand(OpenProj, Path);
        }

        // This function is from a stackoverflow question:
        // URL: https://stackoverflow.com/questions/437419/execute-multiple-command-lines-with-the-same-process-using-net
        private void ExecuteCommand(string[] Commands, string Path)
        {
            string batFileName = Path + @"\" + Guid.NewGuid() + ".bat";

            using (StreamWriter batFile = new StreamWriter(batFileName))
            {
                foreach (string Comm in Commands)
                    batFile.WriteLine(Comm);
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + batFileName);
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;

            Process p = new Process();
            p.StartInfo = processStartInfo;
            p.Start();
            p.WaitForExit();

            File.Delete(batFileName);
        }

        private string GetTemplateProjFile()
        {
            return "<Project Sdk=\"Microsoft.NET.Sdk\">" + "\n" +
                   "<PropertyGroup>" + "\n" +
                   "<OutputType>WinExe</OutputType>" + "\n" +
                   "<TargetFramework>net462</TargetFramework>" + "\n" +
                   "<MonoGamePlatform>DesktopGL</MonoGamePlatform>" + "\n" +
                   "<PublishReadyToRun>false</PublishReadyToRun>" + "\n" +
                   "<TieredCompilation>false</TieredCompilation>" + "\n" +
                   "</PropertyGroup>" + "\n" +
                   "<PropertyGroup>" + "\n" +
                   "<ApplicationManifest>app.manifest</ApplicationManifest>" + "\n" +
                   "<ApplicationIcon>Icon.ico</ApplicationIcon>" + "\n" +
                   "</PropertyGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<EmbeddedResource Include=\"Icon.ico\" />" + "\n" +
                   "<EmbeddedResource Include=\"Icon.bmp\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   @"<MonoGameContentReference Include=""Content\Content.mgcb"" Visible=""false"" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<TrimmerRootAssembly Include=\"Microsoft.Xna.Framework.Content.ContentTypeReader\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<PackageReference Include=\"MonoGame.Framework.DesktopGL\" Version=\"3.7.1.189\" />" + "\n" +
                   "<PackageReference Include=\"MonoGame.Content.Builder\" Version=\"3.7.* \" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "</Project>";
        }

        private string GetTemplateMainFile(string NameSpace)
        {
            return "using Microsoft.Xna.Framework;" + "\n" +
            "using Microsoft.Xna.Framework.Graphics;" + "\n" +
            "using Microsoft.Xna.Framework.Input;" + "\n" +
            "using System;" + "\n" +
            "using MyEngine;" + "\n" +
            "\n" +
            "namespace " + NameSpace + "\n" +
            "{" + "\n" +
            "    /// <summary>" + "\n" +
            "    /// This is the main type for your game." + "\n" +
            "    /// </summary>" + "\n" +
            "    public class Game1 : Game" + "\n" +
            "    {" + "\n" +
            "        GraphicsDeviceManager graphics;" + "\n" +
            "        SpriteBatch spriteBatch;" + "\n" +
            "        ResolutionIndependentRenderer RIR;" + "\n" +
            "        Camera2D Camera;" + "\n" +
            "        ////////<Variables>/////" + "\n" +
            "        public static SpriteFont spriteFont;" + "\n" +
            "        ////////////////////////" + "\n" +
            "\n" +
            "        public Game1()" + "\n" +
            "        {" + "\n" +
            "            graphics = new GraphicsDeviceManager(this);" + "\n" +
            "            Content.RootDirectory = \"Content\";" + "\n" +
            "            RIR = new ResolutionIndependentRenderer(this);" + "\n" +
            "            IsMouseVisible = true;" + "\n" +
            "            Window.AllowUserResizing = false;" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        /// <summary>" + "\n" +
            "        /// Allows the game to perform any initialization it needs to before starting to run." + "\n" +
            "        /// This is where it can query for any required services and load any non-graphic" + "\n" +
            "        /// related content.  Calling base.Initialize will enumerate through any components" + "\n" +
            "        /// and initialize them as well." + "\n" +
            "        /// </summary>" + "\n" +
            "        protected override void Initialize()" + "\n" +
            "        {" + "\n" +
            "            // TODO: Add your initialization logic here" + "\n" +
            "            graphics.GraphicsProfile = GraphicsProfile.HiDef;" + "\n" +
            "            graphics.SynchronizeWithVerticalRetrace = true;" + "\n" +
            "            graphics.PreferMultiSampling = true;" + "\n" +
            "            GraphicsDevice.PresentationParameters.MultiSampleCount = 8;" + "\n" +
            "            graphics.ApplyChanges();" + "\n" +
            "    \n" +
            "            base.Initialize();" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        private void ImportantIntialization()" + "\n" +
            "        {" + "\n" +
            "            // Create a new SpriteBatch, which can be used to draw textures." + "\n" +
            "            spriteBatch = new SpriteBatch(GraphicsDevice);" + "\n" +
            "            graphics.PreferredBackBufferWidth = 1366;" + "\n" +
            "            graphics.PreferredBackBufferHeight = 768;" + "\n" +
            "            //graphics.IsFullScreen = true;" + "\n" +
            "            graphics.ApplyChanges();" + "\n" +
            "    \n" +
            "            RIR.VirtualWidth = graphics.PreferredBackBufferWidth;" + "\n" +
            "            RIR.VirtualHeight = graphics.PreferredBackBufferHeight;" + "\n" +
            "            /////////Camera And Resolution Independent Renderer/////// -> Mandatory" + "\n" +
            "            Camera = new Camera2D();" + "\n" +
            "            Camera.Zoom = 1f;" + "\n" +
            "    \n" +
            "            //You should look at this when deserializing scene for a game! (Note for the Engine Programmer)" + "\n" +
            "            Camera.Position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);" + "\n" +
            "    \n" +
            "            Setup.Initialize(graphics, Content, spriteBatch, RIR, Window, Camera, this);" + "\n" +
            "    \n" +
            "            RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        /// <summary>" + "\n" +
            "        /// LoadContent will be called once per game and is the place to load" + "\n" +
            "        /// all of your content." + "\n" +
            "        /// </summary>" + "\n" +
            "        protected override void LoadContent()" + "\n" +
            "        {" + "\n" +
            "            ImportantIntialization();" + "\n" +
            "            RIR.BackgroundColor = Color.Transparent;" + "\n" +
            "    \n" +
            "            spriteFont = Content.Load<SpriteFont>(\"Font\");" + "\n" +
            "    \n" +
            "            //SceneManager.LoadScene_Serialization(\"MainScene\");" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        /// <summary>" + "\n" +
            "        /// UnloadContent will be called once per game and is the place to unload" + "\n" +
            "        /// game-specific content." + "\n" +
            "        /// </summary>" + "\n" +
            "        protected override void UnloadContent()" + "\n" +
            "        {" + "\n" +
            "            // TODO: Unload any non ContentManager content here" + "\n" +
            "            Content.Unload();" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        /// <summary>" + "\n" +
            "        /// Allows the game to run logic such as updating the world," + "\n" +
            "        /// checking for collisions, gathering input, and playing audio." + "\n" +
            "        /// </summary>" + "\n" +
            "        /// <param name=\"gameTime\">Provides a snapshot of timing values.</param>" + "\n" +
            "        protected override void Update(GameTime gameTime)" + "\n" +
            "        {" + "\n" +
            "            Input.GetState(); //This has to be called at the start of update method!!" + "\n" +
            "    \n" +
            "            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))" + "\n" +
            "                Exit();" + "\n" +
            "    \n" +
            "            SceneManager.Update(gameTime);" + "\n" +
            "    \n" +
            "            base.Update(gameTime);" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        /// <summary>" + "\n" +
            "        /// This is called when the game should draw itself." + "\n" +
            "        /// </summary>" + "\n" +
            "        /// <param name=\"gameTime\">Provides a snapshot of timing values.</param>" + "\n" +
            "        protected override void Draw(GameTime gameTime)" + "\n" +
            "        {" + "\n" +
            "            SceneManager.Draw(gameTime);" + "\n" +
            "    \n" +
            "            base.Draw(gameTime);" + "\n" +
            "        }" + "\n" +
            "    }" + "\n" +
            "}";
        }
    }
}