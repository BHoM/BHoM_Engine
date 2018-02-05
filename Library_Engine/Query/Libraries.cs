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

        public static List<IObject> Library(string name)
        {
            List<IObject> objects;
            if (!m_parsedLibrary.TryGetValue(name, out objects))
            {
                string libraryString = LibraryString(name);

                if (libraryString == null)
                    return new List<IObject>();

                string[] entries = libraryString.Split('\n');

                objects = new List<IObject>();

                foreach (string entry in entries)
                {
                    if (!String.IsNullOrWhiteSpace(entry))
                    {
                        IObject obj = (IObject)BH.Engine.Serialiser.Convert.FromJson(entry);
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

        private static Dictionary<string, string> LibraryStrings()
        {
            if (m_libraryStrings.Count < 1)
                LoadAllLibraries();

            return m_libraryStrings;
        }

        /***************************************************/

        private static string LibraryString(string name)
        {
            string db;
            if (!LibraryStrings().TryGetValue(name, out db))
            {
                db = null;
            }
            return db;
        }
        /***************************************************/

        private static void LoadAllLibraries()
        {
            ResourceSet set = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in set)
            {
                m_libraryStrings[entry.Key.ToString()] = entry.Value.ToString();
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, string> m_libraryStrings = new Dictionary<string, string>();
        private static Dictionary<string, List<IObject>> m_parsedLibrary = new Dictionary<string, List<IObject>>();

        /***************************************************/
    }
}
