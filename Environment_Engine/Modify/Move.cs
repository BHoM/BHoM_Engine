using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;

using BH.Engine.Geometry;


namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement Move(this BuildingElement buildingElement, Storey storey)
        {
            Vector aVector = Geometry.Create.Vector();
            if (buildingElement.Storey != null)
                aVector = Geometry.Create.Vector(0, 0, storey.Elevation - buildingElement.Storey.Elevation);

            BuildingElement aBuildingElement = buildingElement.GetShallowClone() as BuildingElement;
            aBuildingElement = aBuildingElement.Move(aVector);
            aBuildingElement.Storey = storey;

            return aBuildingElement;
        }

        public static BuildingElement Move(this BuildingElement buildingElement, Vector vector)
        {
            BuildingElement aBuildingElement = buildingElement.GetShallowClone() as BuildingElement;
            aBuildingElement.BuildingElementGeometry = Move(buildingElement.BuildingElementGeometry, vector);
            return aBuildingElement;
        }

        public static IBuildingElementGeometry Move(this IBuildingElementGeometry buildingElementGeometry, Vector vector)
        {
            return ISetGeometry(buildingElementGeometry, buildingElementGeometry.Curve.ITranslate(vector));
        }

        public static void TEst()
        {
            Building aBuilding;


            aBuilding.Spaces[0].BuildingElements[0] = aBuilding.Spaces[0].BuildingElements[0].Move(aBuilding.Storeys[2]);
        }
    }
}
