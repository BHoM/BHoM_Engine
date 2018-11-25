using BH.oM.Common;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static Point IGetGeometry(this IElement0D element0D)
        {
            return Reflection.Compute.RunExtentionMethod(element0D, "GetGeometry") as Point;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static ICurve IGetGeometry(this IElement1D element1D)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "GetGeometry") as ICurve;
        }

        /******************************************/
    }
}
