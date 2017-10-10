using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Geometry
    {

        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static IBHoMGeometry _GetGeometry(this Bar bar) // TODO Duplicated code in Structural_Engine.Query.Geometry.Query
        {
            return new Line(bar.StartNode.Point, bar.EndNode.Point);
        }

        /***************************************************/

    }
}
