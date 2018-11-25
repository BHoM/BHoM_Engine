using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****          Public methods          ****/
        /******************************************/

        public static IElement2D ISetOutline(this IElement2D element2D, List<IElement1D> newOutline)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "SetOutline", new object[] { newOutline }) as IElement2D;
        }

        /******************************************/
    }
}
