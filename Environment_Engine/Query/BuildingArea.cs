using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double BuildingArea(BHE.Building building)
        {
            List<double> area = new List<double>();
            foreach (BHE.Space s in building.Spaces)
                area.Add(Environment.Query.FloorArea(s));

            return area.Sum();
        }
    }
}
