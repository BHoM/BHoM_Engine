using BH.oM.Acoustic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double SabineTime(this Room room, double roomAbsorbtion)
        {
            return Constants.SabineTimeCoefficient * room.Volume  / roomAbsorbtion;
        }
    }
}
