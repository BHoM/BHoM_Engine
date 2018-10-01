using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;

using BH.Engine.Geometry;

namespace BH.Engine.Structure
{

    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IGeometry> DeformedShape(List<Bar> bars, List<BarDeformation> barDeformations, string adapterId, object loadCase, double scaleFactor = 1.0, bool drawSections = false)
        {
            barDeformations = barDeformations.SelectCase(loadCase);

            List<IGeometry> geom = new List<IGeometry>();

            foreach (Bar bar in bars)
            {
                string id = bar.CustomData[adapterId].ToString();
                List<BarDeformation> deformations = barDeformations.Where(x => x.ObjectId == id).ToList();
                deformations.Sort();
                if (drawSections)
                    geom.AddRange(DeformedShapeSection(bar, deformations, scaleFactor));
                else
                    geom.Add(DeformedShapeCentreLine(bar, deformations, scaleFactor));

            }

            return geom;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline DeformedShapeCentreLine(Bar bar, List<BarDeformation> deformations, double scaleFactor = 1.0)
        {
            Vector tan = (bar.EndNode.Position - bar.StartNode.Position);
            Vector unitTan = tan.Normalise();
            Vector normal = bar.Normal();
            Vector yAxis = normal.CrossProduct(unitTan);


            List<Point> pts = new List<Point>();

            foreach (BarDeformation defo in deformations)
            {
                //Vector disp = new Vector { X = defo.UX * scaleFactor, Y = defo.UY * scaleFactor, Z = defo.UZ * scaleFactor };
                Vector disp = unitTan * defo.UX * scaleFactor + yAxis * defo.UY * scaleFactor + normal * defo.UZ * scaleFactor;
                Point pt = bar.StartNode.Position + tan * defo.Position + disp;
                pts.Add(pt);
            }

            return new Polyline { ControlPoints = pts };
        }


        /***************************************************/


        private static List<Loft> DeformedShapeSection(Bar bar, List<BarDeformation> deformations, double scaleFactor = 1.0)
        {
            Vector tan = (bar.EndNode.Position - bar.StartNode.Position);
            Vector unitTan = tan.Normalise();
            Vector normal = bar.Normal();
            Vector yAxis = normal.CrossProduct(unitTan);


            List<Point> pts = new List<Point>();

            IEnumerable<ICurve> sectionCurves = bar.Extrude(false).Select(x => (x as BH.oM.Geometry.Extrusion).Curve);

            List<Loft> lofts = new List<Loft>();
            foreach (ICurve sectionCurve in sectionCurves)
            {
                Loft loft = new Loft();
                foreach (BarDeformation defo in deformations)
                {
                    ICurve curve = sectionCurve.IRotate(bar.StartNode.Position, unitTan, defo.RX * scaleFactor);
                    Vector disp = unitTan * defo.UX * scaleFactor + yAxis * defo.UY * scaleFactor + normal * defo.UZ * scaleFactor;
                    disp += tan * defo.Position;
                    loft.Curves.Add(curve.ITranslate(disp));
                }
                lofts.Add(loft);
            }


            return lofts;
        }


        /***************************************************/
    }
}
