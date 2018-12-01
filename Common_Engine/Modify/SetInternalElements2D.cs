using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static IElement2D ISetInternalElements2D(this IElement2D element2D, List<IElement2D> newElements2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "SetInternalElements2D", new object[] { newElements2D }) as IElement2D;
        }

        /******************************************/
    }
}
