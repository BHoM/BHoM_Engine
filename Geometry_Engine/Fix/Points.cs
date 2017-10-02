using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Fix
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> GetDistinctPoints(this List<Point> points, int decimals)
        {
            Dictionary<string, Point> dic = new Dictionary<string, Point>();
            foreach (Point pt in points)
            {
                string key = pt.X.ToString("0.##") + pt.Y.ToString("0.##") + pt.Z.ToString("0.##");
                dic[key] = pt;
            }
            return dic.Values.ToList();
        }
    }
}
