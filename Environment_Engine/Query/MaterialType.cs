using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Interface;
using BH.oM.Environment.Materials;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        /***************************************************/

        public static MaterialType IMaterialType(this IMaterial material)
        {
            return (material as Material).MaterialType;
        }

        /***************************************************/
    }
}
