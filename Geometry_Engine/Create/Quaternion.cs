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

        [Description("Creates a Quaternion based on its core properties.")]
        [InputFromProperty("x")]
        [InputFromProperty("y")]
        [InputFromProperty("z")]
        [InputFromProperty("w")]
        [Output("quaternion", "The created Quaternion.")]
        public static Quaternion Quaternion(double x, double y, double z, double w)
        {
            return new Quaternion { X = x, Y = y, Z = z, W = w };
        }

        /***************************************************/

        [Description("Creates a Quaternion based an vector axis and an angle. Used when computing the rotation matrix.")]
        [Input("axis", "Axis for the rotation Quaternion.")]
        [Input("angle", "Angle for the rotation Quaternion.", typeof(Angle))]
        [Output("quaternion", "The created Quaternion.")]
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

        [Description("Creates a random Quaternion with values between 0 and 1 based on a seed. If no seed is provided, a random one will be generated.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Output("quaternion", "The generated random Quaternion.")]
        public static Quaternion RandomQuaternion(int seed = -1)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomQuaternion(new Random(seed));
        }

        /***************************************************/

        [Description("Creates a random Quaternion with values between 0 and 1 using the provided Random class.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Output("quaternion", "The generated random Quaternion.")]
        public static Quaternion RandomQuaternion(Random rnd)
        {
            return new Quaternion { X = rnd.NextDouble(), Y = rnd.NextDouble(), Z = rnd.NextDouble(), W = rnd.NextDouble() }.Normalise();
        }

        /***************************************************/
    }
}






