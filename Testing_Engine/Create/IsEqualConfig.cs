/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Testing;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Testing
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Config to be used to controll the IsEqual method.")]
        [Input("numericTolerance", "Tolerance used when compare doubles. Default value set to 1e-6")]
        [Input("ignoreGuid", "Toggles wheter to check the BHoM_Guid when comparing the objects.  Defaults to true => ignoring")]
        [Input("ignoreCustomData", "Toggels whether the custom data shouls be compared. Defaults to true => ignoring")]
        [Input("propertiesToIgnore", "names of any additional proerties to be ignored in the comparison. Case sensitive!")]
        public static IsEqualConfig IsEqualConfig(double numericTolerance = 1e-6, bool ignoreGuid = true, bool ignoreCustomData = true, List<string> propertiesToIgnore = null)
        {
            propertiesToIgnore = propertiesToIgnore ?? new List<string>();
            return new IsEqualConfig { NumericTolerance = numericTolerance, IgnoreGuid = ignoreGuid, IgnoreCustomData = ignoreCustomData, PropertiesToIgnore = propertiesToIgnore };
        }

        /***************************************************/
    }
}
