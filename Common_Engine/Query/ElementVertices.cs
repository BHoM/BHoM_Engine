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

        public static List<Point> ElementVertices(this IElement1D element1D)
        {
            ICurve curve = element1D.IGetGeometry();
            List<Point> vertices = curve.IDiscontinuityPoints();

            if (curve.IIsClosed())
                vertices.RemoveAt(vertices.Count - 1);

            return vertices;
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<Point> ElementVertices(this IElement2D element2D)
        {
            List<Point> result = new List<Point>();
            result.AddRange(element2D.IGetOutlineCurve().ElementVertices());
            foreach (IElement2D e in element2D.IGetInternal2DElements())
            {
                result.AddRange(e.ElementVertices());
            }

            return result;
        }


        /******************************************/
        /**** Public Methods - Interfaces      ****/
        /******************************************/

        public static List<Point> IElementVertices(this BHoMObject element)
        {
            return ElementVertices(element as dynamic);
        }

        /******************************************/

        public static List<Point> IElementVertices(this List<BHoMObject> elements)
        {
            List<Point> result = new List<Point>();
            elements.ForEach(e => result.AddRange(e.IElementVertices()));
            return result;
        }

        /******************************************/
    }
}
