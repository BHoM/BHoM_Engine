using System.Collections.Generic;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Geometry;

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

        public static BuildingElement BuildingElement(ICurve crv, Opening opening)
        {
            return BuildingElement(new List<ICurve> { crv }, new List<Opening> { opening });
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<ICurve> curves, List<Opening> openings)
        {
            return new BuildingElement
            {
                PanelCurve = BH.Engine.Geometry.Create.PolyCurve(curves),
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

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve crv, Opening opening, BuildingElementProperties properties)
        {
            return BuildingElement(crv, new List<Opening> { opening }, properties);
        }

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve crv, List<Opening> openings, BuildingElementProperties properties)
        {
            return new BuildingElement
            {
                PanelCurve = crv,
                Openings = openings,
                BuildingElementProperties = properties,
            };
        }
    }
}
