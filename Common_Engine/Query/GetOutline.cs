using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<IElement1D> IGetOutline(this IElement2D element2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "GetOutline") as List<IElement1D>;
        }

        /******************************************/
    }
}
