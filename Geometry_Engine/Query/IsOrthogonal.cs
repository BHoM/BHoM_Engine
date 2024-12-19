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
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****         Public Methods - Vectors          ****/
        /***************************************************/
        
        [Description("Queries whether a vector is orthogonal in relation to global X, Y or Z axis.")]
        [Input("vector", "The vector to evaluate.")]
        [Input("angleTolerance", "Optional, the angle discrepancy in radians from the global axis to consider as tolerance.")]
        [Output("isOrthogonal", "The boolean value of whether the vector is orthogonal or not.")]
        public static bool IsOrthogonal(this Vector vector, double angleTolerance = Tolerance.Angle)
        {
            if (vector == null || angleTolerance == null)
            {
                BH.Engine.Base.Compute.RecordError("One or more of the inputs is empty or null.");
                return false;
            }

            return (vector.IsParallel(Vector.XAxis, angleTolerance) != 0 || vector.IsParallel(Vector.YAxis, angleTolerance) != 0 || vector.IsParallel(Vector.ZAxis, angleTolerance) != 0);
        }

        /***************************************************/

    }
}




