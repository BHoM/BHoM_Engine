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
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts the parameter t from a normalised domain assumed to be between 0 and 1 to the domain of the provided knot vector.")]
        [Input("t", "The normalised parameter to convert. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("knots", "Knot vector to extract domain from.")]
        [Input("degree", "Degree of the curve/surface to which the knot vector belong in the direction of the knot vector.")]
        [Output("t", "Parameter in the domain of the knot vector.")]
        public static double ToKnotDomain(double t, IReadOnlyList<double> knots, int degree)
        {
            if (knots == null)
                return 0;

            t = t < 0 ? 0 : t > 1 ? 1 : t;

            double min = knots[degree - 1];
            double max = knots[knots.Count - degree];
            return min + (max - min) * t;
        }

        /***************************************************/
    }
}
