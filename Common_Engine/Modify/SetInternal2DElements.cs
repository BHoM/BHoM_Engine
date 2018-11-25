using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static IElement2D ISetInternal2DElements(this IElement2D element2D, List<IElement2D> new2DElements)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "SetInternal2DElements", new object[] { new2DElements }) as IElement2D;
        }

        /******************************************/
    }
}
