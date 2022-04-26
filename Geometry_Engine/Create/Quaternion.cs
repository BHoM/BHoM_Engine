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

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Quaternion Quaternion(double x, double y, double z, double w)
        {
            return new Quaternion { X = x, Y = y, Z = z, W = w };
        }

        /***************************************************/

        public static Quaternion Quaternion(Vector axis, double angle)
        {
            double sin = Math.Sin(angle / 2);
            return new Quaternion
            {
                X = axis.X * sin,
                Y = axis.Y * sin,
                Z = axis.Z * sin,
                W = Math.Cos(angle / 2)
            }.Normalise();
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Quaternion RandomQuaternion(int seed = -1)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomQuaternion(new Random(seed));
        }

        /***************************************************/

        public static Quaternion RandomQuaternion(Random rnd)
        {
            return new Quaternion { X = rnd.NextDouble(), Y = rnd.NextDouble(), Z = rnd.NextDouble(), W = rnd.NextDouble() }.Normalise();
        }

        /***************************************************/
    }
}



