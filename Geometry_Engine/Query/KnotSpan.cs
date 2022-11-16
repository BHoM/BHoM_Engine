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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the span index in the knot vector corresponding to parameter t. Used to evaluate Basis Functions for Nurbs Curves/Surfaces.")]
        [Input("knots", "The knots to evaluate.")]
        [Input("degree", "Degree of the Curve/Surface in the direction of the provided knots.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("span", "The index of the span of the knotvector in which the parameter t resides.")]
        public static int KnotSpan(this IReadOnlyList<double> knots, int degree, double t)
        {
            int n = knots.Count - degree;

            if (t >= knots[n])  //End span
                return n - 1;

            //Binary serach
            int low = degree - 1;
            int high = n;
            int mid = (low + high) / 2;

            if (t < knots[low])
            {
                Base.Compute.RecordError("Invalid degree in relation to the knotvector and t provided. Cannot compute knot span.");
                return -1;
            }
            while (t < knots[mid] || t >= knots[mid + 1])
            {
                if (t < knots[mid])
                    high = mid;
                else
                    low = mid;

                mid = (low + high) / 2;
            }
            return mid;
        }

        /***************************************************/
    }
}
