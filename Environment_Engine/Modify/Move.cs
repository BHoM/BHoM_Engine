using BH.oM.Environment.Interface;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement Move(this BuildingElement buildingElement, Level level)
        {
            Vector aVector = Geometry.Create.Vector();
            if (buildingElement.Level != null)
                aVector = Geometry.Create.Vector(0, 0, level.Elevation - buildingElement.Level.Elevation);

            BuildingElement aBuildingElement = buildingElement.GetShallowClone() as BuildingElement;
            aBuildingElement = aBuildingElement.Move(aVector);
            aBuildingElement.Level = level;

            return aBuildingElement;
        }

        /***************************************************/

        public static BuildingElement Move(this BuildingElement buildingElement, Vector vector)
        {
            BuildingElement aBuildingElement = buildingElement.GetShallowClone() as BuildingElement;
            aBuildingElement.BuildingElementGeometry = Move(buildingElement.BuildingElementGeometry, vector);
            return aBuildingElement;
        }

        /***************************************************/

        public static IBuildingElementGeometry Move(this IBuildingElementGeometry buildingElementGeometry, Vector vector)
        {
           return buildingElementGeometry.ISetGeometry(buildingElementGeometry.ICurve().ITranslate(vector));
        }

        /***************************************************/
    }
}
