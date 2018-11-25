using BH.Engine.Geometry;
using BH.oM.Common;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static IElement0D ISetGeometry(this IElement0D element0D, Point point)
        {
            return Reflection.Compute.RunExtentionMethod(element0D, "SetGeometry", new object[] { point.Clone() }) as IElement0D;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static IElement1D ISetGeometry(this IElement1D element1D, ICurve curve)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "SetGeometry", new object[] { curve.IClone() }) as IElement1D;
        }

        /******************************************/
    }
}
