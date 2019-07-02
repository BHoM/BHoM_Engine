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

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Vector(double x = 0, double y = 0, double z = 0)
        {
            return new Vector { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static Vector Vector(Point v)
        {
            return new Vector { X = v.X, Y = v.Y, Z = v.Z };
        }

        public static Vector Vector(Point v,string name)
        {
            return new Vector { X = v.X, Y = v.Y, Z = v.Z };
        }
        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Vector RandomVector(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomVector(rnd, box);
        }

        /***************************************************/

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

        public static Vector RandomVectorInPlane(Plane plane, bool normalise = false, int seed = -1)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomVectorInPlane(plane, rnd, normalise);
        }

        /***************************************************/

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
