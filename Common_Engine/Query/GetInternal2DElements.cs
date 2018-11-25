using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<IElement2D> IGetInternal2DElements(this IElement2D element2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "GetInternal2DElements") as List<IElement2D>;
        }

        /******************************************/
    }
}
