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
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the domain of a NurbsCurve.")]
        [Input("curve", "The NurbsCurve to get the domain from.")]
        [Output("domain", "The domain of the NURBS curve as an array [min, max].")]
        public static double[] Domain(this NurbsCurve curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't compute domain of a null curve.");
                return null;
            }

            return curve.Knots.Domain(curve.Degree());
        }

        /***************************************************/

        [Description("Gets the U domain of a NurbsSurface.")]
        [Input("surface", "The NurbsSurface to get the U domain from.")]
        [Output("domain", "The U domain of the NURBS surface as an array [min, max].")]
        public static double[] UDomain(this NurbsSurface surface)
        {
            if (surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't compute domain of a null surface.");
                return null;
            }

            return surface.UKnots.Domain(surface.UDegree);
        }

        /***************************************************/

        [Description("Gets the V domain of a NurbsSurface.")]
        [Input("surface", "The NurbsSurface to get the V domain from.")]
        [Output("domain", "The V domain of the NURBS surface as an array [min, max].")]
        public static double[] VDomain(this NurbsSurface surface)
        {
            if (surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't compute domain of a null surface.");
                return null;
            }

            return surface.VKnots.Domain(surface.VDegree);
        }

        /***************************************************/

        [Description("Gets the domain from a knot vector and degree.")]
        [Input("knots", "The knot vector.")]
        [Input("degree", "The degree of the curve or surface.")]
        [Output("domain", "The domain as an array [min, max].")]
        public static double[] Domain(this IList<double> knots, int degree)
        {
            return new double[] { knots[degree - 1], knots[knots.Count - degree] };
        }

        /***************************************************/
    }
}

