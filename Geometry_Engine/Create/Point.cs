/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

        [Description("Creates a Point based on its coordinates.")]
        [InputFromProperty("x")]
        [InputFromProperty("y")]
        [InputFromProperty("z")]
        [Output("point", "The created Point.")]
        public static Point Point(double x = 0, double y = 0, double z = 0)
        {
            return new Point { X = x, Y = y, Z = z };
        }

        /***************************************************/

        [Description("Creates a Point from a Vector, assuming the vector is a Position vector from the global origin, e.g. a Point with the same coordinates as the provided Vector.")]
        [Input("v", "The position vector to create a point to create a point based on.")]
        [Output("point", "The created Point.")]
        public static Point Point(Vector v)
        {
            return new Point { X = v.X, Y = v.Y, Z = v.Z };
        }

        /***************************************************/

        [Description("Creates a two dimensional grid of points along the two provided vectors.")]
        [Input("start", "Base point of the grid.")]
        [Input("dir1", "First direction of the grid. Spacing in this direction will be determined by the length of the vector.")]
        [Input("dir2", "Second direction of the grid. Spacing in this direction will be determined by the length of the vector.")]
        [Input("nbPts1", "Number of points along the first direction.")]
        [Input("nbPts2", "Number of points along the second direction.")]
        [Output("grid", "The created grid of points as a nested list, where each inner list corresponds to all values along the first vector.")]
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
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Point based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("point", "The generated random Point.")]
        public static Point RandomPoint(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomPoint(new Random(seed), box);
        }

        /***************************************************/

        [Description("Creates a random Point using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("point", "The generated random Point.")]
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
    }
}





