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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Ground;
using BH.Engine.Base;

namespace BH.Engine.Ground
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ContaminantSample object containing the chemical code, the depth, quantity and properties related to the sample.")]
        [Input("id", "Location identifier for the borehole unique to the project (LOCA_ID).")]
        [Input("top", "Depth to the top of the sample, measured from the top of the borehole (SAMP_TOP).")]
        [Input("chemical", "Chemical code for the contaminant (ERES_CODE).")]
        [Input("name", "The name of the chemical (ERES_NAME).")]
        [Input("result", "The amount of the chemical present (ERES_RVAL).")]
        [Input("type", "The type of sample (SAMP_TYPE).")]
        [Input("properties", "A list of different properties including references, tests, analysis, results and detection..")]
        [Output("contaminantSample", "The created ContaminantSample defined by its chemical code, depth and quantity based on the AGS schema.")]
        public static ContaminantSample ContaminantSample(string id, double top, string chemical, string name, double result, string type, List<IContaminantProperty> properties = null)
        {
            if (string.IsNullOrWhiteSpace(chemical))
            {
                Compute.RecordError("The chemical name is null or whitespace.");
                return null;
            }

            return new ContaminantSample() {Id = id, Top = top, Chemical = chemical, Name = name, Result = result, Type = type, ContaminantProperties = properties};

        }

        /***************************************************/
    }
}





