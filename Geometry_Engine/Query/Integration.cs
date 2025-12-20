/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;


namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                           ****/
        /***************************************************/

        [Description("Performs numerical integration along a curve in the specified direction.")]
        [Input("fx", "The curve to integrate along.")]
        [Input("direction", "The direction vector for integration.")]
        [Input("from", "The starting position for integration.", typeof(Length))]
        [Input("to", "The ending position for integration.", typeof(Length))]
        [Input("centroid", "Reference parameter to receive the centroid position.")]
        [Input("increment", "The increment step size for numerical integration.", typeof(Length))]
        [Output("result", "The integrated value.")]
        public static double CurveIntegration(this ICurve fx, Vector direction, double from, double to, ref double centroid, double increment = 0.001)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            double sumAreaLength = 0;
            int segments = (int)((max - min) / increment);
            increment = (max - min) / (double)(segments + 1);
            Point origin = Point.Origin;
            Plane plane = new Plane { Origin = origin, Normal = direction };

            for (double dx = min; dx < max; dx += increment)
            {
                double currentCentre = dx + increment / 2;
                double sliceWidth = (increment);
                plane.Origin = (origin + plane.Normal * currentCentre);
                List<Point> points = fx.IPlaneIntersections(plane, 0.001);
                double currentValue = 0;

                if (points.Count == 2)
                    currentValue = System.Math.Abs(points[0].Y - points[1].Y);
                else if (points.Count == 1)
                    currentValue = points[0].Y;

                result += currentValue * sliceWidth;
                sumAreaLength += currentValue * sliceWidth * currentCentre;
            }

            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /***************************************************/
        
        [Description("Performs area integration over a collection of integration slices with a curve multiplier.")]
        [Input("slices", "The collection of integration slices to integrate over.")]
        [Input("curve", "The curve multiplier value.")]
        [Input("from", "The starting position for integration.", typeof(Length))]
        [Input("to", "The ending position for integration.", typeof(Length))]
        [Input("centroid", "Reference parameter to receive the centroid position.")]
        [Output("result", "The integrated area value.", typeof(Area))]
        public static double AreaIntegration(this List<IntegrationSlice> slices, double curve, double from, double to, ref double centroid)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);

            double sumAreaLength = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double currentCentre = (topSlice + botSlice) / 2;
                    double currentValue = curve;
                    double sliceWidth = (topSlice - botSlice);
                    result += currentValue * slice.Length * sliceWidth;
                    sumAreaLength += currentValue * slice.Length * sliceWidth * currentCentre;
                }
            }

            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /***************************************************/

        [Description("Performs area integration over integration slices using power functions.")]
        [Input("slices", "The collection of integration slices to integrate over.")]
        [Input("constant", "The constant multiplier.")]
        [Input("xPower", "The power to raise x coordinates to.")]
        [Input("yPower", "The power to raise y coordinates to.")]
        [Input("origin", "The origin offset for the calculation.", typeof(Length))]
        [Output("result", "The integrated value.")]
        public static double AreaIntegration(this List<IntegrationSlice> slices, double constant, double xPower, double yPower, double origin = 0)
        {
            double result = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                double dx = slice.Width;
                result += constant * System.Math.Pow(slice.Centre - origin, xPower) * System.Math.Pow(slice.Length, yPower) * dx;
            }

            return result;
        }

        /***************************************************/

        [Description("Performs area integration over integration slices using power functions within specified bounds.")]
        [Input("slices", "The collection of integration slices to integrate over.")]
        [Input("constant", "The constant multiplier.")]
        [Input("xPower", "The power to raise x coordinates to.")]
        [Input("yPower", "The power to raise y coordinates to.")]
        [Input("from", "The starting position for integration.", typeof(Length))]
        [Input("to", "The ending position for integration.", typeof(Length))]
        [Input("origin", "The origin offset for the calculation.", typeof(Length))]
        [Output("result", "The integrated value.")]
        public static double AreaIntegration(this List<IntegrationSlice> slices, double constant, double xPower, double yPower, double from = double.MinValue, double to = double.MaxValue, double origin = 0)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            
            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double sliceCentre = (topSlice + botSlice) / 2;
                    double dx = (topSlice - botSlice);
                    result += constant * System.Math.Pow(sliceCentre - origin, xPower) * System.Math.Pow(slice.Length, yPower) * dx;
                }
            }

            return result;
        }

        /***************************************************/

        [Description("Performs area integration over integration slices using a curve in the specified direction.")]
        [Input("slices", "The collection of integration slices to integrate over.")]
        [Input("direction", "The direction vector for integration.")]
        [Input("curve", "The curve to use for integration.")]
        [Input("from", "The starting position for integration.", typeof(Length))]
        [Input("to", "The ending position for integration.", typeof(Length))]
        [Input("centroid", "Reference parameter to receive the centroid position.")]
        [Output("result", "The integrated area value.", typeof(Area))]
        public static double AreaIntegration(this List<IntegrationSlice> slices, Vector direction, ICurve curve, double from, double to, ref double centroid)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            double sumAreaLength = 0;
            Point origin = Point.Origin;
            Plane plane = new Plane { Origin = origin, Normal = direction };

            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double currentCentre = (topSlice + botSlice) / 2;
                    double sliceWidth = (topSlice - botSlice);
                    plane.Origin = (origin + plane.Normal * currentCentre);
                    List<Point> points = curve.IPlaneIntersections(plane, 0.001);
                    double currentValue = 0;

                    if (points.Count == 2)
                        currentValue = System.Math.Abs(points[0].Y - points[1].Y);
                    else if (points.Count == 1)
                        currentValue = points[0].Y;

                    result += currentValue * slice.Length * sliceWidth;
                    sumAreaLength += currentValue * slice.Length * sliceWidth * currentCentre;
                }
            }

            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /***************************************************/

        [Description("Performs area integration over solid slices minus void slices using a curve in the specified direction.")]
        [Input("solid", "The collection of solid integration slices.")]
        [Input("voids", "The collection of void integration slices to subtract.")]
        [Input("direction", "The direction vector for integration.")]
        [Input("curve", "The curve to use for integration.")]
        [Input("from", "The starting position for integration.", typeof(Length))]
        [Input("to", "The ending position for integration.", typeof(Length))]
        [Input("centroid", "Reference parameter to receive the centroid position.")]
        [Output("result", "The integrated area value after subtracting voids.", typeof(Area))]
        public static double AreaIntegration(this List<IntegrationSlice> solid, List<IntegrationSlice> voids, Vector direction, ICurve curve, double from, double to, ref double centroid)
        {
            double centroidSolid = 0;
            double centroidVoid = 0;

            double intSolid = AreaIntegration(solid, direction, curve, from, to, ref centroidSolid);
            double intVoid = AreaIntegration(voids, direction, curve, from, to, ref centroidVoid);

            centroid = (intSolid * centroidSolid - intVoid * centroidVoid) / (intSolid - intVoid);
            return intSolid - intVoid;
        }

        /***************************************************/
    }
}
