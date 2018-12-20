/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Numerics
{

    public class Slice
    {
        public double Width;
        public double Length;
        public double Centre;
        public double[] Placement;

        public Slice(double width, double length, double centre, double[] placement)
        {
            Width = width;
            Length = length;
            Centre = centre;
            Placement = placement;
        }

        public override string ToString()
        {
            return (Width * Length).ToString();
        }
    }

    public static class Integration
    {
        public static List<Slice> CreateSlices(Group<Curve> edges, Vector direction, double increment = 0.001)
        {
            List<Slice> slices = new List<Slice>();

            List<double> cutAt = new List<double>();
            List<double> sliceSegments = new List<double>();
            Plane p = new BH.oM.Geometry.Plane(Point.Origin, direction);
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges[i].PointCount; j++)
                {
                    cutAt.Add(ArrayUtils.DotProduct(edges[i].ControlPoint(j), p.Normal));
                }
            }

            cutAt.Sort();
            cutAt = cutAt.Distinct<double>().ToList();


            double currentValue = ArrayUtils.DotProduct(edges.Bounds().Min, p.Normal);
            double max = ArrayUtils.DotProduct(edges.Bounds().Max, p.Normal);
            int index = 0;

            while (currentValue < max)
            {
                if (cutAt.Count > index && currentValue > cutAt[index])
                {
                    sliceSegments.Add(cutAt[index]);
                    index++;
                }
                else
                {
                    sliceSegments.Add(currentValue);
                    currentValue += increment;
                }
            }

            sliceSegments.Add(max);


            for (int i = 0; i < sliceSegments.Count - 1; i++)
            {
                if (sliceSegments[i] == sliceSegments[i + 1])
                {
                    continue;
                }

                currentValue = (sliceSegments[i] + sliceSegments[i + 1]) / 2;

                //for (int edgeIndex = 0; edgeIndex < m_Edges.Count; edgeIndex++)
                //{
                //    if (edgeIndex == 3)
                //    {

                //    }
                //    y.AddRange(Intersect.PlaneCurve(new Plane(new Point(p.Normal * currentValue), p.Normal), m_Edges[edgeIndex], 0.00001));
                //}

                //List<double> isolatedCoords = new List<double>();

                //for (int point = 0; point < y.Count; point++)
                //{
                //    if (p.Normal.X > 0)
                //    {
                //        isolatedCoords.Add(y[point].Y);
                //    }
                //    else
                //    {
                //        isolatedCoords.Add(y[point].X);
                //    }
                //}


                //isolatedCoords.Sort();

                //if (isolatedCoords.Count % 2 != 0)
                //{
                //    for (int k = 0; k < isolatedCoords.Count - 1; k++)
                //    {
                //        if (isolatedCoords[k] == isolatedCoords[k + 1])
                //        {
                //            isolatedCoords.RemoveAt(k + 1);
                //        }
                //    }
                //}

                //for (int j = 0; j < isolatedCoords.Count - 1; j += 2)
                //{
                //    length = length + isolatedCoords[j + 1] - isolatedCoords[j];
                //}

                slices.Add(GetSliceAt(edges, currentValue, -sliceSegments[i] + sliceSegments[i + 1], p));
                //new Slice(-sliceSegments[i] + sliceSegments[i + 1], length, currentValue, isolatedCoords.ToArray()));
            }
            return slices;
        }

        public static Slice GetSliceAt(Group<Curve> edges, double location, double width, Plane p)
        {
            List<Point> y = new List<Point>();
            double length = 0;
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                if (edgeIndex == 3)
                {

                }
                y.AddRange(Intersect.PlaneCurve(new Plane(new Point(p.Normal * location), p.Normal), edges[edgeIndex], 0.00001));
            }

            List<double> isolatedCoords = new List<double>();

            for (int point = 0; point < y.Count; point++)
            {
                if (p.Normal.X > 0)
                {
                    isolatedCoords.Add(y[point].Y);
                }
                else
                {
                    isolatedCoords.Add(y[point].X);
                }
            }

            isolatedCoords.Sort();

            if (isolatedCoords.Count % 2 != 0)
            {
                for (int k = 0; k < isolatedCoords.Count - 1; k++)
                {
                    if (isolatedCoords[k] == isolatedCoords[k + 1])
                    {
                        isolatedCoords.RemoveAt(k + 1);
                    }
                }
            }

            for (int j = 0; j < isolatedCoords.Count - 1; j += 2)
            {
                length = length + isolatedCoords[j + 1] - isolatedCoords[j];
            }
            return new Slice(width, length, location, isolatedCoords.ToArray());
        }

        public static double IntegrateCurve(Curve fx, Vector direction, double from, double to, ref double centroid, double increment = 0.001)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            double sumAreaLength = 0;
            int segments = (int)((max - min) / increment);
            increment = (max - min) / (double)(segments + 1);
            Point origin = Point.Origin;
            Plane plane = new Plane(origin, direction);
            for (double dx = min; dx < max; dx += increment)
            {
                double currentCentre = dx + increment / 2;
                double sliceWidth = (increment);
                plane.Origin = (origin + plane.Normal * currentCentre);
                List<Point> points = Intersect.PlaneCurve(plane, fx, 0.001);
                double currentValue = 0;
                if (points.Count == 2)
                {
                    currentValue = System.Math.Abs(points[0].Y - points[1].Y);
                }
                else if (points.Count == 1)
                {
                    currentValue = points[0].Y;
                }
                result += currentValue * sliceWidth;
                sumAreaLength += currentValue * sliceWidth * currentCentre;
            }
            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> n (where n is a constant value) integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double IntegrateArea(List<Slice> slices, double curve, double from, double to, ref double centroid)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);

            double sumAreaLength = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                Slice slice = slices[i];
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> n (where n is a constant value) integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double IntegrateArea(List<Slice> slices, double constant, double xPower, double yPower, double origin = 0)
        {
            double result = 0;
            
            for (int i = 0; i < slices.Count; i++)
            {
                Slice slice = slices[i];
                double dx = slice.Width;
                result += constant * System.Math.Pow(slice.Centre - origin, xPower) * System.Math.Pow(slice.Length, yPower)  * dx;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> n (where n is a constant value) integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double IntegrateArea(List<Slice> slices, double from, double to, double constant, double xPower, double yPower, double origin = 0, double min = double.MinValue, double max = double.MaxValue)
        {
            double result = 0;

            for (int i = 0; i < slices.Count; i++)
            {
                Slice slice = slices[i];
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="curve">f(x) -> integrated in the x direction</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static double IntegrateArea(List<Slice> slices, Vector direction, Curve curve, double from, double to, ref double centroid)
        {
            double result = 0;
            double max = System.Math.Max(from, to);
            double min = System.Math.Min(from, to);
            double sumAreaLength = 0;
            Point origin = Point.Origin;
            Plane plane = new Plane(origin, direction);
            for (int i = 0; i < slices.Count; i++)
            {
                Slice slice = slices[i];
                if (slice.Centre + slice.Width / 2 > min && slice.Centre - slice.Width / 2 < max)
                {
                    double botSlice = System.Math.Max(min, slice.Centre - slice.Width / 2);
                    double topSlice = System.Math.Min(max, slice.Centre + slice.Width / 2);
                    double currentCentre = (topSlice + botSlice) / 2;
                    double sliceWidth = (topSlice - botSlice);
                    plane.Origin = (origin + plane.Normal * currentCentre); 
                    List<Point> points = Intersect.PlaneCurve(plane, curve, 0.001);
                    double currentValue = 0;
                    if (points.Count == 2)
                    {
                        currentValue = System.Math.Abs(points[0].Y - points[1].Y);
                    }
                    else if (points.Count == 1)
                    {
                        currentValue = points[0].Y;
                    }
                    result += currentValue * slice.Length * sliceWidth;
                    sumAreaLength += currentValue * slice.Length * sliceWidth * currentCentre;
                }
            }
            centroid = result != 0 ? sumAreaLength / result : 0;
            return result;
        }

        public static double IntegrateArea(List<Slice> solid, List<Slice> voids, Vector direction, Curve curve, double from, double to, ref double centroid)
        {
            double centroidSolid = 0;
            double centroidVoid = 0;

            double intSolid = IntegrateArea(solid, direction, curve, from, to, ref centroidSolid);
            double intVoid = IntegrateArea(voids, direction, curve, from, to, ref centroidVoid);

            centroid = (intSolid * centroidSolid - intVoid * centroidVoid) / (intSolid - intVoid); 

            return intSolid - intVoid;
        }
    }
}
