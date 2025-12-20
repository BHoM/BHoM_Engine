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
using BH.oM.Quantities.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Get the smallest possible angle between 2 vectors regardless of their directions.")]
        [Input("vector1", "The first vector.")]
        [Input("vector2", "The second vector.")]
        [Output("angle", "The smallest possible angle between 2 vectors regardless of their directions.", typeof(Angle))]
        public static double AcuteAngle(this Vector vector1, Vector vector2)
        {
            double length1 = vector1.Length();
            double length2 = vector2.Length();

            // Handle zero-length vectors
            if (length1 < Tolerance.Distance || length2 < Tolerance.Distance)
            {
                Engine.Base.Compute.RecordWarning("One or both vectors have zero or near-zero length. Cannot compute angle.");
                return double.NaN;
            }

            double dotProduct = vector1.DotProduct(vector2);
            double cosAngle = Math.Abs(dotProduct) / (length1 * length2);

            //When vectors are almost identical, cosAngle can slightly exceed 1 due to floating-point precision, causing Math.Acos() to return NaN. Here we clamp cosAngle to valid range [-1, 1] to prevent this.
            cosAngle = Math.Max(-1.0, Math.Min(1.0, cosAngle));

            return Math.Acos(cosAngle);
        }

        /***************************************************/
    }
}

