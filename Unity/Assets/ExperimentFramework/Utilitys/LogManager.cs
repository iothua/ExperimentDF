using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ExperimentFramework
{
    public class LogManager
    {
        private static List<string> logs = new List<string>();
        
        public static string FilePath
        {

            get
            {
                string path;
                
                if (Application.isEditor)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                }
                else
                {
                    path = Path.Combine(Application.dataPath, "/Logs");
                }

                return path;
            }
        }

        public static void Debug(string msg)
        {
            if (Application.isEditor)
            {
                UnityEngine.Debug.Log(msg);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.Now);
            builder.Append(msg);
            
            logs.Add(builder.ToString());
        }

        public static void Shutdown()
        {
            StringBuilder builder=new StringBuilder();

            builder.Append(Application.dataPath);
            builder.Append("/Logs");
            builder.Append("/Unity");
            builder.Append(DateTime.Now.ToString("yyyy年MM月dd_HH时mm分ss秒"));
            builder.Append(".txt");

            string fullPath = builder.ToString();

            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Dispose();
            }

            File.WriteAllLines(fullPath, logs.ToArray());

            //UnityEngine.Debug.LogError("保存成功");
        }
    }
}

