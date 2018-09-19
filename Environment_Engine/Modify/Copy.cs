using System.Collections.Generic;

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

        public static Panel Copy(this Panel buildingElementPanel)
        {
            Panel aBuildingElementPanel = buildingElementPanel.GetShallowClone(true) as Panel;
            aBuildingElementPanel.PanelCurve = buildingElementPanel.PanelCurve.IClone() as PolyCurve;
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
            aBuildingElement.BuildingElementProperties = buildingElement.BuildingElementProperties;
            aBuildingElement.Openings = new List<Opening>(buildingElement.Openings);
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
