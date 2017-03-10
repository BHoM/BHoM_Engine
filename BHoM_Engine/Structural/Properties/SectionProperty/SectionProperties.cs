using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Structural.Properties;
using BHoM.Numerics;
using BHoM.Geometry;

namespace BHoM.Structural
{
    public static class XSectionProperty
    {
        public static void Calculate(this SectionProperty property)
        {
            List<Slice> verticalSlices = Integration.CreateSlices(property.Edges, Vector.XAxis());
            List<Slice> horizontalSlices = Integration.CreateSlices(property.Edges, Vector.YAxis());
            Point min = property.Edges.Bounds().Min;
            Point max = property.Edges.Bounds().Max;
            double centreY = 0;
            double centreX = 0;
            property.Area = Integration.IntegrateArea(horizontalSlices, 1, min.Y, max.Y, ref centreY);
            Integration.IntegrateArea(verticalSlices, 1, min.X, max.X, ref centreX);

            property.TotalWidth = max.X - min.X;
            property.TotalDepth = max.Y - min.Y;
            property.Iy = Integration.IntegrateArea(verticalSlices, 1, 2, 1, centreX);
            property.Iz = Integration.IntegrateArea(horizontalSlices, 1, 2, 1, centreY);
            property.Sy = 2 * Integration.IntegrateArea(verticalSlices, 1,1,1, min.X, centreX);
            property.Sz = 2 * Integration.IntegrateArea(verticalSlices, 1, 1, 1, min.Y, centreY);
            property.Rgy = System.Math.Sqrt(property.Iy / property.Area);
            property.Rgz = System.Math.Sqrt(property.Iz / property.Area);
            property.Vy = max.X - centreX;
            property.Vpy = centreX - min.X;
            property.Vz = max.Y - centreY;
            property.Vpz = centreY - min.Y;
            property.Zz = property.Iz / property.Vy;
            property.Zy = property.Iy / property.Vz;
            property.J = property.TorsionContant();
            property.Iw = property.WarpingConstant();
            property.Asy = ShearArea(verticalSlices, property.Iz, centreX);
            property.Asz = ShearArea(horizontalSlices, property.Iy, centreY);
        }

        private static double TorsionContant(this SectionProperty property)
        {
            if (property is SteelSection)
            {
                SteelSection ss = property as SteelSection;
                double b1 = ss.B1;
                double b2 = ss.B2;
                double tf1 = ss.Tf1;
                double tf2 = ss.Tf2;
                double tw = ss.Tw;
                switch (ss.Shape)
                {
                    case ShapeType.ISection:
                    case ShapeType.Channel:
                    case ShapeType.Zed:
                        return (b1 * System.Math.Pow(tf1, 3) + b2 * System.Math.Pow(tf2, 3) + (ss.TotalDepth - tf1) * System.Math.Pow(tw, 3)) / 3;
                    case ShapeType.Tee:
                    case ShapeType.Angle:
                        return ss.TotalWidth * System.Math.Pow(tf1, 3) + ss.TotalDepth * System.Math.Pow(tw, 3);
                    case ShapeType.Circle:
                        return System.Math.PI * System.Math.Pow(ss.TotalDepth, 4) / 2;
                    case ShapeType.Box:
                        return 2 * tf1 * tw * System.Math.Pow(ss.TotalWidth - tw, 2) * System.Math.Pow(ss.TotalDepth - tf1, 2) /
                            (ss.TotalWidth * tw + ss.TotalDepth * tf1 - System.Math.Pow(tw, 2) - System.Math.Pow(tf1, 2));
                    case ShapeType.Tube:
                        return System.Math.PI * (System.Math.Pow(ss.TotalDepth, 4) - System.Math.Pow(ss.TotalDepth - tw, 4)) / 2;
                    default:
                        return 0;
                }
            }
            return 0;
        }


        private static double WarpingConstant(this SectionProperty property)
        {
            if (property is SectionProperty)
            {
                SteelSection steelSection = property as SteelSection;
                double b1 = steelSection.B1; 
                double b2 = steelSection.B2;
                double tf1 = steelSection.Tf1;
                double tf2 = steelSection.Tf2;
                double tw = steelSection.Tw;

                switch (steelSection.Shape)
                {
                    case ShapeType.ISection:
                        if (tf1 == tf2 && b1 == b2)
                        {
                            return tf1 * System.Math.Pow(steelSection.TotalDepth - tf1, 2) * System.Math.Pow(steelSection.TotalWidth, 3) / 24;
                        }
                        else
                        {
                            return tf1 * System.Math.Pow(steelSection.TotalDepth - (tf1 + tf2) / 2, 2) / 12 * (System.Math.Pow(b1, 3) * System.Math.Pow(b2, 3) / (System.Math.Pow(b1, 3) + System.Math.Pow(b2, 3)));
                        }
                    case ShapeType.Channel:
                        return tf1 * System.Math.Pow(steelSection.TotalDepth, 2) / 12 * (3 * b1 * tf1 + 2 * steelSection.TotalDepth * tw / (6 * b1 * tf1 + steelSection.TotalDepth * tw));
                    default:
                        return 0;
                }
            }
            return 0;
        }


        /// <summary>
        /// Shear Area in the Y direction
        /// </summary>
        public static double ShearArea(List<Slice> slices, double momentOfInertia, double centroid)
        {
            double sy = 0;
            double b = 0;
            double sum = 0;

            foreach (Slice slice in slices)
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


        //public virtual double AreaAtDepth(double depth)
        //{
        //    Slice slice;
        //    double area = 0;

        //    if (depth > TotalDepth)
        //    {
        //        return GrossArea;
        //    }

        //    depth = Max(1) - depth;

        //    for (int i = 0; i < HorizontalSlices.Count; i++)
        //    {
        //        slice = m_HorizontalSlices[i];
        //        area += slice.Width * slice.Length;
        //        if (slice.Centre + slice.Width / 2 > depth)
        //        {
        //            area -= slice.Length * (slice.Centre + slice.Width / 2 - depth);
        //            break;
        //        }
        //    }

        //    return area;
        //}

        //public virtual double CentroidAtDepth(double depth)
        //{
        //    Slice slice;
        //    double centroidArea = 0;
        //    double area = 0;

        //    if (depth > TotalDepth)
        //    {
        //        return CentreZ;
        //    }

        //    depth = Max(1) - depth;

        //    for (int i = 0; i < HorizontalSlices.Count; i++)
        //    {
        //        slice = m_HorizontalSlices[i];
        //        if (slice.Centre + slice.Width / 2 < depth)
        //        {
        //            centroidArea += (slice.Width + slice.Length) * slice.Centre;
        //            area += slice.Width + slice.Length;
        //        }
        //        else
        //        {
        //            double remainingWidth = slice.Width / 2 - depth + slice.Centre;
        //            centroidArea += remainingWidth * slice.Length * (slice.Centre + (slice.Width - remainingWidth) / 2);
        //            area += remainingWidth * slice.Length;
        //            break;
        //        }
        //    }

        //    return centroidArea / area;
        //}

        //public virtual double AreaAtWidth(double width)
        //{
        //    Slice slice;
        //    double area = 0;
        //    if (width > TotalWidth)
        //    {
        //        return GrossArea;
        //    }

        //    width = Max(0) - width;

        //    for (int i = VerticalSlices.Count - 1; i > 0; i--)
        //    {
        //        slice = m_VerticalSlices[i];
        //        if (slice.Centre - slice.Width / 2 > width)
        //        {
        //            area += slice.Width + slice.Length;
        //        }
        //        else
        //        {
        //            area += (slice.Width / 2 - width + slice.Centre) * slice.Length;
        //            break;
        //        }
        //    }
        //    return area;
        //}

        //public virtual double CentroidAtWidth(double width)
        //{
        //    Slice slice;
        //    double centroidArea = 0;
        //    double area = 0;

        //    if (width > TotalWidth)
        //    {
        //        return CentreY;
        //    }

        //    width = Max(0) - width;

        //    for (int i = VerticalSlices.Count - 1; i > 0; i--)
        //    {
        //        slice = m_VerticalSlices[i];
        //        if (slice.Centre - slice.Width / 2 > width)
        //        {
        //            centroidArea += (slice.Width + slice.Length) * slice.Centre;
        //            area += slice.Width + slice.Length;
        //        }
        //        else
        //        {
        //            double remainingWidth = slice.Width / 2 - width + slice.Centre;
        //            centroidArea += remainingWidth * slice.Length * (slice.Centre + (slice.Width - remainingWidth) / 2);
        //            area += remainingWidth * slice.Length;
        //            break;
        //        }
        //    }
        //    return centroidArea / area;
        //}

        public static double WidthAt(this SectionProperty prop, double y)
        {
            Slice slice = Integration.GetSliceAt(prop.Edges, y, 1, Plane.XZ());
            return slice.Length;
        }


        public static double WidthAt(this SectionProperty prop, double y, ref double[] range)
        {
            Slice slice = Integration.GetSliceAt(prop.Edges, y, 1, Plane.XZ());
            range = slice.Placement;
            return slice.Length;
        }

        public static double DepthAt(this SectionProperty prop, double x)
        {
            Slice slice = Integration.GetSliceAt(prop.Edges, x, 1, Plane.YZ());
            return slice.Length;
        }


        public static double DepthAt(this SectionProperty prop, double x, ref double[] range)
        {
            Slice slice = Integration.GetSliceAt(prop.Edges, x, 1, Plane.YZ());
            range = slice.Placement;
            return slice.Length;
        }


        /*****************************************************/
        /*********** Static section constructors *******/
        /*****************************************************/

       
        public static string GenerateStandardName(this SteelSection property)
        {
            string name = null;
            switch (property.Shape)
            {
                case ShapeType.ISection:
                    name = "UB " + (property.TotalDepth * 1000).ToString() + "x" + (property.TotalWidth * 1000).ToString() + "x" + (property.Tw * 1000).ToString();
                    break;
                case ShapeType.Tee:
                    name = "TUB " + (property.TotalWidth * 1000).ToString() + "x" + (property.TotalDepth * 1000).ToString() + "x" + (property.Tw * 1000).ToString();
                    break;
                case ShapeType.Box:
                    name = "RHS " + (property.TotalDepth * 1000).ToString() + "x" + (property.TotalWidth * 1000).ToString() + "x" + (property.Tw * 1000).ToString();
                    if (property.Tw != property.Tf1)
                        name += "x" + (property.Tf1 * 1000).ToString();
                    break;
                case ShapeType.Angle:
                    name = "L " + (property.TotalDepth * 1000).ToString() + "x" + (property.TotalWidth * 1000).ToString() + "x" + (property.Tw * 1000).ToString();
                    if (property.Tw != property.Tf1)
                        name += "x" + (property.Tf1 * 1000).ToString();
                    break;
                case ShapeType.Circle:
                    name = "C " + (property.TotalWidth * 1000).ToString();
                    break;
                case ShapeType.Rectangle:
                    name = "R " + (property.TotalDepth * 1000).ToString() + "x" + (property.TotalWidth * 1000).ToString();
                    break;
                case ShapeType.Tube:
                    name = "CHS " + (property.TotalWidth * 1000).ToString() + "x" + (property.Tw * 1000).ToString();
                    break;
                default:
                    name = property.Shape.ToString();
                    break;
            }
            return name;
        }     
    }
}
