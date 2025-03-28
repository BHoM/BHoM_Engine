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

        [Description("Creates a Vector based on its coordinates.")]
        [InputFromProperty("x")]
        [InputFromProperty("y")]
        [InputFromProperty("z")]
        [Output("vector", "The created Vector.")]
        public static Vector Vector(double x = 0, double y = 0, double z = 0)
        {
            return new Vector { X = x, Y = y, Z = z };
        }

        /***************************************************/

        [PreviousInputNames("p","v")]
        [Description("Creates a Vector that is the position Vector to the point from the global origin, e.g. a Vector with the same coordinates as the provided Point.")]
        [Input("p", "The point to create the position vector to.")]
        [Output("vector", "The created Vector.")]
        public static Vector Vector(Point p)
        {
            if (p == null)
                return null;
            return new Vector { X = p.X, Y = p.Y, Z = p.Z };
        }

        /***************************************************/

        [Description("Creates a Vector between two Points.")]
        [Input("a", "The start point of the Vector.")]
        [Input("b", "The end point of the Vector.")]
        [Output("vector", "The created Vector.")]
        public static Vector Vector(Point a, Point b)
        {
            return b - a;
        }

        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Vector based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The  vector coordinates created will be limited to the bounding box. If no box is provided, values between -1 and 1 will be used when generating properties for the geometry.")]
        [Output("vector", "The generated random Vector.")]
        public static Vector RandomVector(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomVector(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Vector using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The vector coordinates created will be limited to the bounding box. If no box is provided, values between -1 and 1 will be used when generating properties for the geometry.")]
        [Output("vector", "The generated random Vector.")]
        public static Vector RandomVector(Random rnd, BoundingBox box = null)
        {
            if (box != null)
            {
                return new Vector
                {
                    X = box.Min.X + rnd.NextDouble() * (box.Max.X - box.Min.X),
                    Y = box.Min.Y + rnd.NextDouble() * (box.Max.Y - box.Min.Y),
                    Z = box.Min.Z + rnd.NextDouble() * (box.Max.Z - box.Min.Z)
                };
            }
            else
                return new Vector { X = rnd.NextDouble()*2-1, Y = rnd.NextDouble()*2 - 1, Z = rnd.NextDouble()*2 - 1 };
        }

        /***************************************************/

        [Description("Creates a random Vector in the provided plane, e.g a vector that is orthogonal to the normal of the plane, based on a seed. If no seed is provided, a random one will be generated.")]
        [Input("plane", "The plane that the vector should be contained to.")]
        [Input("normalise", "If true the created vector will be a unit vector (length 1).")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Output("vector", "The generated random Vector in the plane.")]
        public static Vector RandomVectorInPlane(Plane plane, bool normalise = false, int seed = -1)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomVectorInPlane(plane, rnd, normalise);
        }

        /***************************************************/

        [Description("Creates a random Vector in the provided plane, e.g a vector that is orthogonal to the normal of the plane, using the provided Random class.")]
        [Input("plane", "The plane that the vector should be contained to.")]
        [Input("normalise", "If true the created vector will be a unit vector (length 1).")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Output("vector", "The generated random Vector in the plane.")]
        public static Vector RandomVectorInPlane(Plane plane, Random rnd, bool normalise = false)
        {
            Vector v1 = RandomVector(rnd);

            if (v1.IsParallel(plane.Normal) != 0)
                return RandomVectorInPlane(plane, rnd, normalise);

            Vector v2 = v1.CrossProduct(plane.Normal);

            if (normalise)
                v2 = v2.Normalise();

            return v2;
        }

        /***************************************************/
    }
}






