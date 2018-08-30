using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
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

        //[NotImplemented]
        public static List<Extrusion> Extrude(this Bar bar)
        {

            //throw new NotImplementedException();

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

            TransformMatrix totalTransform = Engine.Geometry.Create.TranslationMatrix(trans) * axisAlign * alignmentPerp;

            List<Extrusion> extrutions = new List<Extrusion>();
            for (int i = 0; i < secCurves.Count; i++)
            {
                ICurve curve = secCurves[i];
                curve = BH.Engine.Geometry.Modify.IRotate(curve, orgin, Vector.ZAxis, bar.OrientationAngle);
                curve = BH.Engine.Geometry.Modify.ITransform(curve, totalTransform);
                extrutions.Add(new Extrusion() { Curve = curve, Direction = tan });
            }

            return extrutions;
            //return new CompositeGeometry() { Elements = extrutions };
        }

        /***************************************************/
    }
}
