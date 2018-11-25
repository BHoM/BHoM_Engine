using BH.oM.Common;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static PolyCurve IGetOutlineCurve(this IElement2D element2D)
        {
            return new PolyCurve { Curves = element2D.IGetOutline().Select(e => Reflection.Compute.RunExtentionMethod(e, "GetGeometry") as ICurve).ToList() };
        }

        /******************************************/

        public static PolyCurve IGetOutlineCurve(this List<IElement1D> elements1D)
        {
            return new PolyCurve { Curves = elements1D.Select(e => Reflection.Compute.RunExtentionMethod(e, "GetGeometry") as ICurve).ToList() };
        }

        /******************************************/

        public static List<PolyCurve> IGetInternalOutlineCurves(this IElement2D element2D)
        {
            return element2D.IGetInternal2DElements().Select(x => x.IGetOutlineCurve()).ToList();
        }

        /******************************************/
    }
}
