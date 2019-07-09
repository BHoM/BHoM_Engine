/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        public static int Degree(this NurbsCurve curve)
        {
            return curve.Knots.Count - curve.ControlPoints.Count + 1;
        }

        /***************************************************/

        public static List<int> Degrees(this NurbsSurface surf)
        {
            int uDegree = 1;
            int vDegree = 1;
            while (surf.UKnots[uDegree - 1] == surf.UKnots[uDegree])
                uDegree++;
            while (surf.VKnots[vDegree - 1] == surf.VKnots[vDegree])
                vDegree++;
            return new List<int>() { uDegree, vDegree };
        }

        /***************************************************/
    }
}
