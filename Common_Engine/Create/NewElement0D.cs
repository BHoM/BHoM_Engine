using BH.oM.Common;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static IElement0D INewElement0D(this IElement1D element1D, Point point)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "NewElement0D", new object[] { point }) as IElement0D;
        }

        /******************************************/
    }
}
