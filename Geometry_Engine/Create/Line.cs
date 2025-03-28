/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Line based on its start and end points.")]
        [InputFromProperty("start")]
        [InputFromProperty("end")]
        [Output("line", "The created Line.")]
        public static Line Line(Point start, Point end)
        {
            return new Line
            {
                Start = start,
                End = end
            };
        }

        /***************************************************/

        [Description("Creates an infinite Line based on its start point and a direction vector.")]
        [InputFromProperty("start")]
        [Input("direction", "The direction of the infinite line")]
        [Output("ray", "The created infinite line Line.")]
        public static Line Line(Point start, Vector direction)
        {
            return new Line
            {
                Start = start,
                End = start + direction,
                Infinite = true
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Line based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("line", "The generated random Line.")]
        public static Line RandomLine(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomLine(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Line using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("line", "The generated random Line.")]
        public static Line RandomLine(Random rnd, BoundingBox box = null)
        {
            return new Line
            {
                Start = RandomPoint(rnd, box),
                End = RandomPoint(rnd, box)
            };
        }

        /***************************************************/

        [Description("Creates a random Line with a set start point based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the Line.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("line", "The generated random Line.")]
        public static Line RandomLine(Point from, int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomLine(from, rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Line with a set start point using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the Line.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("line", "The generated random Line.")]
        public static Line RandomLine(Point from, Random rnd, BoundingBox box = null)
        {
            return new Line
            {
                Start = from,
                End = RandomPoint(rnd, box)
            };
        }

        /***************************************************/
    }
}






