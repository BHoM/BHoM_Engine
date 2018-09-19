using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Properties;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElementProperties BuildingElementProperties(string name, BuildingElementType buildingElementType)
        {
            return new BuildingElementProperties
            {
                Name = name,
                BuildingElementType = buildingElementType
            };
        }

        /***************************************************/

        public static BuildingElementProperties BuildingElementProperties(string name)
        {
            return new BuildingElementProperties
            {
                Name = name,
                BuildingElementType = BuildingElementType.Undefined
            };
        }

        /***************************************************/

        public static BuildingElementProperties BuildingElementProperties(BuildingElementType type)
        {
            return new BuildingElementProperties
            {
                BuildingElementType = type,
            };
        }
    }
}
