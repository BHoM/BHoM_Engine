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
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> SamplePoints(this ICurve curve, double step)
        {
            if (step <= 0)
                throw new ArgumentException("step value must be greater than 0");

            List<Point> points = new List<Point>();
            double dist = 0;

            while (dist <= curve.ILength())
            {
                points.Add(curve.IPointAtLength(dist));
                dist += step;
            }

            return points;
        }

        /***************************************************/

        public static List<Point> SamplePoints(this ICurve curve, int number)
        {
            if (number <= 0)
                throw new ArgumentException("number value must be greater than 0");

            List<Point> points = new List<Point>();
            double length = curve.ILength();
            double step = length / number;
            points.Add(curve.IStartPoint());

            for (int i = 1; i < number; i++)
            {
                points.Add(curve.IPointAtLength(i * step));
            }

            points.Add(curve.IEndPoint());
            return points;
        }

        /***************************************************/
    }
}




