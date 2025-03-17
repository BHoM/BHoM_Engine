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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks whether the specified point is symmetric about the given plane against a list of provided points.")]
        [Input("pts", "The list of points to check the symmetry against.")]
        [Input("pt", "The point to mirror about the given plane.")]
        [Input("p", "The plane to check symmetry about.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("b", "True if the point is symmetric within tolerance, false if not.")]
        public static bool Symmetric(List<Point> pts, Point pt, Plane p, double tolerance = Tolerance.Distance)
        {
            if (pts.IsNullOrEmpty() || pt.IsNull() || p.IsNull())
                return false;

            Point mirror = pt.Mirror(p);
            Point closest = ClosestPoint(pts, mirror);
            if (Distance(mirror, closest) > tolerance)
                return false;
            else
                return true;
        }

        /***************************************************/
    }
}




