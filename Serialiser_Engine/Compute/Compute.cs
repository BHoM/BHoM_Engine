using BH.Engine.Serialiser.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static void AllowUpgradeFromBson(bool allow)
        {
            Config.AllowUpgradeFromBson = allow;
        }

        /*******************************************/
    }
}
