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

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Checks whether a number is inside a given domain (range).")]
        [Input("number", "Number to check for domain containment.")]
        [Input("domain", "Domain (range) to check the number against.")]
        [Input("tolerance", "Tolerance to apply in the check.")]
        [Output("inDomain", "True if the input number is inside the domain, otherwise false.")]
        public static bool IsInDomain(this double number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            return (domain.Min == double.MinValue || number >= domain.Min - tolerance)
                && (domain.Max == double.MaxValue || number <= domain.Max + tolerance);
        }

        /***************************************************/

        [Description("Checks whether a number is inside a given domain (range).")]
        [Input("number", "Number to check for domain containment.")]
        [Input("domain", "Domain (range) to check the number against.")]
        [Input("tolerance", "Tolerance to apply in the check.")]
        [Output("inDomain", "True if the input number is inside the domain, otherwise false.")]
        public static bool IsInDomain(this long number, BH.oM.Data.Collections.Domain domain, double tolerance)
        {
            return (domain.Min == double.MinValue || number >= domain.Min - tolerance)
                && (domain.Max == double.MaxValue || number <= domain.Max + tolerance);
        }

        /***************************************************/
    }
}

