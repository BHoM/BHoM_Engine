using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Measure
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Plane GetPlane(this Storey storey)
        {
            return new Plane(new Point(0, 0, storey.Elevation), new Vector(0, 0, 1));
        }

        /***************************************************/


    }
}
