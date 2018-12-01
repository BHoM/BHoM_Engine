using BH.oM.Common;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static List<Point> ControlPoints(this IElement1D element1D)
        {
            return Geometry.Query.IControlPoints(element1D.IGeometry());
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<Point> ControlPoints(this IElement2D element2D, bool externalOnly = false)
        {
            List<Point> pts = Geometry.Query.ControlPoints(element2D.IOutlineCurve());
            if (!externalOnly)
            {
                foreach (IElement2D e in element2D.IInternalElements2D())
                {
                    pts.AddRange(e.ControlPoints());
                }
            }
            return pts;
        }

        /******************************************/
    }
}
