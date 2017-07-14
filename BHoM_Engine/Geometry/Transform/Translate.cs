using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetTranslated(this IBHoMGeometry geometry, Vector transform)
        {
            return _GetTranslated(geometry as dynamic, transform);
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/
        public static Point _GetTranslated(Point pt, Vector transform)
        {
            return pt + transform;
        }

        /***************************************************/

        public static Vector _GetTranslated(Vector vector, Vector transform)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }

        /***************************************************/

        public static Plane _GetTranslated(Plane plane, Vector transform)
        {
            return new Plane(plane.Origin + transform, plane.Normal.GetClone() as Vector);
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        public static Arc _GetTranslated(Arc arc, Vector transform)
        {
            return new Arc(arc.Start + transform, arc.End + transform, arc.Middle + transform);
        }

        /***************************************************/

        public static Circle _GetTranslated(Circle circle, Vector transform)
        {
            return new Circle(circle.Centre + transform, circle.Normal.GetClone() as Vector, circle.Radius);
        }

        /***************************************************/

        public static Line _GetTranslated(Line line, Vector transform)
        {
            return new Line(line.Start + transform, line.End + transform);
        }

        /***************************************************/

        public static NurbCurve _GetTranslated(NurbCurve curve, Vector transform)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x + transform), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve _GetTranslated(PolyCurve curve, Vector transform)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetTranslated(transform) as ICurve));
        }

        /***************************************************/

        public static Polyline _GetTranslated(Polyline curve, Vector transform)
        {
            return new Polyline(curve.ControlPoints.Select(x => x + transform));
        }
    }
}
