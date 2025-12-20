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

using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("Checks if a NurbsCurve is periodic (closed with continuous derivatives).")]
        [Input("curve", "The NurbsCurve to check for periodicity.")]
        [Output("isPeriodic", "True if the curve is periodic, false otherwise.")]
        public static bool IsPeriodic(this NurbsCurve curve)
        {
            int multiplicity = 1;

            while (curve.Knots[multiplicity - 1] == curve.Knots[multiplicity])
                multiplicity++;

            return (multiplicity != curve.Degree());
        }

        /***************************************************/
        
        [Description("Checks if a NurbsSurface is periodic in either U or V direction.")]
        [Input("surface", "The NurbsSurface to check for periodicity.")]
        [Output("isPeriodic", "True if the surface is periodic in either direction, false otherwise.")]
        public static bool IsPeriodic(this NurbsSurface surface)
        {
            int uMultiplicity = 1;
            int vMultiplicity = 1;

            while (surface.UKnots[uMultiplicity - 1] == surface.UKnots[uMultiplicity])
                uMultiplicity++;
            while (surface.VKnots[vMultiplicity - 1] == surface.VKnots[vMultiplicity])
                vMultiplicity++;

            return (uMultiplicity != surface.UDegree || vMultiplicity != surface.VDegree);
        }

        /***************************************************/
    }
}
