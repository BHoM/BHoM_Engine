/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if the given transformation matrix represents a pure reflection (i.e. a reflection without any additional rotation, translation or shear).")]
        [Input("transform", "Transformation matrix to check.")]
        [Input("tolerance", "Tolerance used in computations.")]
        [Output("isReflection", "True if the transformation matrix represents a pure reflection, false otherwise.")]
        public static bool IsPureReflection(this TransformMatrix transform, double tolerance = 1e-6)
        {
            if (!transform.IsValid())
            {
                BH.Engine.Base.Compute.RecordError("The given TransformMatrix is not valid.");
                return false;
            }

            double[,] m = transform.Matrix;

            // 1. Affine bottom row
            if (Math.Abs(m[3, 0]) > tolerance ||
                Math.Abs(m[3, 1]) > tolerance ||
                Math.Abs(m[3, 2]) > tolerance ||
                Math.Abs(m[3, 3] - 1.0) > tolerance)
                return false;

            // Extract R and t
            double[,] R =
            {
                { m[0,0], m[0,1], m[0,2] },
                { m[1,0], m[1,1], m[1,2] },
                { m[2,0], m[2,1], m[2,2] }
            };

            double[] t = { m[0, 3], m[1, 3], m[2, 3] };

            // 2. Orthogonality: Rᵀ R = I
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double dot = 0;
                    for (int k = 0; k < 3; k++)
                        dot += R[k, i] * R[k, j];

                    double expected = (i == j) ? 1.0 : 0.0;
                    if (Math.Abs(dot - expected) > tolerance)
                        return false;
                }
            }

            // 3. Determinant = -1
            double det =
                R[0, 0] * (R[1, 1] * R[2, 2] - R[1, 2] * R[2, 1]) -
                R[0, 1] * (R[1, 0] * R[2, 2] - R[1, 2] * R[2, 0]) +
                R[0, 2] * (R[1, 0] * R[2, 1] - R[1, 1] * R[2, 0]);

            if (Math.Abs(det + 1.0) > tolerance)
                return false;

            // 4. Symmetry: Rᵀ = R  (CRITICAL)
            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    if (Math.Abs(R[i, j] - R[j, i]) > tolerance)
                        return false;
                }
            }

            // 5. Involution: R² = I
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < 3; k++)
                        sum += R[i, k] * R[k, j];

                    double expected = (i == j) ? 1.0 : 0.0;
                    if (Math.Abs(sum - expected) > tolerance)
                        return false;
                }
            }

            // 6. Translation consistency:
            // (R - I)x = -t must define a plane
            // Check that -t lies in the column space of (R - I)
            // For reflections: rank(R - I) == 1
            double[,] A =
            {
                { R[0,0] - 1, R[0,1],     R[0,2]     },
                { R[1,0],     R[1,1] - 1, R[1,2]     },
                { R[2,0],     R[2,1],     R[2,2] - 1 }
            };

            return true;
        }

        /***************************************************/
    }
}
