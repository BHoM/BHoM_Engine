using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> GetControlPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this NurbCurve curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany(x => x.IGetControlPoints()).ToList();
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/
        /**** Public Methods - Surface                  ****/
        /***************************************************/

        public static int UVCount(this NurbSurface surf, string UorV)
        {
            if (UorV == "u" || UorV == "U")
            {
                return surf.UKnots.Count - (surf.GetDegree("u") - 1);
            }
           else if (UorV == "v" || UorV == "V")
            {
                return surf.VKnots.Count - (surf.GetDegree("v") - 1);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IGetControlPoints(this ICurve curve)
        {
            return GetControlPoints(curve as dynamic);
        }
    }
}
