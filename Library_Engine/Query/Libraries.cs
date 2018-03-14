using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Resources;
using BH.oM.Base;
using BH.Engine.Serialiser;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IBHoMObject> Library(string name)
        {
            List<IBHoMObject> objects;
            if (!m_parsedLibrary.TryGetValue(name, out objects))
            {
                //string libraryString = LibraryString(name);

                string[] entries = LibraryString(name);

                if (entries == null)
                    return new List<IBHoMObject>();

                //string[] entries = libraryString.Split('\n');
                objects = new List<IBHoMObject>();

                //Parse the library from json to IBHoMObjects
                foreach (string entry in entries)
                {
                    if (!string.IsNullOrWhiteSpace(entry))
                    {
                        IBHoMObject obj = (IBHoMObject)BH.Engine.Serialiser.Convert.FromJson(entry);
                        if(obj != null)
                            objects.Add(obj);
                    }
                }

                m_parsedLibrary[name] = objects;
            }

            return objects;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<string, string[]> LibraryStrings()
        {
            //Check that libraries has been loaded
            if (m_libraryStrings.Count < 1)
                LoadAllLibraries();

            return m_libraryStrings;
        }

        /***************************************************/

        private static string[] LibraryString(string name)
        {
            string[] db;
            if (!LibraryStrings().TryGetValue(name, out db))
            {
                db = null;
            }
            return db;
        }
        /***************************************************/

        private static void LoadAllLibraries()
        {
            //Check for any files straight in the DataSets folder
            //foreach (string path in Directory.GetFiles(m_sourceFolder))
            //{
            //    m_libraryPaths[Path.GetFileNameWithoutExtension(path)] = new List<string>() { path };
            //}

            //foreach (string path in Directory.GetDirectories(sourceFolder))
            //{
            //    string dirName = Path.GetFileName(path);

            //}

            GetPaths();

            ////Read in all text files in the resource section to memmory as strings
            //ResourceSet set = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            //foreach (DictionaryEntry entry in set)
            //{
            //    m_libraryStrings[entry.Key.ToString()] = entry.Value.ToString();
            //}
        }


        private static void GetPaths(string folderPath = "", string basePath = "")
        {
            string internalPath = Path.Combine(basePath, folderPath);
            string folder = Path.Combine(m_sourceFolder, internalPath);

            foreach (string path in Directory.GetFiles(folder))
            {
                string filePathName = Path.Combine(internalPath, Path.GetFileNameWithoutExtension(path));
                AddToPathDictionary(filePathName, filePathName);
                m_libraryStrings[filePathName] = File.ReadAllLines(path);
            }

            foreach (string dictPath in Directory.GetDirectories(folder))
            {
                GetPaths(Path.GetFileName(dictPath), internalPath);
            }
        }

        /***************************************************/

        private static void AddToPathDictionary(string path, string dictionaryPath)
        {
            if (m_libraryPaths.ContainsKey(path))
                m_libraryPaths[path].Add(dictionaryPath);
            else
            {
                m_libraryPaths[path] = new List<string>();
                m_libraryPaths[path].Add(dictionaryPath);
            }

            string basePath = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(basePath))
                AddToPathDictionary(basePath, dictionaryPath);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static readonly string m_sourceFolder = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\BHoM\DataSets";

        //private static Dictionary<string, string> m_libraryStrings = new Dictionary<string, string>();
        private static Dictionary<string, string[]> m_libraryStrings = new Dictionary<string, string[]>();
        private static Dictionary<string, List<IBHoMObject>> m_parsedLibrary = new Dictionary<string, List<IBHoMObject>>();
        private static Dictionary<string, List<string>> m_libraryPaths = new Dictionary<string, List<string>>();

        /***************************************************/
    }
}
