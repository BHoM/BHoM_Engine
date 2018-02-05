using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IObject Object(string libraryName, string objectName)
        {
            return Library(libraryName).Where(x => x.Name == objectName).FirstOrDefault();
        }

        /***************************************************/
    }
}
