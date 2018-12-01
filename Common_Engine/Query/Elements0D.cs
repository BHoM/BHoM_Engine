using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static List<IElement0D> IElements0D(this IElement1D element1D)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "Elements0D") as List<IElement0D>;
        }

        /******************************************/
    }
}
