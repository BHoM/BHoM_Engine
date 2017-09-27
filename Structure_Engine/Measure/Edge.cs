using BH.Engine.Geometry;
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

        public static List<ICurve> GetEdges(this Panel panel)
        {
            if (panel.Surface != null)
                return panel.Surface.GetExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/


    }

}
