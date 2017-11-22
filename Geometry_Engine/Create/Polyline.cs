using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Polyline(IEnumerable<Line> lines)
        {
            List<Point> ControlPoints = new List<Point>();
            foreach (Line line in lines)
            {
                ControlPoints.Add(line.Start);
            }
            ControlPoints.Add(ControlPoints[0]);

            return new Polyline(ControlPoints);
        }
    }
}
