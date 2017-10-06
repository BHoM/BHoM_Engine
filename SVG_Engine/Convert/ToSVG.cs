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

        public static string ToSVG(this Point point)
        {
            throw new NotImplementedException();
        }

        public static string ToSVG(this Line line)
        {
            // Converts a BHoM Line into SVG

            Point startPt = line.Start;
            Point endPt = line.End;

            string lineString = "<line x1=\"_x1\" y1=\"_y1\" z1=\"_z1\" x2=\"_x2\" y2=\"_y2\" z2=\"_z2\" vector-effect=\"non-scaling-stroke\" />" + System.Environment.NewLine;

            lineString = lineString.Replace("_x1", startPt.X.ToString());
            lineString = lineString.Replace("_y1", startPt.Y.ToString());
            lineString = lineString.Replace("_z1", startPt.Z.ToString());
            lineString = lineString.Replace("_x2", endPt.X.ToString());
            lineString = lineString.Replace("_y2", endPt.Y.ToString());
            lineString = lineString.Replace("_z2", endPt.Z.ToString());

            return lineString;
        } 

        public static string ToSVG(this List<BH.oM.Geometry.Point> ptList, bool closed)
        { 
            // Converts a Path into SVG

            string pathString = "<path d=\"";

            for (int i = 0; i< ptList.Count; i++)
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

        public static string ToSVG(this Circle circle)
        {
            // Converts a BHoM Circle into SVG

            Point centerPt = circle.Centre;
            Double radius = circle.Radius;

            string circleString = "<circle cx=\"_cx\" cy=\"_cy\" r=\"radius\" stroke=\"\" stroke-width=\"\" fill=\"\" />";

            circleString = circleString.Replace("_cx", centerPt.X.ToString());
            circleString = circleString.Replace("_cy", centerPt.Y.ToString());

            return circleString;
        }

        public static string ToSVG(Polyline polyline)
        {
            // Converts a BHoM Polyline into SVG

            //List<Point> controlPts = polyline.ControlPoints;

            //string polylineString = "<polyline d=\"";

            //for (int i = 0; i < controlPts.Count; i++)
            //{
            //    polylineString += controlPts[i].X.ToString,controlPts[i].Y.ToString " ";
            //}

            //polylineString += "style="fill:\"\";stroke:\"\";stroke-width:\"\" />";

            //return polylineString;

            throw new NotImplementedException();
        }

        //public static string DrawSVGpath(this IBHoMGeometry geometry)
        //{
        //    // sopme code
        //}

        //public static string ToSvg(this Arc arc)
        //{
        //    DrawSVGpath(arc);


    }
}