using BH.oM.Common;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static IElement1D INewElement1D(this IElement2D element2D, ICurve curve)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "NewElement1D", new object[] { curve }) as IElement1D;
        }

        /******************************************/
    }
}
