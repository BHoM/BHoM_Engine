using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.SVG
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToSVG(Point point)
        {
            throw new NotImplementedException();
        }

        public static string ToSVG(Line line)
        {
            Point start = line.Start;
            Point end = line.End;

            string lineString = "<line x1=\"_x1\" y1=\"_y1\" z1=\"_z1\" x2=\"_x2\" y2=\"_y2\" z2=\"_z2\" vector-effect=\"non-scaling-stroke\" />" + System.Environment.NewLine;

            lineString = lineString.Replace("_x1", start.X.ToString());
            lineString = lineString.Replace("_y1", start.Y.ToString());
            lineString = lineString.Replace("_z1", start.Z.ToString());
            lineString = lineString.Replace("_x2", end.X.ToString());
            lineString = lineString.Replace("_y2", end.Y.ToString());
            lineString = lineString.Replace("_z2", end.Z.ToString());

            return lineString;
        }

        public static string ToSVG(List<BH.oM.Geometry.Point> ptList, bool closed)
        { 
            string pathString = "<path d=\"";

            for (int i = 0; i< (ptList.Count); i++)
            {
                if (i == 0)
                {
                    pathString += "M " + ptList[i].X.ToString() + " " + ptList[i].Y.ToString() + " ";
                }
                else
                {
                    pathString += "L " + ptList[i].X.ToString() + " " + ptList[i].Y.ToString() + " ";
                }
            }

            if (closed)
            {
                pathString += "Z";
            }

            pathString += "\" vector-effect=\"non-scaling-stroke\" />" + System.Environment.NewLine;

            return pathString;
        }







    }
}