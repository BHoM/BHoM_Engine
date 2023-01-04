/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsRigidTransformation(this TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsValid())
            {
                BH.Engine.Base.Compute.RecordError("The given TransformMatrix is not valid.");
                return false;
            }

            double[,] rotation = new double[3, 3];
            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    rotation[m, n] = transform.Matrix[m, n];
                }
            }

            double[,] transposed = rotation.Transpose();

            double[,] multiplied = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        multiplied[i, j] += rotation[i, k] * transposed[k, j];
                    }
                }
            }

            double[,] identity = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            };

            return multiplied.IsEqual(identity, tolerance) && Math.Abs(1 - rotation.Determinant()) <= tolerance;
        }

        /***************************************************/
    }
}




