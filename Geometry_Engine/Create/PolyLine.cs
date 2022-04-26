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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Polyline(IEnumerable<Point> points)
        {
            return new Polyline { ControlPoints = points.ToList() };
        }

        /***************************************************/

        public static Polyline Polyline(List<Line> lines)
        {
            if (lines.Count == 0)
                return null;

            List<Point> controlPoints = new List<Point> { lines[0].Start };
            foreach (Line l in lines)
            {
                controlPoints.Add(l.End);
            }
            return new Polyline { ControlPoints = controlPoints };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Polyline RandomPolyline(int seed = -1, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomPolyline(rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static Polyline RandomPolyline(Random rnd, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1); i++)
                points.Add(RandomPoint(rnd, box));
            return new Polyline { ControlPoints = points };
        }

        /***************************************************/

        public static Polyline RandomPolyline(Point from, int seed = -1, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomPolyline(from, rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static Polyline RandomPolyline(Point from, Random rnd, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            points.Add(from);
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1) - 1; i++)
            {
                points.Add(RandomPoint(rnd, box));
            }

            return new Polyline { ControlPoints = points };
        }

        /***************************************************/
    }
}



