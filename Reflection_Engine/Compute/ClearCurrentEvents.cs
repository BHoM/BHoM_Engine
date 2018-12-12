using BH.oM.Base;
using BH.oM.Reflection.Debugging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool ClearCurrentEvents()
        {
            Log log = Query.DebugLog();
            log.CurrentEvents.Clear();
            return true;
        }

        /***************************************************/
    }
}
