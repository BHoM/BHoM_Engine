using BH.oM.Reflection.Debugging;
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

        internal static Log DebugLog()
        {
            if (m_DebugLog == null)
                m_DebugLog = new Log();

            return m_DebugLog;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        [ThreadStatic]
        private static Log m_DebugLog = new Log();


        /***************************************************/
    }
}
