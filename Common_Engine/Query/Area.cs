using BH.Engine.Geometry;
using BH.oM.Common;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static double Area(this IElement2D element2D)
        {
            //TODO: make this work for PolyCurves (Booleans needed)

            double result = element2D.IOutlineCurve().Area();

            List<Polyline> openings = new List<Polyline>();
            foreach (PolyCurve o in element2D.IInternalOutlineCurves())
            {
                Polyline p = o.ToPolyline();
                if (p == null)
                    throw new NotImplementedException();

                openings.Add(p);
            }

            foreach (Polyline p in openings.BooleanUnion())
            {
                result -= p.Area();
            }

            return result;
        }

        /******************************************/
    }
}
