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

using System.Collections.Generic;
using System.ComponentModel;

using BH.oM.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        [Description("Map a Fibonacci lattice onto the surface of a sphere and return the vectors to each equidistant sample point")]
        [Input("nSamples", "Number of samples")]
        [MultiOutput(0, "theta", "Polar angle of each point (θ)")]
        [MultiOutput(1, "phi", "Azimuth angle for each point (ρ)")]
        [MultiOutput(2, "vectors", "Vectors to sample points")]
        public static Output<List<double>, List<double>, List<Vector>> FibonacciSphere(int nSamples)
        {
            if (nSamples <= 0)
            {
                BH.Engine.Base.Compute.RecordWarning("nSamples must be greater than 0 in order to generate sample vectors.");
            }

            List<double> indices = new List<double>();
            for (int i = 0; i < nSamples; i++)
            {
                indices.Add(i + 0.5);
            }

            List<double> phi = new List<double>();
            List<double> theta = new List<double>();
            foreach (double i in indices)
            {
                phi.Add(System.Math.Acos(1 - 2 * i / nSamples));
                theta.Add(System.Math.PI * (1 + System.Math.Pow(5, 0.5)) * i);
            }

            List<Vector> cartesianCoordinates = new List<Vector>();
            for (int i = 0; i < nSamples; i++)
            {
                cartesianCoordinates.Add(
                    BH.Engine.Geometry.Create.Vector(
                        x: System.Math.Cos(theta[i]) * System.Math.Sin(phi[i]),
                        y: System.Math.Sin(theta[i]) * System.Math.Sin(phi[i]),
                        z: System.Math.Cos(phi[i])
                        )
                    );
            }

            return new Output<List<double>, List<double>, List<Vector>>
            {
                Item1 = theta,
                Item2 = phi,
                Item3 = cartesianCoordinates
            };
        }
    }
}




