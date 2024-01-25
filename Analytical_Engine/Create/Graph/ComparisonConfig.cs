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

using BH.oM.Base;
using BH.oM.Diffing;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Create
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Create a simple ComparisonConfig.")]
        [Input("numericTolerance", "Tolerance used to determine numerical differences." +
            "\nDefaults to Tolerance.Distance (1e-6).")]
        [Input("propertyNamesToConsider", "By default, all the properties of the objects are considered in determining uniqueness." +
            "\nHere you can specify a list of property names. Only the properties with a name matching any of this list will be considered." +
            "\nWorks only for top-level properties." +
            "\nE.g., if you input 'Name' only the differences in terms of name will be returned.")]
        public static ComparisonConfig ComparisonConfig(double numericTolerance = oM.Geometry.Tolerance.Distance, HashSet<string> propertyNamesToConsider = null)
        {
            ComparisonConfig cc = new ComparisonConfig()
            {
                NumericTolerance = numericTolerance,
                PropertiesToConsider = propertyNamesToConsider ?? new List<string>(),
                PropertiesToConsider = propertyNamesToConsider ?? new HashSet<string>(),
            };

            return cc;
        }
    }
}




