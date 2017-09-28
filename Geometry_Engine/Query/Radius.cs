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
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetRadius(this Arc arc)
        {
            Point centre = arc.GetCentre();
            if (centre != null)
                return centre.GetDistance(arc.Start);
            else
                return 0;
        }
    }
}
