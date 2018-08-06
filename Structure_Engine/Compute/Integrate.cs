using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, object> Integrate(List<ICurve> curves, double tolerance = Tolerance.Distance)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            BoundingBox box = new BoundingBox();

            for (int i = 0; i < curves.Count; i++)
            {
                box += curves[i].IBounds();
            }

            Point min = box.Min;
            Point max = box.Max;
            double totalWidth = max.X - min.X;
            double totalHeight = max.Y - min.Y;

            List<IntegrationSlice> verticalSlices = Geometry.Create.IntegrationSlices(curves, Vector.XAxis, totalWidth/1000, tolerance);
            List<IntegrationSlice> horizontalSlices = Geometry.Create.IntegrationSlices(curves, Vector.YAxis, totalHeight/1000, tolerance);

            results["VerticalSlices"] = verticalSlices;
            results["HorizontalSlices"] = horizontalSlices;


            double centreZ = 0;
            double centreY = 0;
            double area = Engine.Geometry.Query.AreaIntegration(horizontalSlices, 1, min.Y, max.Y, ref centreZ);
            Engine.Geometry.Query.AreaIntegration(verticalSlices, 1, min.X, max.X, ref centreY);

            results["Area"] = area;
            results["CentreZ"] = centreZ;
            results["CentreY"] = centreY;

            results["TotalWidth"] = totalWidth;
            results["TotalDepth"] = totalHeight;
            results["Iy"] = Geometry.Query.AreaIntegration(horizontalSlices, 1, 2, 1, centreZ);
            results["Iz"] = Geometry.Query.AreaIntegration(verticalSlices, 1, 2, 1, centreY);
            //resutlts["Sy"] = 2 * Engine.Geometry.Query.AreaIntegration(horizontalSlices, min.Y, centreZ, 1, 1, 1);
            results["Wply"] = 2*Math.Abs(Geometry.Query.AreaIntegration(horizontalSlices, 1, 1, 1, min.Y, centreZ, centreZ));// + Math.Abs(Query.AreaIntegration(horizontalSlices, 1, 1, 1, centreZ, max.Y, centreZ));
            //resutlts["Sz"] = 2 * Engine.Geometry.Query.AreaIntegration(verticalSlices, min.X, centreY, 1, 1, 1);
            results["Wplz"] = 2*Math.Abs(Geometry.Query.AreaIntegration(verticalSlices, 1, 1, 1, min.X, centreY, centreY));// + Math.Abs(Query.AreaIntegration(verticalSlices, 1, 1, 1, centreY, max.X, centreY));
            results["Rgy"] = Math.Sqrt((double)results["Iy"] / area);
            results["Rgz"] = Math.Sqrt((double)results["Iz"] / area);
            results["Vy"] = max.X - centreY;
            results["Vpy"] = centreY - min.X;
            results["Vz"] = max.Y - centreZ;
            results["Vpz"] = centreZ - min.Y;
            results["Welz"] = (double)results["Iz"] / (double)results["Vy"];
            results["Wely"] = (double)results["Iy"] / (double)results["Vz"];
            results["Asy"] = Query.ShearArea(verticalSlices, (double)results["Iz"], centreY);
            results["Asz"] = Query.ShearArea(horizontalSlices, (double)results["Iy"], centreZ);

            return results;
        }

        /***************************************************/
    }
}
