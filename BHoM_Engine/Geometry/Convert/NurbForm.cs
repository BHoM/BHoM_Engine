using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static NurbCurve ToNurbCurve(this ICurve geometry)
        {
            return _ToNurbCurve(geometry as dynamic);
        }


        /***************************************************/
        /**** Private  Methods - Curves                 ****/
        /***************************************************/

        private static NurbCurve _ToNurbCurve(this Arc arc)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static NurbCurve _ToNurbCurve(this Circle circle)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static NurbCurve _ToNurbCurve(this Line line)
        {
            return new NurbCurve(new List<Point> { line.Start, line.End }, new double[] { 1, 1 }, new double[] { 0, 0, 1, 1 });

        }

        /***************************************************/

        private static NurbCurve _ToNurbCurve(this NurbCurve curve)
        {
            return curve.Clone() as NurbCurve;
        }

        /***************************************************/

        private static NurbCurve _ToNurbCurve(this PolyCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static NurbCurve _ToNurbCurve(this Polyline curve)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private  Methods - Surfaces               ****/
        /***************************************************/
    }
}
