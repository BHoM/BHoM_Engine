using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Properties;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;

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

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, BuildingElementCurve buildingElementCurve, Storey storey)
        {
            return new BuildingElement
            {
                Storey = storey,
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = buildingElementCurve
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, BuildingElementPanel buildingElementPanel, Storey storey)
        {
            return new BuildingElement
            {
                Storey = storey,
                BuildingElementProperties = buildingElementProperties,
                BuildingElementGeometry = buildingElementPanel
            };
        }

        /***************************************************/
    }
}
