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

        public static PolyCurve IOutlineCurve(this IElement2D element2D)
        {
            return new PolyCurve { Curves = element2D.IOutlineElements1D().Select(e => Reflection.Compute.RunExtentionMethod(e, "Geometry") as ICurve).ToList() };
        }

        /******************************************/

        public static PolyCurve IOutlineCurve(this List<IElement1D> elements1D)
        {
            return new PolyCurve { Curves = elements1D.Select(e => Reflection.Compute.RunExtentionMethod(e, "Geometry") as ICurve).ToList() };
        }

        /******************************************/

        public static List<PolyCurve> IInternalOutlineCurves(this IElement2D element2D)
        {
            return element2D.IInternalElements2D().Select(x => x.IOutlineCurve()).ToList();
        }

        /******************************************/
    }
}
