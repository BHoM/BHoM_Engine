using BH.oM.Common;
using BH.oM.Geometry;
using System;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static Point Centroid(this IElement1D element1D)
        {
            //TODO: find a proper centre of weight of a curve (not an average of control points)
            throw new NotImplementedException();
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static Point Centroid(this IElement2D element2D)
        {
            //TODO: find a proper centre of weight of a panel (not an average of control points)
            throw new NotImplementedException();
        }

        /******************************************/
    }
}
