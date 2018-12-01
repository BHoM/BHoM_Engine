using BH.oM.Common;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static Point IGeometry(this IElement0D element0D)
        {
            return Reflection.Compute.RunExtentionMethod(element0D, "Geometry") as Point;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static ICurve IGeometry(this IElement1D element1D)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "Geometry") as ICurve;
        }

        /******************************************/
    }
}
