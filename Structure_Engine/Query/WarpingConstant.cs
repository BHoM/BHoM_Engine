using System;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static double WarpingConstant(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
        //{

        //    switch (shape)
        //    {
        //        case ShapeType.ISection:
        //            if (tf1 == tf2 && b1 == b2)
        //            {
        //                return tf1 * Math.Pow(totalDepth - tf1, 2) * Math.Pow(totalWidth, 3) / 24;
        //            }
        //            else
        //            {
        //                return tf1 * Math.Pow(totalDepth - (tf1 + tf2) / 2, 2) / 12 * (Math.Pow(b1, 3) * Math.Pow(b2, 3) / (Math.Pow(b1, 3) + Math.Pow(b2, 3)));
        //            }
        //        case ShapeType.Channel:
        //            return tf1 * Math.Pow(totalDepth, 2) / 12 * (3 * b1 * tf1 + 2 * totalDepth * tw / (6 * b1 * tf1 + totalDepth * tw));
        //        default:
        //            return 0;

        //    }
        //}


        //TODO: Implement more warping constants

        /***************************************************/
        
        public static double WarpingConstant(this IProfile profile)
        {
            return 0; // Return 0 for not specifically implemented ones
        }

        /***************************************************/

        public static double WarpingConstant(this ISectionProfile profile)
        {
            double width = profile.Width;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;


            return tf * Math.Pow(height - tf, 2) * Math.Pow(width, 3) / 24;

        }

        /***************************************************/

        public static double WarpingConstant(this FabricatedISectionProfile profile)
        {
            double b1 = profile.TopFlangeWidth;
            double b2 = profile.BotFlangeWidth;
            double height = profile.Height;
            double tf1 = profile.TopFlangeThickness;
            double tf2 = profile.BotFlangeThickness;
            double tw = profile.WebThickness;


            if (tf1 == tf2 && b1 == b2)
            {
                return tf1 * Math.Pow(height - tf1, 2) * Math.Pow(b1, 3) / 24;
            }
            else
            {
                return tf1 * Math.Pow(height - (tf1 + tf2) / 2, 2) / 12 * (Math.Pow(b1, 3) * Math.Pow(b2, 3) / (Math.Pow(b1, 3) + Math.Pow(b2, 3)));
            }
        }

        /***************************************************/

        public static double WarpingConstant(this ChannelProfile profile)
        {
            double width = profile.FlangeWidth;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;


            return tf * Math.Pow(height, 2) / 12 * (3 * width * tf + 2 * height * tw / (6 * width * tf + height * tw));

        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IWarpingConstant(this IProfile profile)
        {
            return WarpingConstant(profile as dynamic);
        }

        /***************************************************/
    }
}
