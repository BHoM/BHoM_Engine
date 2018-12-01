using BH.Engine.Geometry;
using BH.oM.Base;
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

        public static List<ICurve> ElementCurves(this IElement1D element1D, bool recursive = true)
        {
            return new List<ICurve> { element1D.IGeometry() };
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<ICurve> ElementCurves(this IElement2D element2D, bool recursive)
        {
            List<ICurve> result = new List<ICurve>();

            PolyCurve outline = element2D.IOutlineCurve();
            foreach (ICurve curve in outline.Curves)
            {
                if (recursive)
                    result.AddRange(curve.ISubParts());
                else
                    result.Add(curve);
            }

            foreach (IElement2D e in element2D.IInternalElements2D())
            {
                result.AddRange(e.ElementCurves(recursive));
            }

            return result;
        }


        /******************************************/
        /**** Public Methods - Interfaces      ****/
        /******************************************/

        public static List<ICurve> IElementCurves(this BHoMObject element, bool recursive = true)
        {
            return ElementCurves(element as dynamic, recursive);
        }


        /******************************************/

        public static List<ICurve> IElementCurves(this List<BHoMObject> elements, bool recursive = true)
        {
            List<ICurve> result = new List<ICurve>();
            foreach (BHoMObject element in elements)
            {
                result.AddRange(element.IElementCurves(recursive));
            }
            return result;
        }

        /******************************************/
    }
}
