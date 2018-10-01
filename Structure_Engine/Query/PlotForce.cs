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

        public static List<ICurve> PlotBarForce(List<Bar> bars, List<BarForce> forces, string adapterId, double scaleFactor = 1.0, object loadCase = null, bool fx = true, bool fy = true, bool fz = true, bool mx = true, bool my = true, bool mz = true)
        {
            forces = forces.SelectCase(loadCase);

            List<ICurve> plots = new List<ICurve>();

            foreach (Bar bar in bars)
            {
                string barId = bar.CustomData[adapterId].ToString();
                List<BarForce> elementForces = forces.Where(x => x.ObjectId == barId).ToList();
                elementForces.Sort();
                plots.AddRange(PlotBarForce(bar, elementForces, scaleFactor, fx,fy,fz,mx,my,mz));
            }

            return plots;
        }

        /***************************************************/

        private static List<ICurve> PlotBarForce(Bar bar, List<BarForce> forces, double scaleFactor = 1.0, bool fx = true, bool fy = true, bool fz = true, bool mx = true, bool my = true, bool mz = true)
        {
            Vector tan = (bar.EndNode.Position - bar.StartNode.Position);
            Vector unitTan = tan.Normalise();
            Vector normal = bar.Normal();
            Vector yAxis = normal.CrossProduct(unitTan);

            scaleFactor /= 1000;

            List<Point> basePoints = forces.Select(x => bar.StartNode.Position + tan * x.Position).ToList();

            List<ICurve> plots = new List<ICurve>();

            if (fx) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.FX * scaleFactor).ToList()));
            if (fy) plots.AddRange(PlotSpecificForce(yAxis, basePoints, forces.Select(x => x.FY * scaleFactor).ToList()));
            if (fz) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.FZ * scaleFactor).ToList()));
            if (mx) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.MX * scaleFactor).ToList()));
            if (my) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.MY * scaleFactor).ToList()));
            if (mz) plots.AddRange(PlotSpecificForce(yAxis, basePoints, forces.Select(x => x.MZ * scaleFactor).ToList()));

            return plots;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> PlotSpecificForce(Vector v, List<Point> pts, List<double> values)
        {
            List<Point> otherPTs = new List<Point>();

            for (int i = 0; i < pts.Count; i++)
            {
                otherPTs.Add(pts[i] + v * values[i]);
            }

            List<ICurve> curves = new List<ICurve>();

            for (int i = 0; i < pts.Count; i++)
            {
                curves.Add(Engine.Geometry.Create.Line(pts[i], otherPTs[i]));
            }

            curves.Add(Engine.Geometry.Create.Polyline(otherPTs));
            return curves;
        }

        /***************************************************/
    }
}
