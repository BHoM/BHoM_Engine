using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Panel Panel(ICurve panelCurve, IEnumerable<Opening> openings)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
                Openings = openings.ToList(),
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve, PanelProperties properties)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
                PanelProperties = properties,
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve, PanelProperties properties, Opening opening)
        {
            return Panel(panelCurve, properties, new List<Opening> { opening });
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve, PanelProperties properties, IEnumerable<Opening> openings)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
                PanelProperties = properties,
                Openings = openings as List<Opening>,
            };
        }

        /***************************************************/

        public static Panel Panel(PanelProperties properties)
        {
            return new Panel
            {
                PanelProperties = properties,
            };
        }

        /***************************************************/

        public static Panel Panel(Opening opening)
        {
            return Panel(new List<Opening> { opening });
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<Opening> openings)
        {
            return new Panel
            {
                Openings = openings as List<Opening>,
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<ICurve> panelCurves)
        {
            return new Panel
            {
                PanelCurve = Geometry.Create.PolyCurve(panelCurves),
            };
        }

        /***************************************************/

        public static Panel Panel(PolyCurve panelCurve)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<Polyline> panelPolylines)
        {
            return new Panel
            {
                PanelCurve = Geometry.Create.PolyCurve(panelPolylines),
            };
        }

        /***************************************************/

        public static Panel Panel(Polyline panelPolyline)
        {
            return new Panel
            {
                PanelCurve = panelPolyline,
            };
        }

        /***************************************************/
    }
}
