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

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static double GetWarpingConstant(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
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
        //Return 0 for not specifically implemented ones
        public static double GetWarpingConstant(this ISectionDimensions dimensions)
        {
            return 0;
        }

        /***************************************************/

        public static double GetWarpingConstant(this StandardISectionDimensions dimensions)
        {
            double width = dimensions.Width;
            double height = dimensions.Height;
            double tf = dimensions.FlangeThickness;
            double tw = dimensions.WebThickness;


            return tf * Math.Pow(height - tf, 2) * Math.Pow(width, 3) / 24;

        }

        /***************************************************/

        public static double GetWarpingConstant(this FabricatedISectionDimensions dimensions)
        {
            double b1 = dimensions.TopFlangeWidth;
            double b2 = dimensions.BotFlangeWidth;
            double height = dimensions.Height;
            double tf1 = dimensions.TopFlangeThickness;
            double tf2 = dimensions.BotFlangeThickness;
            double tw = dimensions.WebThickness;


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

        public static double GetWarpingConstant(this StandardChannelSectionDimensions dimensions)
        {
            double width = dimensions.FlangeWidth;
            double height = dimensions.Height;
            double tf = dimensions.FlangeThickness;
            double tw = dimensions.WebThickness;


            return tf * Math.Pow(height, 2) / 12 * (3 * width * tf + 2 * height * tw / (6 * width * tf + height * tw));

        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IGetWarpingConstant(this ISectionDimensions dimensions)
        {
            return GetWarpingConstant(dimensions as dynamic);
        }

    }
}
