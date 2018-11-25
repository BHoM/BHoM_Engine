using BH.oM.Base;
using BH.oM.Common;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static IBHoMObject IGetShallowClone(this IElement0D element0D, bool newGuid = false)
        {
            return Reflection.Compute.RunExtentionMethod(element0D, "GetShallowClone", new object[] { newGuid }) as IBHoMObject;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static IBHoMObject IGetShallowClone(this IElement1D element1D, bool newGuid = false)
        {
            return Reflection.Compute.RunExtentionMethod(element1D, "GetShallowClone", new object[] { newGuid }) as IBHoMObject;
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static IBHoMObject IGetShallowClone(this IElement2D element2D, bool newGuid = false)
        {
            return Reflection.Compute.RunExtentionMethod(element2D, "GetShallowClone", new object[] { newGuid }) as IBHoMObject;
        }

        /******************************************/
    }
}
