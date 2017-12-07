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
        public static double GetWarpingConstant(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
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
    }
}
