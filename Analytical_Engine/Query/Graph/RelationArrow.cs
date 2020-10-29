using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static CompositeGeometry IRelationArrow(this IRelation relation)
        {
            List<IGeometry> geometries = new List<IGeometry>();
            geometries.Add(RelationArrow(relation as dynamic));
            return BH.Engine.Geometry.Create.CompositeGeometry(geometries);
        }

        /***************************************************/
        public static CompositeGeometry RelationArrow(this SpatialRelation relation)
        {
            List<IGeometry> geometries = new List<IGeometry>();
            ICurve curve = relation.Curve;
            geometries.Add(curve);
            if (curve is NurbsCurve)
            {
                NurbsCurve nurbsCurve = curve as NurbsCurve;
                curve = Engine.Geometry.Create.Polyline(nurbsCurve.ControlPoints);
            }
            List<ICurve> nonNurbs = new List<ICurve>();
            foreach (ICurve sub in curve.ISubParts())
            {
                if (sub is NurbsCurve)
                {
                    NurbsCurve nurbsCurve = sub as NurbsCurve;
                    nonNurbs.Add(Engine.Geometry.Create.Polyline(nurbsCurve.ControlPoints));
                }
                else
                {
                    nonNurbs.Add(sub);
                }
            }
            foreach (ICurve c in nonNurbs)
                geometries.AddRange(ArrowHead(c.IPointAtLength(c.ILength() * 0.9), c.IEndPoint()));

            return BH.Engine.Geometry.Create.CompositeGeometry(geometries);
        }
        /***************************************************/
        public static CompositeGeometry RelationArrow(this ProcessRelation relation)
        {
            List<IGeometry> geometries = new List<IGeometry>();
            ICurve curve = relation.Curve;
            geometries.Add(curve);
            
            geometries.AddRange(ArrowHead(curve.IPointAtLength(curve.ILength() * 0.9), curve.IEndPoint()));

            return BH.Engine.Geometry.Create.CompositeGeometry(geometries);
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static CompositeGeometry Process(this IRelation relation)
        {
            // Do nothing
            return new CompositeGeometry();
        }

        /***************************************************/
        /**** Private Methods                          ****/
        /***************************************************/
        private static List<Line> ArrowHead(Point start, Point end)
        {
            Vector back = start - end;
            Vector perp = back.CrossProduct(Vector.ZAxis);
            if (perp.Length() == 0)
                perp = back.CrossProduct(Vector.YAxis);
            perp = perp.Normalise();
            double l = perp.Length();
            perp = perp * start.Distance(end) / 2;
            Point p1 = end + (back + perp);
            Point p2 = end + (back - perp);
            
            return new List<Line>() 
            { 
                BH.Engine.Geometry.Create.Line(end, p1),
                BH.Engine.Geometry.Create.Line(end, p2)
            };
        }
        /***************************************************/
    }
}
