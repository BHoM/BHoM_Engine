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
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Cartesian CoordinateSystem. x and y will be unitised. If x and why are non-orthogonal, y will be made orthogonal to x, while x will be kept.")]
        public static Cartesian CartesianCoordinateSystem(Point origin, Vector x, Vector y)
        {

            x = x.Normalise();
            y = y.Normalise();

            double dot = x.DotProduct(y);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
                throw new ArgumentException("Can not create coordinate system from parallell vectors");

            Vector z = x.CrossProduct(y);
            z.Normalise();

            if (Math.Abs(dot) > Tolerance.Angle)
            {
                Base.Compute.RecordWarning("x and y are not orthogonal. y will be made othogonal to x and z");
                y = z.CrossProduct(x).Normalise();
            }

            return new Cartesian(origin, x, y, z);
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Cartesian based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("cartesian", "The generated random Cartesian.")]
        public static Cartesian RandomCartesianCoordinateSystem(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomCartesianCoordinateSystem(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Cartesian using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("cartesian", "The generated random Cartesian.")]
        public static Cartesian RandomCartesianCoordinateSystem(Random rnd, BoundingBox box = null)
        {
            Vector x = RandomVector(rnd, box);
            Vector y = RandomVector(rnd, box);

            if (x.IsParallel(y) != 0)
                return RandomCartesianCoordinateSystem(rnd, box);

            Point orgin = RandomPoint(rnd, box);

            return CartesianCoordinateSystem(orgin, x, y);
        }

        /***************************************************/
    }
}



