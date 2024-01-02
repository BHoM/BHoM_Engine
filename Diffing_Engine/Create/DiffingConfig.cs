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
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Diffing
{
    public static partial class Create
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/
        ///
        [Description("Defines configurations for the diffing.")]
        [Input("enablePropertyDiffing", "Enables the property-level diffing: differences in object properties are stored in the `ModifiedPropsPerObject` dictionary.")]
        [Input("storeUnchangedObjects", "If enabled, the Diff stores also the objects that did not change (`Unchanged` property).")]
        public static DiffingConfig DiffingConfig(bool enablePropertyDiffing, bool storeUnchangedObjects)
        {
            return Create.DiffingConfig(enablePropertyDiffing, storeUnchangedObjects, null);
        }

        [Description("Defines configurations for the diffing.")]
        [Input("enablePropertyDiffing", "Enables the property-level diffing: differences in object properties are stored in the `ModifiedPropsPerObject` dictionary.")]
        [Input("storeUnchangedObjects", "If enabled, the Diff stores also the objects that did not change (`Unchanged` property).")]
        public static DiffingConfig DiffingConfig(bool enablePropertyDiffing = false, bool storeUnchangedObjects = true, List<string> propertyNamesToConsider = null)
        {
            return new DiffingConfig()
            {
                EnablePropertyDiffing = enablePropertyDiffing,
                IncludeUnchangedObjects = storeUnchangedObjects,
                ComparisonConfig = new ComparisonConfig()
                {
                    PropertiesToConsider = propertyNamesToConsider,
                },
            };
        }
    }
}





