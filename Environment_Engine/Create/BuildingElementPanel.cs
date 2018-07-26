using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElementPanel BuildingElementPanel(IEnumerable<ICurve> curves)
        {
            return new BuildingElementPanel
            {
                PolyCurve = Geometry.Create.PolyCurve(curves)
            };
        }

        /***************************************************/

        public static BuildingElementPanel BuildingElementPanel(PolyCurve polyCurve)
        {
            return new BuildingElementPanel
            {
                PolyCurve = polyCurve
            };
        }

        /***************************************************/

        public static BuildingElementPanel BuildingElementPanel(IEnumerable<Polyline> polylines)
        {
            return new BuildingElementPanel
            {
                PolyCurve = Geometry.Create.PolyCurve(polylines)
            };
        }

        public static BuildingElementPanel BuildingElementPanel(Polyline polyline)
        {
            return new BuildingElementPanel
            {
                PolyCurve = Geometry.Create.PolyCurve(new Polyline[] { polyline })
            };
        }

        /***************************************************/
    }
}
