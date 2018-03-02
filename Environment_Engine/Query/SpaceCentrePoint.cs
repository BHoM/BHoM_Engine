using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environmental.Elements;
using BH.oM.Environmental.Interface;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment.Query
{
    public static partial class Query
    {

        public static BHG.Point SpaceCentrePoint(List<BHE.BuildingElementPanel> bHoMPanels) //This does only work for convex spaces. we need to change this method later
        {
            List<BHG.Point> spacePts = new List<BHG.Point>();
            foreach (BHE.BuildingElementPanel panel in bHoMPanels)
            {
                spacePts.AddRange(panel.PolyCurve.ControlPoints());
            }
            BHG.Point centrePt = BH.Engine.Geometry.Query.Bounds(spacePts).Centre();

            return centrePt;
        }
    }
}
