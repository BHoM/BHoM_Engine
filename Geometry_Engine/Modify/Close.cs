/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;

using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Close open polyline by adding first point to the end of control points.")]
        [Input("polyline", "Polyline to close.")]
        [Input("tolerance", "Tolerance used in the method.")]
        [Output("polyline", "Closed polyline.")]
        public static Polyline Close(this Polyline polyline, double tolerance = Tolerance.Distance)
        {
            if (polyline == null || polyline.ControlPoints.Count == 0 || polyline.IsClosed(tolerance))
            {
                return polyline;
            }

            List<Point> polylinePoints = polyline.ControlPoints.ToList();
            polylinePoints.Add(polylinePoints[0]);

            return Create.Polyline(polylinePoints);
        }

        /***************************************************/

        [Description("Close open polyline by adding first point to the end of control points.")]
        [Input("polyline", "Polyline to close.")]
        [Input("tolerance", "Tolerance used in the method.")]
        [Output("polyline", "Closed polyline.")]
        public static PolyCurve Close(this PolyCurve polyCurve, double tolerance = Tolerance.Distance)
        {
            if (polyCurve == null || polyCurve.Curves.Count == 0 || polyCurve.IsClosed(tolerance))
            {
                return polyCurve;
            }

            List<ICurve> curves = polyCurve.Curves.ToList();
            curves.Add(new Line { Start = polyCurve.EndPoint(), End = polyCurve.StartPoint()});

            return new PolyCurve { Curves = curves };
        }

        /***************************************************/
    }
}



