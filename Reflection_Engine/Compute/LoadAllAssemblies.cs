using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Compute 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void LoadAllAssemblies(string folder = "")
        {
            if (!m_AssemblyAlreadyLoaded)
            {
                m_AssemblyAlreadyLoaded = true;
                HashSet<string> loaded = new HashSet<string>(AppDomain.CurrentDomain.GetAssemblies().Select(x => x.FullName.Split(',').First()));

                if (folder.Length == 0)
                    folder = Query.BHoMFolder();
                foreach (string file in Directory.GetFiles(folder))
                {
                    string[] parts = file.Split(new char[] { '.', '\\' });
                    if (parts.Length >= 2)
                    {
                        string name = parts[parts.Length - 2];
                        if (loaded.Contains(name))
                            continue;
                    }

                    if (file.EndsWith("oM.dll") || file.EndsWith("_Engine.dll") || file.EndsWith("_Adapter.dll") || file.EndsWith("_Test.dll"))
                    {
                        try
                        {
                            Assembly.LoadFrom(file);
                        }
                        catch
                        {
                            RecordWarning("Failed to load assembly " + file);
                        }
                    }
                }
            }
            
        }


        /***************************************************/
        /**** Private Static Fields                     ****/
        /***************************************************/

        private static bool m_AssemblyAlreadyLoaded = false;


        /***************************************************/
    }
}
