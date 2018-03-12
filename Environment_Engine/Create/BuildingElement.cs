using System.Collections.Generic;

using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Properties;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, BuildingElementPanel buildingElementPanel)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = buildingElementPanel
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementPanel buildingElementPanel)
        {
            return new BuildingElement
            {
                BuildingElementProperties = null,
                BuildingElementGeometry = buildingElementPanel
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, IEnumerable<Polyline> polyLines)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = Create.BuildingElementPanel(polyLines)
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, ICurve curve)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = Create.BuildingElementPanel(new ICurve[] { curve })
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, BuildingElementCurve buildingElementCurve, Level level)
        {
            return new BuildingElement
            {
                Level = level,
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = buildingElementCurve
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, BuildingElementPanel buildingElementPanel, Level level)
        {
            return new BuildingElement
            {
                Level = level,
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = buildingElementPanel
            };
        }

        /***************************************************/
    }
}
