using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;

namespace FN_Engine.FN_Project
{
    internal class VisualizeEngineStartup: GameObjectComponent
    {
        public static string GamePath = "";
        public static bool NewProject = false;
        public static string GameName = "";

        private static bool FirstTimeOnThisMachine = false;
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

                GamePath = MakeForm.CreateOrOpenProj();
            }
        }

        //public override void DrawUI()
        //{
        //    ImGui.Begin("Manage Projects");
        //    ImGui.Text(GamePath);
        //    ImGui.End();
        //}

        private void DownloadPrerequisites()
        {
            string[] Commands = new string[]
            {
                "dotnet new --install MonoGame.Templates.CSharp::3.8.0.1641"
            };

            Utility.ExecuteCommand(Commands, "");
        }

        public static void MakeNewProject(string Path)
        {
            string ProjName = Path.Split('\\')[(Path.Split('\\').Length - 1)] + ".csproj";
            string[] MakeDesktopGLProj = new string[]
            {
                //"dotnet new -i MonoGame.Templates.CSharp::3.8.0.1641", //Download Template
                "cd " + Path, //Go to the new directory
                "dotnet new mgdesktopgl", //DesktopGL
                "dotnet new sln", //Make a new visual studio solution
                "dotnet sln add " + ProjName, //Add the project to the solution
            };

            //Change this accord to which platform you will target!
            Utility.ExecuteCommand(MakeDesktopGLProj, Path);

            string[] RunGame = new string[]
            {
                "cd " + Path, //Go to the new directory
                "dotnet build"//,
                //"dotnet run"
            };

            Utility.ExecuteCommand(RunGame, Path);

            //Copying DLLs
            string GameDebugDir = Path + "\\bin\\Debug\\netcoreapp3.1";
            File.Copy(Environment.CurrentDirectory + "\\FN_Engine.dll", GameDebugDir + "\\FN_Engine.dll");
            File.Copy(Environment.CurrentDirectory + "\\MonoGame.Framework.Content.Pipeline.dll", GameDebugDir + "\\MonoGame.Framework.Content.Pipeline.dll");
            File.Copy(Environment.CurrentDirectory + "\\Newtonsoft.Json.dll", GameDebugDir + "\\Newtonsoft.Json.dll");
            File.Copy(Environment.CurrentDirectory + "\\ImGui.NET.dll", GameDebugDir + "\\ImGui.NET.dll");
            File.Copy(Environment.CurrentDirectory + "\\runtimes\\win-x64\\native\\cimgui.dll", GameDebugDir + "\\cimgui.dll"); //x64 and x32?
            File.Copy(Environment.CurrentDirectory + "\\libmojoshader_64.dll", GameDebugDir + "\\libmojoshader_64.dll");

            string NameSpaceName = Path.Substring(Path.LastIndexOf('\\') + 1);
            File.WriteAllText(Path + @"\" + NameSpaceName + ".csproj", GetTemplateProjFile());
            File.WriteAllText(Path + @"\" + "Game1.cs", GetTemplateMainFile(NameSpaceName));
            GameName = NameSpaceName;

            string SourceDir = Environment.CurrentDirectory;

            string[] GCP = SourceDir.Split('\\');
            SourceDir = SourceDir.Remove(SourceDir.Length - GCP[GCP.Length - 1].Length - 1, GCP[GCP.Length - 1].Length + 1);
            SourceDir = SourceDir.Remove(SourceDir.Length - GCP[GCP.Length - 2].Length - 1, GCP[GCP.Length - 2].Length + 1);
            SourceDir = SourceDir.Remove(SourceDir.Length - GCP[GCP.Length - 3].Length - 1, GCP[GCP.Length - 3].Length + 1);
            SourceDir = SourceDir + "\\Content";

            string DestinationDir = Path + "\\Content";

            Directory.CreateDirectory(DestinationDir + "\\Icons");
            Directory.CreateDirectory(DestinationDir + "\\Scenes");
            Directory.CreateDirectory(DestinationDir + "\\Default Textures");
            Directory.CreateDirectory(DestinationDir + "\\bin\\DesktopGL");
            //File.Copy(SourceDir + "\\LightShaderCompiled\\LightTest.xnb", DestinationDir + "\\bin\\DesktopGL\\LightTest.xnb");

            CopyAndPasteFiles(SourceDir + "\\Icons", DestinationDir + "\\Icons");
            CopyAndPasteFiles(SourceDir + "\\Default Textures", DestinationDir + "\\Default Textures");
            File.Copy(SourceDir + "\\LightTest.fx", DestinationDir + "\\LightTest.fx");
            File.Copy(SourceDir + "\\Font.spritefont", DestinationDir + "\\Font.spritefont");
            File.Copy(SourceDir + "\\Sea Horses.ttf", DestinationDir + "\\Sea Horses.ttf");
            File.Copy(SourceDir + "\\imgui.ini", DestinationDir + "\\imgui.ini");

            NewProject = true;

            //Utility.ExecuteCommand(new string[] { "cd " + Path + "\\Content", })

            Utility.ExecuteCommand(RunGame, Path);

            Directory.CreateDirectory(GameDebugDir + "\\Content");
            File.Copy(SourceDir + "\\LightShaderCompiled\\LightTest.xnb", GameDebugDir + "\\Content\\LightTest.xnb");

            //Reload Assembly
            FN_Editor.InspectorWindow.ReloadAssemblyOnChange();
        }

        private static void CopyAndPasteFiles(string Source, string Destination)
        {
            foreach (string F in Directory.GetFiles(Source))
                File.Copy(F, Destination + "\\" + F.Remove(0, Source.Length + 1));
        }

        public static void OpenExistingProject(string Path)
        {
            string SlnName = Path.Split('\\')[(Path.Split('\\').Length - 1)] + ".sln";
            string[] OpenProj = new string[]
            {
                //"dotnet new -i MonoGame.Templates.CSharp::3.8.0.1641", //Download Template
                "cd " + Path, //Go to the existing directory
                //"start " + SlnName
                //"mgcb-editor " + "Content\\Content.mgcb" //Open MGCB Editor (Optional)
            };

            GameName = Path.Split('\\')[(Path.Split('\\').Length - 1)];

            Utility.ExecuteCommand(OpenProj, Path);

            //Reload Assembly
            FN_Editor.InspectorWindow.ReloadAssemblyOnChange();
        }

        public static void RunGame()
        {
            string[] RunGM = new string[]
            {
                //"dotnet new -i MonoGame.Templates.CSharp::3.8.0.1641", //Download Template
                "cd " + GamePath, //Go to the existing directory
                "dotnet build",
                "dotnet run"
            };

            Utility.ExecuteCommand(RunGM, GamePath);
        }

        public static void RunExecutable()
        {
            string[] RunGM = new string[]
            {
                //"dotnet new -i MonoGame.Templates.CSharp::3.8.0.1641", //Download Template
                "cd " + GamePath + "\\bin\\Debug\\netcoreapp3.1", //Go to the existing directory
                "start " + GameName + ".exe"
            };

            Utility.ExecuteCommand(RunGM, GamePath);
        }

        private static string GetTemplateProjFile() //Edit this whenever you add a package to the engine, or change settings
        {
            return "<Project Sdk=\"Microsoft.NET.Sdk\">" + "\n" +
                   "<PropertyGroup>" + "\n" +
                   "<OutputType>WinExe</OutputType>" + "\n" +
                   "<TargetFramework>netcoreapp3.1</TargetFramework>" + "\n" +
                   "<PublishReadyToRun>false</PublishReadyToRun>" + "\n" +
                   "<TieredCompilation>false</TieredCompilation>" + "\n" +
                   "</PropertyGroup>" + "\n" +
                   "<PropertyGroup>" + "\n" +
                   "<ApplicationManifest>app.manifest</ApplicationManifest>" + "\n" +
                   "<ApplicationIcon>Icon.ico</ApplicationIcon>" + "\n" +
                   "</PropertyGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<None Remove=\"Icon.ico\" />" + "\n" +
                   "<None Remove=\"Icon.bmp\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<EmbeddedResource Include=\"Icon.ico\" />" + "\n" +
                   "<EmbeddedResource Include=\"Icon.bmp\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<MonoGameContentReference Include=\"Content\\Content.mgcb\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<TrimmerRootAssembly Include=\"Microsoft.Xna.Framework.Content.ContentTypeReader\" Visible=\"false\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<PackageReference Include=\"MonoGame.Framework.DesktopGL\" Version=\"3.8.0.1641\" />" + "\n" +
                   "<PackageReference Include=\"MonoGame.Content.Builder.Task\" Version=\"3.8.0.1641\" />" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "<ItemGroup>" + "\n" +
                   "<Reference Include = \"FN_Engine\" >" + "\n" +
                   "<HintPath>bin\\Debug\\netcoreapp3.1\\FN_Engine.dll</HintPath>" + "\n" +
                   "</Reference>" + "\n" +
                   "<Reference Include = \"Newtonsoft.Json\" >" + "\n" +
                   "<HintPath>bin\\Debug\\netcoreapp3.1\\Newtonsoft.Json.dll</HintPath>" + "\n" +
                   "</Reference>" + "\n" +
                   "<Reference Include = \"ImGui.NET\" >" + "\n" +
                   "<HintPath>bin\\Debug\\netcoreapp3.1\\ImGui.NET.dll</HintPath>" + "\n" +
                   "</Reference>" + "\n" +
                   "</ItemGroup>" + "\n" +
                   "</Project>";
        }

        private static string GetTemplateMainFile(string NameSpace)
        {
            return "using Microsoft.Xna.Framework;" + "\n" +
            "using Microsoft.Xna.Framework.Graphics;" + "\n" +
            "using Microsoft.Xna.Framework.Input;" + "\n" +
            "using System;" + "\n" +
            "using FN_Engine;" + "\n" +
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
            "        Camera2D Camera;" + "\n" +
            "        ////////<Variables>/////" + "\n" +
            "        public static SpriteFont spriteFont;" + "\n" +
            "        ////////////////////////" + "\n" +
            "\n" +
            "        public Game1()" + "\n" +
            "        {" + "\n" +
            "            graphics = new GraphicsDeviceManager(this);" + "\n" +
            "            Content.RootDirectory = \"Content\";" + "\n" +
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
            "            //graphics.GraphicsProfile = GraphicsProfile.HiDef; //Uncomment this if you want more graphical capabilities" + "\n" +
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
            "    \n" +
            "            /////////Camera And Resolution Independent Renderer/////// -> Mandatory" + "\n" +
            "            Camera = new Camera2D();" + "\n" +
            "            Camera.Zoom = 1f;" + "\n" +
            "            Setup.Initialize(graphics, Content, spriteBatch, Window, Camera, this);" + "\n" +
            "    \n" +
            "            ResolutionIndependentRenderer.Init(ref graphics);" + "\n" +
            "            ResolutionIndependentRenderer.SetResolution(1366, 768, false);" + "\n" +
            "            ResolutionIndependentRenderer.SetVirtualResolution(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);" + "\n" +
            "        }" + "\n" +
            "    \n" +
            "        /// <summary>" + "\n" +
            "        /// LoadContent will be called once per game and is the place to load" + "\n" +
            "        /// all of your content." + "\n" +
            "        /// </summary>" + "\n" +
            "        protected override void LoadContent()" + "\n" +
            "        {" + "\n" +
            "            ImportantIntialization();" + "\n" +
            "            spriteFont = Content.Load<SpriteFont>(\"Font\");" + "\n" +
            "            // This bit of code handles the directories from a PC to another" + "\n" +
            "            string WorkingDirectory = \"\";" + "\n" +
            "            foreach (string S in Environment.CurrentDirectory.Split('\\\\'))" + "\n" +
            "            {" + "\n" +
            "                if (S == \"bin\")" + "\n" +
            "                {" + "\n" +
            "                    WorkingDirectory = WorkingDirectory.Remove(WorkingDirectory.Length - 1, 1);" + "\n" +
            "                    break;" + "\n" +
            "                }" + "\n" +
            "    \n" +
            "                WorkingDirectory += S + '\\\\';" + "\n" +
            "            }" + "\n" +
            "    \n" +
            "            Environment.CurrentDirectory = WorkingDirectory + \"\\\\Content\";" + "\n" +
            "            Setup.SourceFilePath = Environment.CurrentDirectory;" + "\n" +
            "            Setup.IntermediateFilePath = Setup.SourceFilePath + \"\\\\obj\\\\DesktopGL\\\\\";" + "\n" +
            "            Setup.OutputFilePath = WorkingDirectory + \"\\\\bin\\\\Debug\\\\netcoreapp3.1\\\\Content\";" + "\n" +
            "            Setup.ConfigurePipelineMG();" + "\n" +
            "    \n" +
            "            SceneManager.LoadScene_Serialization(\"Scenes\\\\DefaultScene\");" + "\n" +
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