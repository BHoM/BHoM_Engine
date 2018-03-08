﻿using System;
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

        public static IBHoMObject Match(string libraryName, string objectName, bool removeWhiteSpace = false, bool toUpper = false)
        {
            objectName = removeWhiteSpace ? objectName.Replace(" ", "") : objectName;
            objectName = toUpper ? objectName.ToUpper() : objectName;

            Func<IBHoMObject, bool> conditionalMatch = delegate (IBHoMObject x)
            {
                string name = x.Name;
                name = removeWhiteSpace ? name.Replace(" ", "") : name;
                name = toUpper ? name.ToUpper() : name;
                return name == objectName;
            };

            return Library(libraryName).Where(conditionalMatch).FirstOrDefault();
        }

        /***************************************************/

        public static List<IBHoMObject> Match(string libraryName, string propertyName, string value)
        {
            return Library(libraryName).StringMatch(propertyName, value);
        }

        /***************************************************/

        //TODO: Move this extension method to somewhere else. Reflection_Engine/BHoM_Engine
        public static List<IBHoMObject> StringMatch(this List<IBHoMObject> objects, string propertyName, string value)
        {
            return objects.Where(x => x.PropertyValue(propertyName).ToString() == value).ToList();
        }

        /***************************************************/
    }
}
