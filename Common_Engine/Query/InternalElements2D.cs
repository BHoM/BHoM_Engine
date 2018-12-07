using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<IElement2D> IInternalElements2D(this IElement2D element2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "InternalElements2D") as List<IElement2D>;
        }

        /******************************************/
    }
}
