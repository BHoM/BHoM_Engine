using BH.oM.Common;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****          Public methods          ****/
        /******************************************/

        public static IElement2D ISetOutlineElements1D(this IElement2D element2D, List<IElement1D> newOutline)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "SetOutlineElements1D", new object[] { newOutline }) as IElement2D;
        }

        /******************************************/
    }
}
