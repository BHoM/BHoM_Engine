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
        //***************************************************/
        //**** Public Methods                            ****/
        //***************************************************/

        public static double GetLength(this Bar bar)
        {
            return bar.StartNode.Point.GetDistance(bar.EndNode.Point);
        }
    }
}
