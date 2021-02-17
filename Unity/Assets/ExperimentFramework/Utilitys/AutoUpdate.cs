using System;
using System.Diagnostics;
using UnityEngine;

namespace ExperimentFramework
{
    public class AutoUpdate
    {
#if UNITY_STANDALONE_WIN
        Process p;
#endif

        public void Cheak(string server, string xml, string version, string user, string password)
        {
#if UNITY_STANDALONE_WIN

            string exePath = Environment.CurrentDirectory + "\\AutoUpdate\\AutoUpdate.exe";
            if (!System.IO.File.Exists(exePath)) {
                LogManager.Debug("更新路径不存在...");
                return;
            }

            p = new Process();
            p.StartInfo.FileName = Environment.CurrentDirectory + "\\AutoUpdate\\AutoUpdate.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Arguments = $"server={server} xml={xml} version={version} user={user} password={password}";
            p.EnableRaisingEvents = true;
            p.Exited += Exited;
            p.Start();
#endif
        }
        void Exited(object sender, EventArgs e)
        {
#if UNITY_STANDALONE_WIN
            p.Exited -= Exited;
            if (p.StandardOutput.ReadLine() == "1")
            {
                Application.Quit();
            }
            p.Close();
#endif
        }
    }
}

