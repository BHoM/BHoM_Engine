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
        public static double GetTorsionalConstant(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
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
                case ShapeType.Rectangle:
                    if (Math.Abs(totalDepth - totalWidth) < Tolerance.Distance)
                        return 2.25 * Math.Pow(totalDepth, 4);
                    else
                    {
                        double a = Math.Max(totalDepth, totalWidth);
                        double b = Math.Min(totalDepth, totalWidth);
                        return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
                    }
                default:
                    return 0;
            }
        }


    }
}
