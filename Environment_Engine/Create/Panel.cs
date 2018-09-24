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

        public static Panel Panel(ICurve curve, IEnumerable<Opening> openings)
        {
            return new Panel
            {
                PanelCurve = curve,
                Openings = openings.ToList(),
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve curve, PanelProperties properties)
        {
            return new Panel
            {
                PanelCurve = curve,
                PanelProperties = properties,
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve curve, PanelProperties properties, Opening opening)
        {
            return Panel(curve, properties, new List<Opening> { opening });
        }

        /***************************************************/

        public static Panel Panel(ICurve curve, PanelProperties properties, IEnumerable<Opening> openings)
        {
            return new Panel
            {
                PanelCurve = curve,
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

        public static Panel Panel(IEnumerable<ICurve> curves)
        {
            return new Panel
            {
                PanelCurve = Geometry.Create.PolyCurve(curves),
            };
        }

        /***************************************************/

        public static Panel Panel(PolyCurve curve)
        {
            return new Panel
            {
                PanelCurve = curve,
            };
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<Polyline> polylines)
        {
            return new Panel
            {
                PanelCurve = Geometry.Create.PolyCurve(polylines),
            };
        }

        /***************************************************/

        public static Panel Panel(Polyline polyline)
        {
            return new Panel
            {
                PanelCurve = polyline,
            };
        }

        /***************************************************/
    }
}
