using BH.oM.Common;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static IElement2D INewInternalElement2D(this IElement2D element2D)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "NewInternalElement2D") as IElement2D;
        }

        /******************************************/
    }
}
