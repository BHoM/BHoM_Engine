using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BHG.Polyline> ClosedShellGeometry(this List<BuildingElement> buildingElements)
        {
            List<BHG.Polyline> pLinesCurtainWall = new List<BHG.Polyline>();
            List<BHG.Polyline> pLinesOther = new List<BHG.Polyline>();

            //Merge curtain panels
            foreach (BuildingElement element in buildingElements)
            {
                BH.oM.Environment.Properties.BuildingElementProperties beProperty = element.BuildingElementProperties;
                BHG.Polyline pline = new BHG.Polyline() { ControlPoints = element.PanelCurve.IControlPoints() };

                if (beProperty != null && beProperty.CustomData.ContainsKey("Family Name") && (beProperty.CustomData["Family Name"].ToString() == "Curtain Wall"))
                    pLinesCurtainWall.Add(pline);
                else
                    pLinesOther.Add(pline);
            }

            //Add the rest of the geometries
            List<BHG.Polyline> mergedPolyLines = Compute.BooleanUnion(pLinesCurtainWall);
            mergedPolyLines.AddRange(pLinesOther);

            return mergedPolyLines;
        }
    }
}
