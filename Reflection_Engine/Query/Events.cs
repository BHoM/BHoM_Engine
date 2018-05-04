using BH.oM.Reflection.Debuging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Event> AllEvents()
        {
            Log log = Query.DebugLog();
            return log.AllEvents;
        }


        /***************************************************/

        public static List<Event> CurrentEvents()
        {
            Log log = Query.DebugLog();
            return log.CurrentEvents;       
        }


        /***************************************************/
    }
}
