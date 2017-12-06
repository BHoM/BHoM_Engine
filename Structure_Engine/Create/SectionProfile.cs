using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> ISecctionCurves(double tft, double tfw, double bft, double bfw, double wt, double wd, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point(bfw / 2, 0, 0);

            perimeter.Add(new Line(p, p = p + Vector.YAxis * (bft - r2)));
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p - Vector.XAxis * r2, p, p = p + new Vector(-r2, r2, 0)));
            perimeter.Add(new Line(p, p = p - Vector.XAxis * (bfw / 2 - wt / 2 - r1 - r2)));
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.YAxis * r1, p, p = p + new Vector(-r1, r1, 0)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis * (wd - 2 * r1)));
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.XAxis * r1, p, p = p + new Vector(r1, r1, 0)));
            perimeter.Add(new Line(p, p = p + Vector.XAxis * (tfw / 2 - wt / 2 - r1 - r2)));
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.YAxis * r2, p, p = p + new Vector(r2, r2, 0)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis * (tft - r2)));

            int count = perimeter.Count;
            for (int i = 0; i < count;i++)       
            {
                perimeter.Add(perimeter[i].IGetMirrored(new Plane(Point.Origin, Vector.XAxis)));
            }
            perimeter.Add(new Line(p, p - Vector.XAxis * (tfw)));
            perimeter.Add(new Line(Point.Origin + Vector.XAxis * (-bfw / 2), Point.Origin + Vector.XAxis * (bfw / 2)));
            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> TeeSectionCurves(double tft, double tfw, double wt, double wd, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point(wt / 2, 0, 0);

            perimeter.Add(new Line(p, p = p + Vector.YAxis*(wd - r1)));
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.XAxis*(r1), p, p = p + new Vector(r1, r1, 0)));
            perimeter.Add(new Line(p, p = p + Vector.XAxis*(tfw / 2 - wt / 2 - r1 - r2)));
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.YAxis*(r2), p, p = p + new Vector(r2, r2, 0)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis*(tft - r2)));

            int count = perimeter.Count;
            for (int i = 0; i < count; i++)
            {
                perimeter.Add(perimeter[i].IGetMirrored(new Plane(Point.Origin, Vector.XAxis)));
            }

            perimeter.Add(new Line(p, p - Vector.XAxis*(tfw)));
            perimeter.Add(new Line(Point.Origin + Vector.XAxis*(-wt / 2), Point.Origin + Vector.XAxis*(wt / 2)));

            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> AngleSectionCurves(double width, double depth, double flangeThickness, double webThickness, double innerRadius, double toeRadius)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point(0, 0, 0);
            perimeter.Add(new Line(p, p = p + Vector.XAxis*(width)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis*(flangeThickness - toeRadius)));
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p - Vector.XAxis * (toeRadius), p, p = p + new Vector(-toeRadius, toeRadius, 0)));
            perimeter.Add(new Line(p, p = p - Vector.XAxis*(width - webThickness - innerRadius - toeRadius)));
            if (innerRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.YAxis * (innerRadius), p, p = p + new Vector(-innerRadius, innerRadius, 0)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis*(depth - flangeThickness - innerRadius - toeRadius)));
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.Arc(p - Vector.XAxis * (toeRadius), p, p = p + new Vector(-toeRadius, toeRadius, 0)));
            perimeter.Add(new Line(p, p = p - Vector.XAxis*(webThickness - toeRadius)));
            perimeter.Add(new Line(p, p = p - Vector.YAxis*(depth)));
            List<ICurve> translatedCurves = new List<ICurve>();

            foreach (ICurve crv in perimeter)
                translatedCurves.Add(crv.IGetTranslated(new Vector(-width / 2, -depth / 2, 0)));

            return translatedCurves;
        }

        /***************************************************/

        public static List<ICurve> RectangleSectionCurves(double width, double height, double radius)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point(-width / 2, height / 2 - radius, 0);
            perimeter.Add(new Line(p, p = p - Vector.YAxis * (height - 2*radius)));
            perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.XAxis * radius, p, p = p + new Vector(radius, -radius, 0)));
            perimeter.Add(new Line(p, p = p + Vector.XAxis * (width - 2 * radius)));
            perimeter.Add(BH.Engine.Geometry.Create.Arc(p + Vector.YAxis * radius, p, p = p + new Vector(radius, radius, 0)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis * (height - 2 * radius)));
            perimeter.Add(BH.Engine.Geometry.Create.Arc(p - Vector.XAxis * radius, p, p = p + new Vector(-radius, radius, 0)));
            perimeter.Add(new Line(p, p = p - Vector.XAxis * (width - 2 * radius)));
            perimeter.Add(BH.Engine.Geometry.Create.Arc(p - Vector.YAxis * radius, p, p = p + new Vector(-radius, -radius, 0)));
            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> BoxSectionCurves(double width, double height, double tw, double tf, double innerRadius, double outerRadius)
        {
            List<ICurve> box = RectangleSectionCurves(width, height, outerRadius);
            box.AddRange(RectangleSectionCurves(width - 2 * tw, height - 2 * tf, innerRadius));
            return box;
        }

        /***************************************************/

        public static List<ICurve> CircleSectionCurves(double radius)
        {
            return new List<ICurve> { new Circle(Point.Origin, radius) };
        }

        /***************************************************/

        public static List<ICurve> TubeSectionCurves(double outerRadius, double thickness)
        {
            List<ICurve> group = new List<ICurve>();
            group.AddRange(CircleSectionCurves(outerRadius));
            group.AddRange(CircleSectionCurves(outerRadius - thickness));
            return group;
        }

    }
}
