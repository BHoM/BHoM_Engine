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

        public static double TravelTime(this Ray ray)
        {
            return ray.Length() / Constants.SpeedOfSound;
        }

        /***************************************************/
    }
}
