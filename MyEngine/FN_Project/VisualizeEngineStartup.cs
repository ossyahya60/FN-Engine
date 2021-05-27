using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MyEngine.FN_Project
{
    public class VisualizeEngineStartup: GameObjectComponent
    {
        private static bool FirstTimeOnThisMachine = false;
        private string SelectedPath = "";
        private static bool FirstBootUp = true;

        public override void Start()
        {
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
                        //string[] files = Directory.GetFiles(fbd.SelectedPath);
                        //MessageBox.Show("Files found: " + files.Length.ToString(), "Message");

                        SelectedPath = fbd.SelectedPath;
                        OpenExistingProject(SelectedPath);
                    }

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
                "dotnet sln add" + ProjName, //Add the project to the solution
                "mgcb-editor --register", //Register the content in MGCB
                "mgcb-editor", //Open MGCB Editor (Optional)
            };

            ExecuteCommand(MakeDesktopGLProj, Path);
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
                foreach(string Comm in Commands)
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
    }
}
