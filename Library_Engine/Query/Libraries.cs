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
                string libraryString = LibraryString(name);

                if (libraryString == null)
                    return new List<IBHoMObject>();

                string[] entries = libraryString.Split('\n');
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

        private static Dictionary<string, string> LibraryStrings()
        {
            //Check that libraries has been loaded
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
            //Read in all text files in the resource section to memmory as strings
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
        private static Dictionary<string, List<IBHoMObject>> m_parsedLibrary = new Dictionary<string, List<IBHoMObject>>();

        /***************************************************/
    }
}
