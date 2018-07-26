using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Interface;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElementCurve Copy(this BuildingElementCurve buildingElementCurve)
        {
            BuildingElementCurve aBuildingElementCurve = buildingElementCurve.GetShallowClone(true) as BuildingElementCurve;
            aBuildingElementCurve.Curve = buildingElementCurve.Curve.IClone();
            return aBuildingElementCurve;
        }

        /***************************************************/

        public static BuildingElementPanel Copy(this BuildingElementPanel buildingElementPanel)
        {
            BuildingElementPanel aBuildingElementPanel = buildingElementPanel.GetShallowClone(true) as BuildingElementPanel;
            aBuildingElementPanel.PolyCurve = buildingElementPanel.PolyCurve.IClone() as PolyCurve;
            return aBuildingElementPanel;
        }

        /***************************************************/

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry buildingElementGeometry)
        {
            IBuildingElementGeometry aBuildingElementGeometry = Copy(buildingElementGeometry as dynamic);
            return aBuildingElementGeometry;
        }

        /***************************************************/

        public static Level Copy(this Level level)
        {
            return level.GetShallowClone(true) as Level;
        }

        /***************************************************/

        public static BuildingElement Copy(this BuildingElement buildingElement)
        {
            BuildingElement aBuildingElement = buildingElement.GetShallowClone(true) as BuildingElement;
            if (buildingElement.BuildingElementGeometry != null)
                aBuildingElement.BuildingElementGeometry = aBuildingElement.BuildingElementGeometry.Copy();
            return aBuildingElement;
        }

        /***************************************************/

        public static Level Copy(this Level level, string name, double elevation)
        {
            Level aLevel = level.Copy();
            aLevel.Name = name;
            aLevel.Elevation = elevation;

            return aLevel;
        }

        /***************************************************/

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry buildingElementGeometry, Vector vector)
        {
            IBuildingElementGeometry aBuildingElementGeometry = Copy(buildingElementGeometry);
            aBuildingElementGeometry.Move(vector);
            return aBuildingElementGeometry;
        }

        /***************************************************/

        public static BuildingElement Copy(this BuildingElement buildingElement, Vector vector)
        {
            BuildingElement aBuildingElement = buildingElement.Copy();
            aBuildingElement.Move(vector);
            return aBuildingElement;
        }

        /***************************************************/

        public static BuildingElement Copy(this BuildingElement buildingElement, Level level)
        {
            BuildingElement aBuildingElement = buildingElement.Copy();
            aBuildingElement.Move(level);
            return aBuildingElement;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/



        /***************************************************/
    }
}
