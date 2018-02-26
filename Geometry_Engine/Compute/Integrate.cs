using System;
using System.Collections.Generic;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, object> Integrate(List<ICurve> curves)
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

            List<IntegrationSlice> verticalSlices = Create.IntegrationSlices(curves, Vector.XAxis, totalWidth/1000);
            List<IntegrationSlice> horizontalSlices = Create.IntegrationSlices(curves, Vector.YAxis, totalHeight/1000);

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
            results["Iy"] = Query.AreaIntegration(horizontalSlices, 1, 2, 1, centreZ);
            results["Iz"] = Query.AreaIntegration(verticalSlices, 1, 2, 1, centreY);
            //resutlts["Sy"] = 2 * Engine.Geometry.Query.AreaIntegration(horizontalSlices, min.Y, centreZ, 1, 1, 1);
            results["Sy"] = 2 * Math.Abs(Query.AreaIntegration(horizontalSlices, 1, 1, 1, min.Y, centreZ));
            //resutlts["Sz"] = 2 * Engine.Geometry.Query.AreaIntegration(verticalSlices, min.X, centreY, 1, 1, 1);
            results["Sz"] = 2 * Math.Abs(Query.AreaIntegration(verticalSlices, 1, 1, 1, min.X, centreY));
            results["Rgy"] = Math.Sqrt((double)results["Iy"] / area);
            results["Rgz"] = Math.Sqrt((double)results["Iz"] / area);
            results["Vy"] = max.X - centreY;
            results["Vpy"] = centreY - min.X;
            results["Vz"] = max.Y - centreZ;
            results["Vpz"] = centreZ - min.Y;
            results["Zz"] = (double)results["Iz"] / (double)results["Vy"];
            results["Zy"] = (double)results["Iy"] / (double)results["Vz"];
            results["Asy"] = Query.ShearArea(verticalSlices, (double)results["Iz"], centreY);
            results["Asz"] = Query.ShearArea(horizontalSlices, (double)results["Iy"], centreZ);

            return results;
        }

        /***************************************************/
    }
}
