using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        public static BuildingElementPanel BuildingElementPanel(IEnumerable<ICurve> Curves)
        {
            return new BuildingElementPanel()
            {
                PolyCurve = Geometry.Create.PolyCurve(Curves)
            };
        }

        public static BuildingElementPanel BuildingElementPanel(IEnumerable<Polyline> Polylines)
        {
            return new BuildingElementPanel()
            {
                PolyCurve = Geometry.Create.PolyCurve(Polylines)
            };
        }
    }
}
