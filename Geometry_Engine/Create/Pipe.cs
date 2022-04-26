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
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Pipe Pipe(ICurve centreline, double radius, bool capped = true)
        {
            return new Pipe
            {
                Centreline = centreline,
                Radius = radius,
                Capped = capped
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Pipe RandomPipe(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomPipe(rnd, box);
        }

        /***************************************************/

        public static Pipe RandomPipe(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                return new Pipe
                {
                    Centreline = RandomCurve(rnd),
                    Radius = rnd.NextDouble(),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }
            else
            {
                ICurve curve = RandomCurve(rnd, box);
                BoundingBox curveBox = curve.IBounds();
                double maxRadius = new double[]
                {
                    box.Max.X - curveBox.Max.X,
                    box.Max.Y - curveBox.Max.Y,
                    box.Max.Z - curveBox.Max.Z,
                    curveBox.Min.X - box.Min.X,
                    curveBox.Min.Y - box.Min.Y,
                    curveBox.Min.Z - box.Min.Z
                }.Min();

                return new Pipe
                {
                    Centreline = curve,
                    Radius = maxRadius * rnd.NextDouble(),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }
        }

        /***************************************************/
    }
}



