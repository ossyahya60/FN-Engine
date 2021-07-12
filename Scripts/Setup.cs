using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System.Windows.Forms;

namespace FN_Engine
{
    public class Setup
    {
        public static GraphicsDeviceManager graphics;
        public static ContentManager Content { get; private set; }
        public static GraphicsDevice GraphicsDevice { get; private set; }
        public static SpriteBatch spriteBatch { get; private set; }
        public static GameWindow GameWindow { get; private set; }
        public static Camera2D Camera { get; private set; }
        public static Game Game { get; private set; }
        public static PipelineManager PM { get; private set; }
        public static string OutputFilePath = @"C:\FN_Engine\FN_Engine\FN_Engine\bin\Windows\x86\Debug\Content";
        public static string SourceFilePath = @"C:\FN_Engine\FN_Engine\FN_Engine\Content";
        public static string IntermediateFilePath = @"C:\FN_Engine\FN_Engine\FN_Engine\Content\obj\Windows";
        //public static Form DragAndDropForm = new Form();

        public static void Initialize(GraphicsDeviceManager GDM, ContentManager CM, SpriteBatch SB, GameWindow GW, Camera2D camera, Game game)
        {
            ConfigurePipelineMG();
            graphics = GDM;
            Content = CM;
            GraphicsDevice = GDM.GraphicsDevice;
            spriteBatch = SB;
            GameWindow = GW;
            Camera = camera;
            Game = game;


            //DragAndDropForm.StartPosition = FormStartPosition.Manual;
            ////Recalculate these two upon any change to the game screen
            //DragAndDropForm.Location = new System.Drawing.Point(GameWindow.Position.X, GameWindow.Position.Y);
            //DragAndDropForm.Size = new System.Drawing.Size(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //DragAndDropForm.TransparencyKey = System.Drawing.Color.Red;
            //DragAndDropForm.BackColor = System.Drawing.Color.Red;
            //DragAndDropForm.Show();
            //DragAndDropForm.Visible = false;
        }

        public static void ConfigurePipelineMG()
        {
            PM = new PipelineManager(SourceFilePath, OutputFilePath, IntermediateFilePath);
        }
    }
}
