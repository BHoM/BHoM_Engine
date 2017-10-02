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
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> GetEdges(this Panel panel)
        {
            if (panel.Surface != null)
                return panel.Surface.IGetExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/


    }

}
