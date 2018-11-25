using BH.oM.Common;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static IElement0D INewInstance(this IElement0D element0D)
        {
            return Reflection.Compute.RunExtentionMethod(element0D, "NewInstance") as IElement0D;
        }

        /******************************************/

        public static IElement0D INewInstance(this IElement0D element0D, Point point)
        {
            IElement0D newElement0D = Reflection.Compute.RunExtentionMethod(element0D, "NewInstance") as IElement0D;
            return newElement0D.ISetGeometry(point);
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static IElement1D INewInstance(this IElement1D element1D)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "NewInstance") as IElement1D;
        }

        /******************************************/

        public static IElement1D INewInstance(this IElement1D element1D, ICurve curve)
        {
            IElement1D newElement1D = Reflection.Compute.RunExtentionMethod(element1D, "NewInstance") as IElement1D;
            return newElement1D.ISetGeometry(curve);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static IElement2D INewInstance(this IElement2D element2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "NewInstance") as IElement2D;
        }

        /******************************************/
    }
}
