using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Properties;
using BH.oM.Environmental.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        public static BuildingElementProperties BuildingElementProperties(BuildingElementType BuildingElementType, string Name)
        {
            return new BuildingElementProperties
            {
                Name = Name,
                BuildingElementType = BuildingElementType
            };
        }

    }
}
