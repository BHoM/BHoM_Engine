using System.Collections.Generic;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Structure.Elements;
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

        public static BuildingElement BuildingElement(BuildingElementPanel analyticalPanel)
        {
            return new BuildingElement
            {
                AnalyticalBuildingElementPanel = analyticalPanel,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementOpening opening)
        {
            List<BuildingElementOpening> openings = new List<oM.Environment.Elements.BuildingElementOpening>();
            openings.Add(opening);

            return BuildingElement(openings);
        }

        /***************************************************/

        public static BuildingElement BuildingElement(List<BuildingElementOpening> openings)
        {
            return new BuildingElement
            {
                Openings = openings,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, BuildingElementPanel buildingElementPanel)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                AnalyticalBuildingElementPanel = buildingElementPanel
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, IEnumerable<Polyline> polyLines)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                PanelCurve = Geometry.Create.PolyCurve(polyLines),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, ICurve curve)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                PanelCurve = curve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<ICurve> curves)
        {
            return new BuildingElement
            {
                PanelCurve = Geometry.Create.PolyCurve(curves),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(PolyCurve curve)
        {
            return new BuildingElement
            {
                PanelCurve = curve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<Polyline> polylines)
        {
            return new BuildingElement
            {
                PanelCurve = Geometry.Create.PolyCurve(polylines),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(Polyline polyline)
        {
            return new BuildingElement
            {
                PanelCurve = polyline,
            };
        }
    }
}
