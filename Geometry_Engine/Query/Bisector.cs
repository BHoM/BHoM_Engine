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

        [Description("Returns the unit bisector of two vectors.")]
        [Input("v1", "First vector.")]
        [Input("v2", "Second vector.")]
        [Output("bisector", "Normalised vector along the half-angle between v1 and v2.")]
        public static Vector Bisector(this Vector v1, Vector v2)
        {
            Vector normalizedV1 = v1.Normalise();
            Vector normalizedV2 = v2.Normalise();

            float angle = (float)Math.Acos(normalizedV1.DotProduct(normalizedV2));
            float halfAngle = angle / 2.0f;
            Vector axis = normalizedV1.CrossProduct(normalizedV2);
            TransformMatrix rotation = Create.RotationMatrix(Point.Origin, axis, halfAngle);

            return normalizedV1.Transform(rotation).Normalise();
        }

        /***************************************************/
    }
}
