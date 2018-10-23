using System.Collections.Generic;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties properties)
        {
            return new BuildingElement
            {
                BuildingElementProperties = properties,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve panelCurve)
        {
            return new BuildingElement
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(Opening opening)
        {
            return BuildingElement(new List<Opening> { opening });
        }

        /***************************************************/

        public static BuildingElement BuildingElement(List<Opening> openings)
        {
            return new BuildingElement
            {
                Openings = openings,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, IEnumerable<Polyline> panelPolyLines)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                PanelCurve = Geometry.Create.PolyCurve(panelPolyLines),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, ICurve panelCurve)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<ICurve> panelCurves)
        {
            return new BuildingElement
            {
                PanelCurve = Geometry.Create.PolyCurve(panelCurves),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(PolyCurve panelCurve)
        {
            return new BuildingElement
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<Polyline> panelPolyLines)
        {
            return new BuildingElement
            {
                PanelCurve = Geometry.Create.PolyCurve(panelPolyLines),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(Polyline panelPolyLine)
        {
            return new BuildingElement
            {
                PanelCurve = panelPolyLine,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(string name, ICurve panelCurve, BuildingElementProperties properties)
        {
            return new BuildingElement
            {
                Name = name,
                PanelCurve = panelCurve,
                BuildingElementProperties = properties,
            };
        }
    }
}
