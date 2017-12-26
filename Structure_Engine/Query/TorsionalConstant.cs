using System;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static double TorsionalConstantThinWalled(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
        //{
        //    switch (shape)
        //    {
        //        case ShapeType.ISection:
        //        case ShapeType.Channel:
        //        case ShapeType.Zed:
        //            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (totalDepth - tf1) * Math.Pow(tw, 3)) / 3;
        //        case ShapeType.Tee:
        //        case ShapeType.Angle:
        //            return totalWidth * Math.Pow(tf1, 3) + totalDepth * Math.Pow(tw, 3);
        //        case ShapeType.Circle:
        //            return Math.PI * Math.Pow(totalDepth, 4) / 2;
        //        case ShapeType.Box:
        //            return 2 * tf1 * tw * Math.Pow(totalWidth - tw, 2) * Math.Pow(totalDepth - tf1, 2) /
        //                (totalWidth * tw + totalDepth * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        //        case ShapeType.Tube:
        //            return Math.PI * (Math.Pow(totalDepth, 4) - Math.Pow(totalDepth - tw, 4)) / 2;
        //        case ShapeType.Rectangle:
        //            if (Math.Abs(totalDepth - totalWidth) < Tolerance.Distance)
        //                return 2.25 * Math.Pow(totalDepth, 4);
        //            else
        //            {
        //                double a = Math.Max(totalDepth, totalWidth);
        //                double b = Math.Min(totalDepth, totalWidth);
        //                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
        //            }
        //        default:
        //            return 0;
        //    }
        //}


        /***************************************************/
        
        public static double TorsionalConstant(this ISectionDimensions dimensions) 
        {
            return 0; //Return 0 for not specifically implemented ones
        }

        /***************************************************/

        public static double TorsionalConstant(this CircleDimensions dimensions)
        {
            return Math.PI * Math.Pow(dimensions.Diameter, 4) / 2;
        }

        /***************************************************/

        public static double TorsionalConstant(this TubeDimensions dimensions)
        {
            return Math.PI * (Math.Pow(dimensions.Diameter, 4) - Math.Pow(dimensions.Diameter - dimensions.Thickness, 4)) / 2;
        }

        /***************************************************/

        public static double TorsionalConstant(this FabricatedBoxDimensions dimensions)
        {
            double tf1 = dimensions.TopFlangeThickness; //TODO: Allow for varying plate thickness
            double tw = dimensions.WebThickness;
            double width = dimensions.Width;
            double height = dimensions.Height;

            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        public static double TorsionalConstant(this StandardBoxDimensions dimensions)
        {
            double tf1 = dimensions.Thickness;
            double tw = dimensions.Thickness;
            double width = dimensions.Width;
            double height = dimensions.Height;

            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        public static double TorsionalConstant(this FabricatedISectionDimensions dimensions)
        {
            double b1 = dimensions.TopFlangeWidth;
            double b2 = dimensions.BotFlangeWidth;
            double height = dimensions.Height;
            double tf1 = dimensions.TopFlangeThickness;
            double tf2 = dimensions.BotFlangeThickness;
            double tw = dimensions.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - tf1) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this StandardISectionDimensions dimensions)
        {
            double b1 = dimensions.Width;
            double b2 = dimensions.Width;
            double height = dimensions.Height;
            double tf = dimensions.FlangeThickness;
            double tw = dimensions.WebThickness;

            return (b1 * Math.Pow(tf, 3) + b2 * Math.Pow(tf, 3) + (height - tf) * Math.Pow(tw, 3)) / 3;
        }


        /***************************************************/

        public static double TorsionalConstant(this StandardChannelSectionDimensions dimensions)
        {
            double b1 = dimensions.FlangeWidth;
            double b2 = dimensions.FlangeWidth;
            double height = dimensions.Height;
            double tf = dimensions.FlangeThickness;
            double tw = dimensions.WebThickness;

            return (b1 * Math.Pow(tf, 3) + b2 * Math.Pow(tf, 3) + (height - tf) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this StandardZedSectionDimensions dimensions)
        {
            double b1 = dimensions.FlangeWidth;
            double b2 = dimensions.FlangeWidth;
            double height = dimensions.Height;
            double tf1 = dimensions.FlangeThickness;
            double tf2 = dimensions.FlangeThickness;
            double tw = dimensions.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - tf1) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this StandardTeeSectionDimensions dimensions)
        {
            double totalWidth = dimensions.Width;
            double totalDepth = dimensions.Width;
            double height = dimensions.Height;
            double tf1 = dimensions.FlangeThickness;
            double tw = dimensions.WebThickness;

            return totalWidth * Math.Pow(tf1, 3) + totalDepth * Math.Pow(tw, 3);
        }

        /***************************************************/

        public static double TorsionalConstant(this RectangleSectionDimensions dimensions)
        {
            if (Math.Abs(dimensions.Height - dimensions.Width) < Tolerance.Distance)
                return 2.25 * Math.Pow(dimensions.Height, 4);
            else
            {
                double a = Math.Max(dimensions.Height, dimensions.Width);
                double b = Math.Min(dimensions.Height, dimensions.Width);
                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
            }
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double ITorsionalConstant(this ISectionDimensions dimensions)
        {
            return TorsionalConstant(dimensions as dynamic);
        }

        /***************************************************/
    }
}
