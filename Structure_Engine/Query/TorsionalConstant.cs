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
        
        public static double TorsionalConstant(this IProfile profile) 
        {
            return 0; //Return 0 for not specifically implemented ones
        }

        /***************************************************/

        public static double TorsionalConstant(this CircleProfile profile)
        {
            return Math.PI * Math.Pow(profile.Diameter, 4) / 32;
        }

        /***************************************************/

        public static double TorsionalConstant(this TubeProfile profile)
        {
            return Math.PI * (Math.Pow(profile.Diameter, 4) - Math.Pow(profile.Diameter - 2* profile.Thickness, 4)) / 32;
        }

        /***************************************************/

        public static double TorsionalConstant(this FabricatedBoxProfile profile)
        {
            double tf1 = profile.TopFlangeThickness; //TODO: Allow for varying plate thickness
            double tw = profile.WebThickness;
            double width = profile.Width;
            double height = profile.Height;


            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        public static double TorsionalConstant(this BoxProfile profile)
        {
            double tf1 = profile.Thickness;
            double tw = profile.Thickness;
            double width = profile.Width;
            double height = profile.Height;



            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        public static double TorsionalConstant(this FabricatedISectionProfile profile)
        {
            double b1 = profile.TopFlangeWidth;
            double b2 = profile.BotFlangeWidth;
            double height = profile.Height;
            double tf1 = profile.TopFlangeThickness;
            double tf2 = profile.BotFlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - (tf1 + tf2) / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this ISectionProfile profile)
        {
            double b1 = profile.Width;
            double b2 = profile.Width;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf, 3) + b2 * Math.Pow(tf, 3) + (height - tf) * Math.Pow(tw, 3)) / 3;
        }


        /***************************************************/

        public static double TorsionalConstant(this ChannelProfile profile)
        {
            double b = profile.FlangeWidth;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (2 * (b - tw / 2) * Math.Pow(tf, 3) + (height - tf) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this ZSectionProfile profile)
        {
            double b1 = profile.FlangeWidth;
            double b2 = profile.FlangeWidth;
            double height = profile.Height;
            double tf1 = profile.FlangeThickness;
            double tf2 = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - tf1) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this TSectionProfile profile)
        {
            double totalWidth = profile.Width;
            double totalDepth = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (totalWidth * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this AngleProfile profile)
        {
            double totalWidth = profile.Width;
            double totalDepth = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return ((totalWidth - tw / 2) * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        public static double TorsionalConstant(this RectangleProfile profile)
        {
            if (Math.Abs(profile.Height - profile.Width) < Tolerance.Distance)
                return 2.25 * Math.Pow(profile.Height/2, 4);
            else
            {
                double a = Math.Max(profile.Height, profile.Width)/2;
                double b = Math.Min(profile.Height, profile.Width)/2;
                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
            }
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double ITorsionalConstant(this IProfile profile)
        {
            return TorsionalConstant(profile as dynamic);
        }

        /***************************************************/
    }
}
