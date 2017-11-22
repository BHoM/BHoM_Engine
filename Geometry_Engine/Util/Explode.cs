using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.Collections;

namespace BH.Engine.Geometry
{
    public static partial class Util
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Line> Explode(this Polyline pline)
        {
            List<Line> segments = new List<Line>();
            for (int i = 0; i < pline.ControlPoints.Count - 1; i++)
            {
                segments.Add(new Line(pline.ControlPoints[i], pline.ControlPoints[i + 1]));
            }
            return segments;
        }
    }
}
