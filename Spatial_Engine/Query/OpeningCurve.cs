/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the curve of a single opening in the XY plane.")]
        [Input("opening", "The cellular opening to get the opening curve from.")]
        [Output("curve", "The outline curve of a single opening.")]
        public static ICurve OpeningCurve(this CircularOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the curve from a null opening.");
                return null;
            }
            double r = opening.Diameter / 2;
            return new Circle { Centre = Point.Origin, Normal = Vector.ZAxis, Radius = r };
        }

        /***************************************************/

        [Description("Returns the curve of a single opening in the XY plane.")]
        [Input("opening", "The cellular opening to get the opening curve from.")]
        [Output("curve", "The outline curve of a single opening.")]
        public static Polyline OpeningCurve(this HexagonalOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the curve from a null opening.");
                return null;
            }

            List<Point> ctrlPts;
            if (opening.SpacerHeight == 0)
            {
                ctrlPts = new List<Point>
                {
                    new Point{ X = opening.WidthWebPost/2, Y = -opening.Height/2 },
                    new Point{ X = opening.Width/2},
                    new Point{ X = opening.WidthWebPost/2, Y = opening.Height/2},
                    new Point{ X = -opening.WidthWebPost/2, Y = opening.Height/2 },
                    new Point{ X = -opening.Width/2},
                    new Point{ X = -opening.WidthWebPost/2, Y = -opening.Height/2 },
                    new Point{ X = opening.WidthWebPost/2, Y = -opening.Height/2 },
                };
            }
            else
            {
                double h = opening.Height + opening.SpacerHeight;
                ctrlPts = new List<Point>
                {
                    new Point{ X = opening.WidthWebPost / 2, Y = - h / 2 },
                    new Point{ X = opening.Width/2, Y = - opening.SpacerHeight/2},
                    new Point{ X = opening.Width/2, Y = opening.SpacerHeight/2},
                    new Point{ X = opening.WidthWebPost/2, Y = h/2},
                    new Point{ X = -opening.WidthWebPost/2, Y =h/2 },
                    new Point{ X = -opening.Width/2, Y = opening.SpacerHeight/2},
                    new Point{ X = -opening.Width/2, Y = -opening.SpacerHeight/2},
                    new Point{ X = -opening.WidthWebPost/2, Y = -h/2 },
                    new Point{ X = opening.WidthWebPost/2, Y = -h/2 },
                };
            }

            return new Polyline { ControlPoints = ctrlPts };
        }

        /***************************************************/

        [Description("Returns the curve of a single opening in the XY plane.")]
        [Input("opening", "The cellular opening to get the opening curve from.")]
        [Output("curve", "The outline curve of a single opening.")]
        public static PolyCurve OpeningCurve(this SinusoidalOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the curve from a null opening.");
                return null;
            }

            Point basePt = new Point() { X = opening.WidthWebPost / 2, Y = -opening.Height / 2 };

            NurbsCurve s1 = ApproxSine(basePt, opening.Height / 2, opening.SinusoidalLength);
            NurbsCurve s2 = s1.Mirror(Plane.XZ).Flip();
            NurbsCurve s3 = s2.Mirror(Plane.YZ).Flip();
            NurbsCurve s4 = s3.Mirror(Plane.XZ).Flip();

            return new PolyCurve
            {
                Curves = new List<ICurve>
                {
                    s1,
                    s2,
                    new Line{ Start = s2.EndPoint(), End = s3.StartPoint() },
                    s3,
                    s4,
                    new Line{ Start = s4.EndPoint(), End = s1.StartPoint() }
                }
            };
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns the curve of a single opening in the XY plane.")]
        [Input("opening", "The cellular opening to fetch the opening curve from.")]
        [Output("curve", "The outline curve of a single opening.")]
        public static ICurve IOpeningCurve(this ICellularOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the curve from a null opening.");
                return null;
            }
            return OpeningCurve(opening as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static NurbsCurve ApproxSine(Point basePt, double h, double w)
        {
            double quarterScale = 0.18217;

            Vector x = Vector.XAxis;
            Vector y = Vector.YAxis;

            List<Point> ctrlPts = new List<Point>
            {
                basePt,
                basePt + x * quarterScale * w,
                basePt + x * w * 0.5 + y * h * 0.5,
                basePt + x * (1 - quarterScale) * w + y * h,
                basePt + x * w + y * h,
            };

            return new NurbsCurve
            {
                ControlPoints = ctrlPts,
                Knots = new List<double>() { 0, 0, 0, 0.5, 1, 1, 1 },
                Weights = new List<double> { 1, 1, 1, 1, 1 }
            };
        }

        /***************************************************/
    }
}
