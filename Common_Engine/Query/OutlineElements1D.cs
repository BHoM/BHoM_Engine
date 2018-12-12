using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<IElement1D> IOutlineElements1D(this IElement2D element2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "OutlineElements1D") as List<IElement1D>;
        }

        /******************************************/
    }
}
