using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        // TODO: Does not work because of the GetIntersection method issue.
        public static bool GetSelfIntersections(this Polyline contour)
        {
            // TODO: Better to return point list

            //Polyline ccontour = contour.RemoveZeroSegments(0.001);
            List<Line> crvs = contour.GetExploded().Cast<Line>().ToList();

            int lc = crvs.Count;
            for (int i = 0; i < crvs.Count; i++)
            {
                for (int j = 0; j < crvs.Count; j++)
                {
                    if (Math.Abs(i - j) % (lc - 1) > 1)
                    {
                        Point intpt = crvs[i].GetIntersection(crvs[j]);
                        if (crvs[i].GetClosestPoint(intpt).GetDistance(intpt) < 0.000001 && crvs[j].GetClosestPoint(intpt).GetDistance(intpt) < 0.000001)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
