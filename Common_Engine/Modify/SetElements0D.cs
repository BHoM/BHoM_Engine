using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static IElement1D ISetElements0D(this IElement1D element1D, List<IElement0D> newElements0D)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "SetElements0D", new object[] { newElements0D }) as IElement1D;
        }

        /******************************************/
    }
}
