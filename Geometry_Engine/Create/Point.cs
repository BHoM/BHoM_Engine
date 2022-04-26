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

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point Point(double x = 0, double y = 0, double z = 0)
        {
            return new Point { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static Point Point(Vector v)
        {
            return new Point { X = v.X, Y = v.Y, Z = v.Z };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Point RandomPoint(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomPoint(new Random(seed), box);
        }

        /***************************************************/

        public static Point RandomPoint(Random rnd, BoundingBox box = null)
        {
            if (box != null)
            {
                return new Point
                {
                    X = box.Min.X + rnd.NextDouble() * (box.Max.X - box.Min.X),
                    Y = box.Min.Y + rnd.NextDouble() * (box.Max.Y - box.Min.Y),
                    Z = box.Min.Z + rnd.NextDouble() * (box.Max.Z - box.Min.Z)
                };
            }
            else
                return new Point { X = rnd.NextDouble(), Y = rnd.NextDouble(), Z = rnd.NextDouble() };
        }

        /***************************************************/

        public static List<List<Point>> PointGrid(Point start, Vector dir1, Vector dir2, int nbPts1, int nbPts2)
        {
            List<List<Point>> pts = new List<List<Point>>();
            for (int i = 0; i < nbPts1; i++)
            {
                List<Point> row = new List<Point>();
                for (int j = 0; j < nbPts2; j++)
                {
                    row.Add(start + i * dir1 + j * dir2);
                }
                pts.Add(row);
            }

            return pts;
        }
        
        /***************************************************/
    }
}



