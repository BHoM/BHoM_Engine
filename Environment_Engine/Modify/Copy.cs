using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Interface;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        public static Storey Copy(this Storey Storey, string Name, double Elevation)
        {
            Storey aStorey = Storey.GetShallowClone(true) as Storey;
            aStorey.Name = Name;
            aStorey.Elevation = Elevation;

            return aStorey;
        }

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry BuildingElementGeometry, Vector Vector)
        {
            IBuildingElementGeometry aBuildingElementGeometry = BuildingElementGeometry.GetShallowClone(true) as IBuildingElementGeometry;
            aBuildingElementGeometry.Curve.ITranslate(Vector);
            return aBuildingElementGeometry;
        }

        public static BuildingElement Copy(this BuildingElement BuildingElement, Vector Vector)
        {
            BuildingElement aBuildingElement = BuildingElement.GetShallowClone(true) as BuildingElement;

            if(BuildingElement.BuildingElementGeometry != null)
                aBuildingElement.BuildingElementGeometry = BuildingElement.BuildingElementGeometry.Copy(Vector);

            return aBuildingElement;
        }
    }
}
