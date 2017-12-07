using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {


        public static Dictionary<string, object> IntegrateCurve(List<ICurve> curves)
        {

            Dictionary<string, object> resutlts = new Dictionary<string, object>();

            BoundingBox box = new BoundingBox();

            for (int i = 0; i < curves.Count; i++)
            {
                box += curves[i].IGetBounds();
            }

            Point min = box.Min;
            Point max = box.Max;
            double totalWidth = max.X - min.X;
            double totalHeight = max.Y - min.Y;

            List<IntegrationSlice> verticalSlices = Geometry.Create.CreateSlices(curves, Vector.XAxis, totalWidth/1000);
            List<IntegrationSlice> horizontalSlices = Geometry.Create.CreateSlices(curves, Vector.YAxis, totalHeight/1000);

            resutlts["VerticalSlices"] = verticalSlices;
            resutlts["HorizontalSlices"] = horizontalSlices;


            double centreZ = 0;
            double centreY = 0;
            double area = Geometry.Query.GetAreaIntegration(horizontalSlices, 1, min.Y, max.Y, ref centreZ);
            Geometry.Query.GetAreaIntegration(verticalSlices, 1, min.X, max.X, ref centreY);

            resutlts["Area"] = area;
            resutlts["CentreZ"] = centreZ;
            resutlts["CentreY"] = centreY;

            resutlts["TotalWidth"] = totalWidth;
            resutlts["TotalDepth"] = totalHeight;
            resutlts["Iy"] = Geometry.Query.GetAreaIntegration(horizontalSlices, 1, 2, 1, centreZ);
            resutlts["Iz"] = Geometry.Query.GetAreaIntegration(verticalSlices, 1, 2, 1, centreY);
            //resutlts["Sy"] = 2 * Geometry.Query.GetAreaIntegration(horizontalSlices, min.Y, centreZ, 1, 1, 1);
            resutlts["Sy"] = 2 * Math.Abs(Geometry.Query.GetAreaIntegration(horizontalSlices, 1, 1, 1, min.Y, centreZ));
            //resutlts["Sz"] = 2 * Geometry.Query.GetAreaIntegration(verticalSlices, min.X, centreY, 1, 1, 1);
            resutlts["Sz"] = 2 * Math.Abs(Geometry.Query.GetAreaIntegration(verticalSlices, 1, 1, 1, min.X, centreY));
            resutlts["Rgy"] = System.Math.Sqrt((double)resutlts["Iy"] / area);
            resutlts["Rgz"] = System.Math.Sqrt((double)resutlts["Iz"] / area);
            resutlts["Vy"] = max.X - centreY;
            resutlts["Vpy"] = centreY - min.X;
            resutlts["Vz"] = max.Y - centreZ;
            resutlts["Vpz"] = centreZ - min.Y;
            resutlts["Zz"] = (double)resutlts["Iz"] / (double)resutlts["Vy"];
            resutlts["Zy"] = (double)resutlts["Iy"] / (double)resutlts["Vz"];
            resutlts["Asy"] = ShearArea(verticalSlices, (double)resutlts["Iz"], centreY);
            resutlts["Asz"] = ShearArea(horizontalSlices, (double)resutlts["Iy"], centreZ);


            return resutlts;

        }


        /// <summary>
        /// Shear Area in the Y direction
        /// </summary>
        public static double ShearArea(List<IntegrationSlice> slices, double momentOfInertia, double centroid)
        {
            double sy = 0;
            double b = 0;
            double sum = 0;

            foreach (IntegrationSlice slice in slices)
            {
                sy += slice.Length * slice.Width * (centroid - slice.Centre);
                b = slice.Length;
                if (b > 0)
                {
                    sum += System.Math.Pow(sy, 2) / b * slice.Width;
                }

            }
            return System.Math.Pow(momentOfInertia, 2) / sum;
        }
    }
}
