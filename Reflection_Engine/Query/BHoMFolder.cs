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

        public static string BHoMFolder()
        {
            return @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\BHoM\";
        }


        /***************************************************/
    }
}
