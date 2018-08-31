using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Width(BHG.Polyline pLine, double length)
        {
            //Not convinced this is the best way of doing this - but this is being ported over from the XML Toolkit where it appears to be working so far so I'm not going to fiddle with it at this stage...
            return (pLine.Area() / length);
        }
    }
}
