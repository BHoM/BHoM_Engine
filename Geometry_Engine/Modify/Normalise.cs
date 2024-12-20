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

using BH.Engine.Base;
using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Normalise(this Vector vector)
        {
            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;
            double d = Math.Sqrt(x * x + y * y + z * z);

            if (d == 0)
                return vector;

            return new Vector { X = x / d, Y = y / d, Z = z / d };
        }

        /***************************************************/

        public static Quaternion Normalise(this Quaternion q)
        {
            double x = q.X;
            double y = q.Y;
            double z = q.Z;
            double w = q.W;
            double d = Math.Sqrt(x * x + y * y + z * z + w * w);

            return new Quaternion { X = x / d, Y = y / d, Z = z / d, W = w / d };
        }

        /***************************************************/
    }
}






