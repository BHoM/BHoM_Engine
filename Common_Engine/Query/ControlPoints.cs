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
            return Geometry.Query.IControlPoints(element1D.IGetGeometry());
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<Point> ControlPoints(this IElement2D element2D, bool externalOnly = false)
        {
            List<Point> pts = Geometry.Query.ControlPoints(element2D.IGetOutlineCurve());
            if (!externalOnly)
            {
                foreach (IElement2D e in element2D.IGetInternal2DElements())
                {
                    pts.AddRange(e.ControlPoints());
                }
            }
            return pts;
        }

        /******************************************/
    }
}
