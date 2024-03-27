/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TransformMatrix Invert(this TransformMatrix transform)
        {
            double[,] m = transform.Matrix;
            double[,] mNew = new double[4, 4] {
                {
                    m[1,2] * m[2,3] * m[3,1] - m[1,3] * m[2,2] * m[3,1] + m[1,3] * m[2,1] * m[3,2] - m[1,1] * m[2,3] * m[3,2] - m[1,2] * m[2,1] * m[3,3] + m[1,1] * m[2,2] * m[3,3],
                    m[0,3] * m[2,2] * m[3,1] - m[0,2] * m[2,3] * m[3,1] - m[0,3] * m[2,1] * m[3,2] + m[0,1] * m[2,3] * m[3,2] + m[0,2] * m[2,1] * m[3,3] - m[0,1] * m[2,2] * m[3,3],
                    m[0,2] * m[1,3] * m[3,1] - m[0,3] * m[1,2] * m[3,1] + m[0,3] * m[1,1] * m[3,2] - m[0,1] * m[1,3] * m[3,2] - m[0,2] * m[1,1] * m[3,3] + m[0,1] * m[1,2] * m[3,3],
                    m[0,3] * m[1,2] * m[2,1] - m[0,2] * m[1,3] * m[2,1] - m[0,3] * m[1,1] * m[2,2] + m[0,1] * m[1,3] * m[2,2] + m[0,2] * m[1,1] * m[2,3] - m[0,1] * m[1,2] * m[2,3]
                },
                {
                    m[1,3] * m[2,2] * m[3,0] - m[1,2] * m[2,3] * m[3,0] - m[1,3] * m[2,0] * m[3,2] + m[1,0] * m[2,3] * m[3,2] + m[1,2] * m[2,0] * m[3,3] - m[1,0] * m[2,2] * m[3,3],
                    m[0,2] * m[2,3] * m[3,0] - m[0,3] * m[2,2] * m[3,0] + m[0,3] * m[2,0] * m[3,2] - m[0,0] * m[2,3] * m[3,2] - m[0,2] * m[2,0] * m[3,3] + m[0,0] * m[2,2] * m[3,3],
                    m[0,3] * m[1,2] * m[3,0] - m[0,2] * m[1,3] * m[3,0] - m[0,3] * m[1,0] * m[3,2] + m[0,0] * m[1,3] * m[3,2] + m[0,2] * m[1,0] * m[3,3] - m[0,0] * m[1,2] * m[3,3],
                    m[0,2] * m[1,3] * m[2,0] - m[0,3] * m[1,2] * m[2,0] + m[0,3] * m[1,0] * m[2,2] - m[0,0] * m[1,3] * m[2,2] - m[0,2] * m[1,0] * m[2,3] + m[0,0] * m[1,2] * m[2,3]
                },
                {
                    m[1,1] * m[2,3] * m[3,0] - m[1,3] * m[2,1] * m[3,0] + m[1,3] * m[2,0] * m[3,1] - m[1,0] * m[2,3] * m[3,1] - m[1,1] * m[2,0] * m[3,3] + m[1,0] * m[2,1] * m[3,3],
                    m[0,3] * m[2,1] * m[3,0] - m[0,1] * m[2,3] * m[3,0] - m[0,3] * m[2,0] * m[3,1] + m[0,0] * m[2,3] * m[3,1] + m[0,1] * m[2,0] * m[3,3] - m[0,0] * m[2,1] * m[3,3],
                    m[0,1] * m[1,3] * m[3,0] - m[0,3] * m[1,1] * m[3,0] + m[0,3] * m[1,0] * m[3,1] - m[0,0] * m[1,3] * m[3,1] - m[0,1] * m[1,0] * m[3,3] + m[0,0] * m[1,1] * m[3,3],
                    m[0,3] * m[1,1] * m[2,0] - m[0,1] * m[1,3] * m[2,0] - m[0,3] * m[1,0] * m[2,1] + m[0,0] * m[1,3] * m[2,1] + m[0,1] * m[1,0] * m[2,3] - m[0,0] * m[1,1] * m[2,3]
                },
                {
                    m[1,2] * m[2,1] * m[3,0] - m[1,1] * m[2,2] * m[3,0] - m[1,2] * m[2,0] * m[3,1] + m[1,0] * m[2,2] * m[3,1] + m[1,1] * m[2,0] * m[3,2] - m[1,0] * m[2,1] * m[3,2],
                    m[0,1] * m[2,2] * m[3,0] - m[0,2] * m[2,1] * m[3,0] + m[0,2] * m[2,0] * m[3,1] - m[0,0] * m[2,2] * m[3,1] - m[0,1] * m[2,0] * m[3,2] + m[0,0] * m[2,1] * m[3,2],
                    m[0,2] * m[1,1] * m[3,0] - m[0,1] * m[1,2] * m[3,0] - m[0,2] * m[1,0] * m[3,1] + m[0,0] * m[1,2] * m[3,1] + m[0,1] * m[1,0] * m[3,2] - m[0,0] * m[1,1] * m[3,2],
                    m[0,1] * m[1,2] * m[2,0] - m[0,2] * m[1,1] * m[2,0] + m[0,2] * m[1,0] * m[2,1] - m[0,0] * m[1,2] * m[2,1] - m[0,1] * m[1,0] * m[2,2] + m[0,0] * m[1,1] * m[2,2]
                }
            };
            return new TransformMatrix { Matrix = mNew } / transform.Determinant();
        }

        /***************************************************/
    }
}





