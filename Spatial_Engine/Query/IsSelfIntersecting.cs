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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Checks if the one dimensional representation of the IElement0D is closer to itself than the tolerance at any two points. Always false because a zero-dimensional IElement0D does not consist of curves.")]
        [Input("element0D", "The IElement0D to evaluate self intersections from.")]
        [Input("tolerance", "Minimum distance to be considered intersecting.", typeof(Length))]
        [Output("A boolean which is true if the geometrical representation of an IElement0D is self intersecting.")]
        public static bool IsSelfIntersecting(this IElement0D element0D, double tolerance = Tolerance.Distance)
        {
            return false;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Checks if the one dimensional representation of the IElement1D is closer to itself than the tolerance at any two points.")]
        [Input("element1D", "The IElement1D to evaluate self intersections from.")]
        [Input("tolerance", "Minimum distance to be considered intersecting.", typeof(Length))]
        [Output("A boolean which is true if the IElement1Ds curve is self intersecting.")]
        public static bool IsSelfIntersecting(this IElement1D element1D, double tolerance = Tolerance.Distance)
        {
            return Engine.Geometry.Query.IIsSelfIntersecting(element1D.IGeometry(), tolerance);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Checks if any of the element curves of the IElement2D is closer to itself than the tolerance at any two points. Does not check for intersections between external and internal curves, or between different internal curves.")]
        [Input("element2D", "The IElement2D which curves are to be evaluated for self intersection.")]
        [Input("tolerance", "Minimum distance to be considered intersecting.", typeof(Length))]
        [Output("A boolean which is true if any of the IElement2Ds element curves are self intersecting.")]
        public static bool IsSelfIntersecting(this IElement2D element2D, double tolerance = Tolerance.Distance)
        {
            if (Engine.Geometry.Query.IIsSelfIntersecting(element2D.OutlineCurve(), tolerance))
                return true;

            foreach (PolyCurve internalOutline in element2D.InternalOutlineCurves())
            {
                if (Engine.Geometry.Query.IIsSelfIntersecting(internalOutline, tolerance))
                    return true;
            }

            return false;
        }


        /******************************************/
        /****   Public Methods - Interfaces    ****/
        /******************************************/

        [Description("Checks if any of the curves defining an IElement is closer to itself than the tolerance at any two points (is self intersecting). In case of IElement2D, does not check for intersections between external and internal curves, or between different internal curves.")]
        [Input("element", "The IElement to evaluate self intersections from.")]
        [Input("tolerance", "Minimum distance to be considered intersecting.", typeof(Length))]
        [Output("A boolean which is true if any of the IElement's curves is self intersecting.")]
        public static bool IIsSelfIntersecting(this IElement element, double tolerance = Tolerance.Distance)
        {
            return IsSelfIntersecting(element as dynamic, tolerance);
        }

        /******************************************/
    }
}




