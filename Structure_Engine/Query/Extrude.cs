using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Section.ShapeProfiles;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IGeometry> Extrude(this Bar bar, bool simple = false)
        {

            System.Reflection.PropertyInfo prop = bar.SectionProperty.GetType().GetProperty("SectionProfile");

            IProfile profile;

            if (prop != null)
                profile = prop.GetValue(bar.SectionProperty) as IProfile;
            else
                return null;// bar.Geometry();

            List<ICurve> secCurves = profile.Edges.ToList();

            Point orgin = new Point { X = bar.SectionProperty.CentreY, Y = bar.SectionProperty.CentreZ, Z = 0 };
            Vector z = Vector.ZAxis;
            Point startPos = bar.StartNode.Position;
            Vector tan = bar.EndNode.Position - startPos;
            Vector trans = startPos - orgin;

            double anglePerp = BH.Engine.Geometry.Query.Angle(z, tan);
            TransformMatrix alignmentPerp = Engine.Geometry.Create.RotationMatrix(orgin, BH.Engine.Geometry.Query.CrossProduct(z, tan), anglePerp);
            Vector localX = Vector.XAxis.Transform(alignmentPerp);

            double angleAxisAlign = localX.Angle(z.CrossProduct(tan));
            if (localX.DotProduct(Vector.ZAxis) > 0) angleAxisAlign = -angleAxisAlign;

            TransformMatrix axisAlign = Engine.Geometry.Create.RotationMatrix(orgin, tan, angleAxisAlign);

            TransformMatrix orientationAlign = Engine.Geometry.Create.RotationMatrix(orgin, Vector.ZAxis, bar.OrientationAngle);

            TransformMatrix totalTransform = Engine.Geometry.Create.TranslationMatrix(trans) * axisAlign * alignmentPerp * orientationAlign;

            if (simple)
                return ExtrudeSimple(secCurves, totalTransform, tan);
            else
                return ExtrudeFullCurves(secCurves, totalTransform, tan);

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IGeometry> ExtrudeFullCurves(List<ICurve> sectionCurves, TransformMatrix matrix, Vector tangent)
        {
            List<IGeometry> extrusions = new List<IGeometry>();

            List<PolyCurve> joined = sectionCurves.IJoin();

            for (int i = 0; i < joined.Count; i++)
            {
                ICurve curve = joined[i];
                curve = BH.Engine.Geometry.Modify.ITransform(curve, matrix);
                extrusions.Add(new Extrusion() { Curve = curve, Direction = tangent });
            }

            return extrusions;
        }

        /***************************************************/

        private static List<IGeometry> ExtrudeSimple(List<ICurve> sectionCurves, TransformMatrix matrix, Vector tangent)
        {
            BoundingBox box = sectionCurves.First().IBounds();

            for (int i = 1; i < sectionCurves.Count; i++)
            {
                box += sectionCurves[i].IBounds();
            }

            List<Point> pts = new List<Point>();

            pts.Add(new Point { X = box.Min.X, Y = box.Min.Y });
            pts.Add(new Point { X = box.Min.X, Y = box.Max.Y });
            pts.Add(new Point { X = box.Max.X, Y = box.Max.Y });
            pts.Add(new Point { X = box.Max.X, Y = box.Min.Y });

            for (int i = 0; i < pts.Count; i++)
            {
                pts[i] = pts[i].Transform(matrix);
            }

            for (int i = 0; i < 4; i++)
            {
                pts.Add(pts[i] + tangent);
            }

            Mesh mesh = new Mesh() { Vertices = pts };

            mesh.Faces.Add(new Face { A = 0, B = 1, C = 2, D = 3 });
            mesh.Faces.Add(new Face { A = 0, B = 1, C = 5, D = 4 });
            mesh.Faces.Add(new Face { A = 1, B = 2, C = 6, D = 5 });
            mesh.Faces.Add(new Face { A = 2, B = 3, C = 7, D = 6 });
            mesh.Faces.Add(new Face { A = 3, B = 0, C = 4, D = 7 });
            mesh.Faces.Add(new Face { A = 4, B = 5, C = 6, D = 7 });

            return new List<IGeometry> { mesh };

        }

        /***************************************************/
    }
}
