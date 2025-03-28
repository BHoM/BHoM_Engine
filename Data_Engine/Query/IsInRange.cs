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

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.Threading.Tasks;
using BH.oM.Data.Collections;
using BH.oM.Geometry;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries if the two DomainBoxes are in range of each other.")]
        [Input("box1", "Box to evaluate intersection of.")]
        [Input("box2", "Box to evaluate intersection of.")]
        [Input("tolerance", "Numerical tolerance for the operation.")]
        [Output("true if the two Domainboxes are in range of each other.")]
        public static bool IsInRange(this DomainBox box1, DomainBox box2, double tolerance = Tolerance.Distance)
        {
            if (box1 == null || box2 == null)
                return false; //Cannot be in range if either is null

            return SquareDistance(box1, box2) < (tolerance * tolerance);
        }

        /***************************************************/

        [Description("Queries if the two Domains are in range of each other.")]
        [Input("domain1", "Domain to evaluate overlap with.")]
        [Input("domain2", "Domain to evaluate overlap with.")]
        [Input("tolerance", "Numerical tolerance for the operation.")]
        [Output("true if the two Domains are in range of each other.")]
        public static bool IsInRange(this Domain domain1, Domain domain2, double tolerance = Tolerance.Distance)
        {
            if (domain1 == null || domain2 == null)
                return false; //Cannot be in range if either is null

            return Distance(domain1, domain2) < tolerance;
        }

        /***************************************************/

        [Description("Queries if the value is in range of the domain.")]
        [Input("domain", "Domain to evaluate overlap with.")]
        [Input("val", "Value to query overlap with the domain.")]
        [Input("tolerance", "Numerical tolerance for the operation.")]
        [Output("true if the value is in the domain.")]
        public static bool IsInRange(this Domain domain, double val, double tolerance = Tolerance.Distance)
        {
            if (domain == null)
                return false;

            return val > domain.Min && val < domain.Max;
        }

        /***************************************************/

    }
}





