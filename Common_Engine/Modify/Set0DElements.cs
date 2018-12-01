using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static IElement1D ISet0DElements(this IElement1D element1D, List<IElement0D> new0DElements)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "Set0DElements", new object[] { new0DElements }) as IElement1D;
        }

        /******************************************/
    }
}
