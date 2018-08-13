using System.Collections.Generic;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Structural.Properties;
using System.Linq;
using System;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> ISectionCurves(double tft, double tfw, double bft, double bfw, double wt, double wd, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = bfw / 2, Y = 0, Z = 0 };

            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis * (bft - r2) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - Vector.XAxis * r2, p, p = p + new Vector { X = -r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - Vector.XAxis * (bfw / 2 - wt / 2 - r1 - r2) });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.YAxis * r1, p, p = p + new Vector { X = -r1, Y = r1, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis * (wd - 2 * r1) });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.XAxis * r1, p, p = p + new Vector { X = r1, Y = r1, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.XAxis * (tfw / 2 - wt / 2 - r1 - r2) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.YAxis * r2, p, p = p + new Vector { X = r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis * (tft - r2) });

            int count = perimeter.Count;
            for (int i = 0; i < count;i++)       
            {
                perimeter.Add(perimeter[i].IMirror(new Plane { Origin = Point.Origin, Normal = Vector.XAxis }));
            }
            perimeter.Add(new Line { Start = p, End = p - Vector.XAxis * (tfw) });
            perimeter.Add(new Line { Start = Point.Origin + Vector.XAxis * (-bfw / 2), End = Point.Origin + Vector.XAxis * (bfw / 2) });
            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> TeeSectionCurves(double tft, double tfw, double wt, double wd, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = wt / 2, Y = 0, Z = 0 };

            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis*(wd - r1) });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.XAxis*(r1), p, p = p + new Vector { X = r1, Y = r1, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.XAxis*(tfw / 2 - wt / 2 - r1 - r2) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.YAxis*(r2), p, p = p + new Vector { X = r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis*(tft - r2) });

            int count = perimeter.Count;
            for (int i = 0; i < count; i++)
            {
                perimeter.Add(perimeter[i].IMirror(new Plane { Origin = Point.Origin, Normal = Vector.XAxis }));
            }

            perimeter.Add(new Line { Start = p, End = p - Vector.XAxis*(tfw) });
            perimeter.Add(new Line { Start = Point.Origin + Vector.XAxis*(-wt / 2), End = Point.Origin + Vector.XAxis*(wt / 2) });

            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> AngleSectionCurves(double width, double depth, double flangeThickness, double webThickness, double innerRadius, double toeRadius)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = 0, Y = 0, Z = 0 };
            perimeter.Add(new Line { Start = p, End = p = p + Vector.XAxis * (width) });
            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis*(flangeThickness - toeRadius) });
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - Vector.XAxis * (toeRadius), p, p = p + new Vector { X = -toeRadius, Y = toeRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - Vector.XAxis*(width - webThickness - innerRadius - toeRadius) });
            if (innerRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.YAxis * (innerRadius), p, p = p + new Vector { X = -innerRadius, Y = innerRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis*(depth - flangeThickness - innerRadius - toeRadius) });
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - Vector.XAxis * (toeRadius), p, p = p + new Vector { X = -toeRadius, Y = toeRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - Vector.XAxis*(webThickness - toeRadius) });
            perimeter.Add(new Line { Start = p, End = p = p - Vector.YAxis*(depth) });
            List<ICurve> translatedCurves = new List<ICurve>();

            foreach (ICurve crv in perimeter)
                translatedCurves.Add(crv.ITranslate(new Vector { X = -width / 2, Y = -depth / 2, Z = 0 }));

            return translatedCurves;
        }

        /***************************************************/

        public static List<ICurve> RectangleSectionCurves(double width, double height, double radius)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = -width / 2, Y = height / 2 - radius, Z = 0 };
            perimeter.Add(new Line { Start = p, End = p = p - Vector.YAxis * (height - 2*radius) });
            if(radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.XAxis * radius, p, p = p + new Vector { X = radius, Y = -radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.XAxis * (width - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + Vector.YAxis * radius, p, p = p + new Vector { X = radius, Y = radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + Vector.YAxis * (height - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - Vector.XAxis * radius, p, p = p + new Vector { X = -radius, Y = radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - Vector.XAxis * (width - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - Vector.YAxis * radius, p, p = p + new Vector { X = -radius, Y = -radius, Z = 0 }));
            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> BoxSectionCurves(double width, double height, double webThickness, double flangeThickness, double innerRadius, double outerRadius)
        {
            List<ICurve> box = RectangleSectionCurves(width, height, outerRadius);
            box.AddRange(RectangleSectionCurves(width - 2 * webThickness, height - 2 * flangeThickness, innerRadius));
            return box;
        }

        /***************************************************/

        public static List<ICurve> FabricatedBoxSectionCurves(double width, double height, double webThickness, double topFlangeThickness, double botFlangeThickness)
        {
            List<ICurve> box = RectangleSectionCurves(width, height, 0);
            List<ICurve> innerBox = RectangleSectionCurves(width - 2 * webThickness, height - (topFlangeThickness + botFlangeThickness), 0);
            double diff = botFlangeThickness- topFlangeThickness;

            if (diff != 0)
            {
                Vector v = new Vector() { X = 0, Y = diff / 2, Z = 0 };
                innerBox = innerBox.Select(x => Geometry.Modify.ITranslate(x, v)).ToList();
            }

            box.AddRange(innerBox);
            return box;
        }

        /***************************************************/

        public static List<ICurve> GeneralisedFabricatedBoxSectionCurves(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double topLeftCorbelWidth, double topRightCorbelWidth, double botLeftCorbelWidth, double botRightCorbelWidth)
        {
            List<ICurve> externalEdges = new List<ICurve>();
            List<ICurve> internalEdges = new List<ICurve>();
            List<ICurve> group = new List<ICurve>();
            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = new Point { X = 0, Y = botFlangeThickness, Z = 0 };

            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.XAxis * ((width / 2) + botRightCorbelWidth) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.YAxis * botFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - Vector.XAxis * botRightCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.YAxis * (height - botFlangeThickness - topFlangeThickness) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.XAxis * topRightCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.YAxis * topFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - Vector.XAxis * (width + topRightCorbelWidth + topLeftCorbelWidth) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - Vector.YAxis * topFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.XAxis * topLeftCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - Vector.YAxis * (height - botFlangeThickness - topFlangeThickness) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - Vector.XAxis * botLeftCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - Vector.YAxis * botFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + Vector.XAxis * ((width / 2) + botLeftCorbelWidth) });

            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + Vector.XAxis * ((width / 2) - webThickness) });
            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + Vector.YAxis * (height - botFlangeThickness - topFlangeThickness) });
            internalEdges.Add(new Line { Start = p2, End = p2 = p2 - Vector.XAxis * ((width / 2) - webThickness) });

            int intCount = internalEdges.Count;
            for (int i = 0; i < intCount; i++)
            {
                internalEdges.Add(internalEdges[i].IMirror(new Plane { Origin = Point.Origin, Normal = Vector.XAxis }));
            }

            group.AddRange(externalEdges);
            group.AddRange(internalEdges);

            return group;
        }

        /***************************************************/

        public static List<ICurve> KiteSectionCurves(double width1, double angle1, double thickness)
        {
            List<ICurve> externalEdges = new List<ICurve>();
            List<ICurve> internalEdges = new List<ICurve>();
            List<ICurve> group = new List<ICurve>();

            double width2 = width1 * Math.Tan(angle1 / 2);
            double angle2 = Math.PI - angle1;

            double tolerance = 1e-3;

            if (angle2 < tolerance || angle2 > Math.PI - tolerance)
            {
                throw new NotImplementedException("Angles must be well between 0 and Pi");
            }

            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = p1 + Vector.XAxis * Math.Abs(thickness / Math.Sin(angle1 / 2));

            Vector dirVec1 = Vector.XAxis.Rotate(angle1 / 2, Vector.ZAxis);
            Vector dirVec2 = dirVec1.Rotate(-(Math.PI / 2), Vector.ZAxis);

            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + dirVec1 * width1 });
            externalEdges.Add(new Line { Start = p1, End = p1 + dirVec2 * (width1 * Math.Tan(angle1 / 2)) });

            int extCount = externalEdges.Count;
            for (int i = 0; i < extCount; i++)
            {
                externalEdges.Add(externalEdges[i].IMirror(new Plane { Origin = Point.Origin, Normal = Vector.YAxis }));
            }

            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + dirVec1 * (width1 - thickness - (thickness * Math.Cos(angle1 / 2)) / Math.Sin(angle1 / 2)) });
            internalEdges.Add(new Line { Start = p2, End = p2 + dirVec2 * (width2 - thickness - (thickness * Math.Cos(angle2 / 2)) / Math.Sin(angle2 / 2)) });

            int intCount = internalEdges.Count;
            for (int i = 0; i < intCount; i++)
            {
                internalEdges.Add(internalEdges[i].IMirror(new Plane { Origin = Point.Origin, Normal = Vector.YAxis }));
            }

            group.AddRange(externalEdges);
            group.AddRange(internalEdges);

            return group;
        }

        /***************************************************/

        public static List<ICurve> CircleSectionCurves(double radius)
        {
            return new List<ICurve> { new Circle { Centre = Point.Origin, Radius = radius } };
        }

        /***************************************************/

        public static List<ICurve> TubeSectionCurves(double outerRadius, double thickness)
        {
            List<ICurve> group = new List<ICurve>();
            group.AddRange(CircleSectionCurves(outerRadius));
            group.AddRange(CircleSectionCurves(outerRadius - thickness));
            return group;
        }

        /***************************************************/

        public static List<ICurve> ChannelSectionCurves(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> edges = new List<ICurve>();

            Point p = new Point() { X = 0, Y = -height / 2 };
            edges.Add(new Line() { Start = new Point() { X = 0, Y = 0 }, End = new Point() { X = 0, Y = height / 2 } });
            edges.Add(new Line() { Start = new Point() { X = 0, Y = height / 2 }, End = new Point() { X = width, Y = height / 2 } });
            edges.Add(new Line() { Start = new Point() { X = width, Y = height / 2 }, End = new Point() { X = width, Y = height / 2 - flangeThickness + toeRadius } });
            if (toeRadius > 0) edges.Add(Geometry.Create.ArcByCentre(new Point() { X = width - toeRadius, Y = height / 2 - flangeThickness + toeRadius }, new Point() { X = width, Y = height / 2 - flangeThickness + toeRadius }, new Point() { X = width - toeRadius, Y = height / 2 - flangeThickness }));
            edges.Add(new Line() { Start = new Point() { X = width - toeRadius, Y = height / 2 - flangeThickness }, End = new Point() { X = webthickness + rootRadius, Y = height / 2 - flangeThickness } });
            if (rootRadius > 0) edges.Add(Geometry.Create.ArcByCentre(new Point() { X = webthickness + rootRadius, Y = height / 2 - flangeThickness - rootRadius }, new Point() { X = webthickness + rootRadius, Y = height / 2 - flangeThickness }, new Point() { X = webthickness, Y = height / 2 - flangeThickness - rootRadius }));
            edges.Add(new Line() { Start = new Point() { X = webthickness, Y = height / 2 - flangeThickness - rootRadius }, End = new Point() { X = webthickness, Y = 0 } });

            int count = edges.Count;
            for (int i = 0; i < count; i++)
            {
                edges.Add(edges[i].IMirror(new Plane { Origin = Point.Origin, Normal = Vector.YAxis }));
            }

            return edges;
        }

        /***************************************************/
    }
}
