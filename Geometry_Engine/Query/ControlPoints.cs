using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> ControlPoints(this Arc curve)
        {
            //TODO: Should this give back the control points of an arc in nurbs form?
            return new List<Point>() { curve.StartPoint(), curve.PointAtParameter(0.25), curve.PointAtParameter(0.5), curve.PointAtParameter(0.75), curve.EndPoint() };
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Circle curve)
        {
            //TODO: Should this give back the control points of a circle in nurbs form?
            return new List<Point>() { curve.StartPoint(), curve.PointAtParameter(0.25), curve.PointAtParameter(0.5), curve.PointAtParameter(0.75), curve.EndPoint() };
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Ellipse curve)
        {
            //TODO: Should this give back the control points of a circle in nurbs form?
            return new List<Point>()
            {
                curve.Centre + curve.Radius1*curve.Axis1,
                curve.Centre + curve.Radius2*curve.Axis2,
                curve.Centre - curve.Radius1*curve.Axis1,
                curve.Centre - curve.Radius2*curve.Axis2,
                curve.Centre + curve.Radius1*curve.Axis1,
            };
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        [NotImplemented]
        public static List<Point> ControlPoints(this NurbsCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> ControlPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany((x, i) => x.IControlPoints().Skip((i > 0) ? 1 : 0)).ToList();
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IControlPoints(this ICurve curve)
        {
            return ControlPoints(curve as dynamic);
        }

        /***************************************************/
    }
}
