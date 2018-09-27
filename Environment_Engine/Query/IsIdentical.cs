using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsIdentical(this BuildingElement element, BuildingElement elementToCompare)
        {
            //Go through building elements and compare vertices and centre points
            if (element == null || elementToCompare == null) return false;

            List<Point> controlPoints = element.PanelCurve.IControlPoints();
            List<Point> measurePoints = elementToCompare.PanelCurve.IControlPoints();

            if (controlPoints.Count != measurePoints.Count) return false;

            bool allPointsMatch = true;
            foreach(Point p in controlPoints)
            {
                allPointsMatch &= measurePoints.IsContaining(p);
            }

            return allPointsMatch;  
        }
    }
}
