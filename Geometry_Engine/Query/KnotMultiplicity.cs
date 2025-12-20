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
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Determines the multiplicity of a knot value in a knot vector, i.e., how many times the value appears consecutively.")]
        [Input("knots", "The knot vector to search in.")]
        [Input("t", "The knot value to find the multiplicity of.")]
        [Output("multiplicity", "The number of times the knot value appears in the knot vector.")]
        public static int KnotMultiplicity(this IList<double> knots, double t)
        {
            double tol = double.Epsilon;
            int multiplicity = 0;

            for (int i = 0; i < knots.Count; i++)
            {
                if (Math.Abs(knots[i] - t) < tol)
                    multiplicity++;
            }

            return multiplicity;
        }

        /***************************************************/
    }
}

