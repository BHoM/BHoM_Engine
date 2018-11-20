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
            aBuildingElementPanel.PanelCurve = buildingElementPanel.PanelCurve.IClone();
            return aBuildingElementPanel;
        }

        /***************************************************/

        public static IBuildingObject Copy(this IBuildingObject buildingObject)
        {
            IBuildingObject aBuildingObject = Copy(buildingObject as dynamic);
            return aBuildingObject;
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
            aBuildingElement.CustomData = buildingElement.CustomData;
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

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/



        /***************************************************/
    }
}
