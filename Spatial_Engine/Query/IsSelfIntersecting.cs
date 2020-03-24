/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Returns if the one dimensional representation of the IElement1D is closer to itself than the tolerance at any two points.")]
        [Input("element1D", "The IElement1D to evaluate self intersections from.")]
        [Input("tolerance", "Minimum distance to be considered intersecting.", typeof(Length))]
        [Output("o", "A boolean which is true if the IElement1Ds curve is self intersecting.")]
        public static bool IsSelfIntersecting(this IElement1D element1D, double tolerance = Tolerance.Distance)
        {
            return Geometry.Query.IIsSelfIntersecting(element1D.IGeometry(), tolerance);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Returns if any of the element curves of the IElement2D is closer to itself than the tolerance at any two points. Does not check for intersections between external and internal curves, or between different internal curves.")]
        [Input("element2D", "The IElement2D which curves are to be evaluated for self intersection.")]
        [Input("tolerance", "Minimum distance to be considered intersecting.", typeof(Length))]
        [Output("o", "A boolean which is true if any of the IElement2Ds element curves are self intersecting.")]
        public static bool IsSelfIntersecting(this IElement2D element2D, double tolerance = Tolerance.Distance)
        {
            if (Geometry.Query.IIsSelfIntersecting(element2D.OutlineCurve(), tolerance))
                return true;

            foreach (PolyCurve internalOutline in element2D.InternalOutlineCurves())
            {
                if (Geometry.Query.IIsSelfIntersecting(internalOutline, tolerance))
                    return true;
            }

            return false;
        }

        /******************************************/
    }
}
