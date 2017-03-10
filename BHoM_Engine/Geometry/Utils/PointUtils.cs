using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static class PointUtils
    {
        /// <summary>
        /// Removes duplicate points which have the same value when rounded to the number of input decimals
        /// </summary>
        /// <param name="points"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static List<Point> RemoveDuplicates(List<Point> points, int decimals)
        {
            Dictionary<string, Point> values = new Dictionary<string, Point>();
            Point tempPoint = null;
            for (int i = 0; i < points.Count; i++)
            {
                string key = points[i].ToString(decimals);
                if (!values.TryGetValue(key, out tempPoint))
                {
                    values.Add(key, points[i]);
                }
            }
            return values.Values.ToList();
        }
    }

}
