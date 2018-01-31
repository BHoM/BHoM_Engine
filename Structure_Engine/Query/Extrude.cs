using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry Extrude(this Bar bar)
        {

            if (bar.SectionProperty == null || !(bar.SectionProperty is IGeometricalSection))
                return bar.Geometry();

            List<ICurve> secCurves = ((CompositeGeometry)((IGeometricalSection)bar.SectionProperty).Geometry()).Elements.Select(x => (ICurve)x).ToList();

            Vector z = Vector.ZAxis;
            Point startPos = bar.StartNode.Position;
            Vector tan = bar.EndNode.Position- startPos;
            Vector rotAxis = BH.Engine.Geometry.Query.CrossProduct(z, tan);
            Vector trans = startPos - Point.Origin;
            double angle = BH.Engine.Geometry.Query.Angle(z, tan);


            List<IBHoMGeometry> extrutions = new List<IBHoMGeometry>();
            for (int i = 0; i < secCurves.Count; i++)
            {
                ICurve curve = secCurves[i];
                curve = BH.Engine.Geometry.Modify.ITranslate(curve, trans);
                curve = BH.Engine.Geometry.Modify.IRotate(curve, startPos, rotAxis, angle);
                extrutions.Add(new Extrusion() { Curve = curve, Direction = tan });

            }

            return new CompositeGeometry() { Elements = extrutions };
        }

        /***************************************************/
    }
}
