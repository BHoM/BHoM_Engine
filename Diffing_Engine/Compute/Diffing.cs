using BH.oM.Base;
using BH.oM.Data.Collections;
using Diffing_oM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffing_Engine
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Delta Diffing(List<IBHoMObject> ToPush, List<IBHoMObject> Read)
        {
            var groups_ToPush = ToPush.GroupBy(x => x.GetType());
            var groups_Read = Read.GroupBy(x => x.GetType());

            Delta delta = new Delta(null, null, null, "", "", 0, "");
            return null;
        }

        /***************************************************/
    }
}
