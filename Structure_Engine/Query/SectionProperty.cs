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
        //public static void Calculate(this SectionProperty property)
        //{
        //    List<IntegrationSlice> verticalSlices = Geometry.Create.CreateSlices(property.Edges, Vector.XAxis);
        //    List<IntegrationSlice> horizontalSlices = Geometry.Create.CreateSlices(property.Edges, Vector.YAxis);



        //    Point min = property.Edges.GetBounds().Min;
        //    Point max = property.Edges.Bounds().Max;
        //    double centreY = 0;
        //    double centreX = 0;
        //    property.Area = Geometry.Query.IntegrateArea(horizontalSlices, 1, min.Y, max.Y, ref centreY);
        //    Geometry.Query.IntegrateArea(verticalSlices, 1, min.X, max.X, ref centreX);

        //    property.TotalWidth = max.X - min.X;
        //    property.TotalDepth = max.Y - min.Y;
        //    property.Iy = Geometry.Query.IntegrateArea(verticalSlices, 1, 2, 1, centreX);
        //    property.Iz = Geometry.Query.IntegrateArea(horizontalSlices, 1, 2, 1, centreY);
        //    property.Sy = 2 * Geometry.Query.IntegrateArea(verticalSlices, 1, 1, 1, min.X, centreX);
        //    property.Sz = 2 * Geometry.Query.IntegrateArea(verticalSlices, 1, 1, 1, min.Y, centreY);
        //    property.Rgy = System.Math.Sqrt(property.Iy / property.Area);
        //    property.Rgz = System.Math.Sqrt(property.Iz / property.Area);
        //    property.Vy = max.X - centreX;
        //    property.Vpy = centreX - min.X;
        //    property.Vz = max.Y - centreY;
        //    property.Vpz = centreY - min.Y;
        //    property.Zz = property.Iz / property.Vy;
        //    property.Zy = property.Iy / property.Vz;
        //    property.J = property.TorsionContant();
        //    property.Iw = property.WarpingConstant();
        //    property.Asy = ShearArea(verticalSlices, property.Iz, centreX);
        //    property.Asz = ShearArea(horizontalSlices, property.Iy, centreY);
        //}

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
            resutlts["Sy"] = 2 * Geometry.Query.GetAreaIntegration(horizontalSlices, 1, 1, 1, min.Y, centreZ);
            resutlts["Sz"] = 2 * Geometry.Query.GetAreaIntegration(verticalSlices, 1, 1, 1, min.X, centreY);
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


        public static double TorsionalConstant(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
        {
            switch (shape)
            {
                case ShapeType.ISection:
                case ShapeType.Channel:
                case ShapeType.Zed:
                    return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (totalDepth - tf1) * Math.Pow(tw, 3)) / 3;
                case ShapeType.Tee:
                case ShapeType.Angle:
                    return totalWidth * Math.Pow(tf1, 3) + totalDepth * Math.Pow(tw, 3);
                case ShapeType.Circle:
                    return Math.PI * Math.Pow(totalDepth, 4) / 2;
                case ShapeType.Box:
                    return 2 * tf1 * tw * Math.Pow(totalWidth - tw, 2) * Math.Pow(totalDepth - tf1, 2) /
                        (totalWidth * tw + totalDepth * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
                case ShapeType.Tube:
                    return Math.PI * (Math.Pow(totalDepth, 4) - Math.Pow(totalDepth - tw, 4)) / 2;
                default:
                    return 0;
            }
        }

        public static double WarpingConstant(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
        {

            switch (shape)
            {
                case ShapeType.ISection:
                    if (tf1 == tf2 && b1 == b2)
                    {
                        return tf1 * Math.Pow(totalDepth - tf1, 2) * Math.Pow(totalWidth, 3) / 24;
                    }
                    else
                    {
                        return tf1 * Math.Pow(totalDepth - (tf1 + tf2) / 2, 2) / 12 * (Math.Pow(b1, 3) * Math.Pow(b2, 3) / (Math.Pow(b1, 3) + Math.Pow(b2, 3)));
                    }
                case ShapeType.Channel:
                    return tf1 * Math.Pow(totalDepth, 2) / 12 * (3 * b1 * tf1 + 2 * totalDepth * tw / (6 * b1 * tf1 + totalDepth * tw));
                default:
                    return 0;

            }
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
