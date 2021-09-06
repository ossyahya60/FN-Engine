using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace FN_Engine.FN_Project
{
    internal class MakeForm
    {
		public static string CreateOrOpenProj()
        {
            string GamePath = null;

            //You should Handle different platforms here
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        VisualizeEngineStartup.GamePath = fbd.SelectedPath;
                        if (Directory.GetFiles(fbd.SelectedPath).Length == 0 && Directory.GetDirectories(fbd.SelectedPath).Length == 0) // Make New Project
                        {
                            VisualizeEngineStartup.MakeNewProject(fbd.SelectedPath);
                            FN_Editor.ContentWindow.LogText.Add("Project created successfully!");
                        }
                        else
                        {
                            VisualizeEngineStartup.OpenExistingProject(fbd.SelectedPath);
                            FN_Editor.ContentWindow.LogText.Add("Project opened successfully!");
                        }

                        GamePath = fbd.SelectedPath;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        System.Environment.Exit(0);
                    }

                    return GamePath;
                }
            }

            return null;
        }
    }
}
