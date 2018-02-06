using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.Engine.Reflection;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IObject Match(string libraryName, string objectName)
        {
            return Library(libraryName).Where(x => x.Name == objectName).FirstOrDefault();
        }

        /***************************************************/

        public static List<IObject> Match(string libraryName, string propertyName, string value)
        {
            return Library(libraryName).StringMatch(propertyName, value);
        }

        /***************************************************/

        //TODO: Move this extension method to somewhere else. Reflection_Engine/BHoM_Engine
        public static List<IObject> StringMatch(this List<IObject> objects, string propertyName, string value)
        {
            return objects.Where(x => x.PropertyValue(propertyName).ToString() == value).ToList();
        }

        /***************************************************/
    }
}
