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

using System.Collections.Generic;
using BH.oM.Geometry;


namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                           ****/
        /***************************************************/

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
        
        public static double AreaIntegration(List<IntegrationSlice> slices, double curve, double from, double to, ref double centroid)
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
        
        public static double AreaIntegration(List<IntegrationSlice> slices, double constant, double xPower, double yPower, double origin = 0)
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
        
        public static double AreaIntegration(List<IntegrationSlice> slices, double constant, double xPower, double yPower, double from = double.MinValue, double to = double.MaxValue, double origin = 0)
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

        public static double AreaIntegration(List<IntegrationSlice> slices, Vector direction, ICurve curve, double from, double to, ref double centroid)
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

        public static double AreaIntegration(List<IntegrationSlice> solid, List<IntegrationSlice> voids, Vector direction, ICurve curve, double from, double to, ref double centroid)
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




