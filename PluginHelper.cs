using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace search3D
{
    class PluginHelper
    {
        private static string pluginDir = @"\plugins";

        static PluginHelper() 
        {
            pluginDir = Path.GetDirectoryName(Application.ExecutablePath);
            pluginDir += @"\plugins";   
        }

        private static List<T> GetPlugins<T>(string folder)
        {
            if (Directory.Exists(folder) == false) return null;

            string[] files = Directory.GetFiles(folder, "*.dll");
            
            List<T> tList = new List<T>();
            System.Diagnostics.Debug.Assert(typeof(T).IsInterface);

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsNotPublic) continue;

                        Type[] interfaces = type.GetInterfaces();

                        if (interfaces.Contains(typeof(T)))
                        {
                            object obj = Activator.CreateInstance(type);
                            T t = (T)obj;
                            tList.Add(t);
                        }
                    }
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("plugin error: " + ex);
                }
            }

            return tList;
        }

        public static List<s3dCore.ListLayout.IListLayout> GetLayoutPlugins()
        {           
            List<s3dCore.ListLayout.IListLayout> list = GetPlugins<s3dCore.ListLayout.IListLayout>(pluginDir);

            return list;
        }

        public static List<s3dCore.SearchEngines.ISearchEngine> GetEnginePlugins()
        {
            List<s3dCore.SearchEngines.ISearchEngine> list = GetPlugins<s3dCore.SearchEngines.ISearchEngine>(pluginDir);

            return list;
        }
    }
}
