using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Common.Materials;
using BH.oM.Structural.Properties;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static List<string> SubSectionNames(string libraryName)
        {
            return Library(libraryName).Select(x => x.IObjectSubSection()).Distinct().ToList();
        }

        /***************************************************/

        public static List<IBHoMObject> SubSection(string libraryName, string subSection)
        {
            return Library(libraryName).Where(x => x.IObjectSubSection() == subSection).ToList();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string IObjectSubSection(this IBHoMObject obj)
        {
            return ObjectSubSection(obj as dynamic);
        }

        /***************************************************/
        private static string ObjectSubSection(IBHoMObject obj)
        {
            return "Main";
        }

        /***************************************************/

        private static string ObjectSubSection(ISectionDimensions section)
        {
            return section.Shape.ToString();
        }

        /***************************************************/

        private static string ObjectSubSection(Material material)
        {
            return material.Type.ToString();
        }

        /***************************************************/
    }
}
