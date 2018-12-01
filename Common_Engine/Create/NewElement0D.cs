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
            IElement0D newElement0D = Reflection.Compute.RunExtentionMethod(element1D, "NewElement0D") as IElement0D;
            return newElement0D.ISetGeometry(point);
        }

        /******************************************/
    }
}
