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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the plane at a specific UV parameter location on a NurbsSurface.")]
        [Input("surface", "The NurbsSurface to evaluate.")]
        [Input("u", "The U parameter (0 to 1).")]
        [Input("v", "The V parameter (0 to 1).")]
        [Input("tolerance", "The tolerance for surface evaluation.", typeof(Length))]
        [Output("planeAtParameter", "The plane at the specified UV parameter location.")]
        public static Plane PlaneAtParameter(this NurbsSurface surface, double u, double v, double tolerance = Tolerance.Distance)
        {
            var vectors = TangentAtParameter(surface, u, v, tolerance);

            return Create.Plane(
                PointAtParameter(surface, u, v), 
                vectors.Item1.CrossProduct(vectors.Item2));
        }

        /***************************************************/

    }
}
