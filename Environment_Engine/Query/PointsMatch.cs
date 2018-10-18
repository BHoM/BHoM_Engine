using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;

using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool PointsMatch(this List<Point> ctrlPoints, List<Point> measurePts)
        {
            if (ctrlPoints.Count != measurePts.Count) return false;

            foreach (Point p in ctrlPoints)
            {
                Point ptInMeasure = measurePts.Where(x => x.X == p.X && x.Y == p.Y && x.Z == p.Z).FirstOrDefault();
                if (ptInMeasure == null) return false; //Point did not have a match
            }

            return true; //No points returned false before now
        }
    }
}