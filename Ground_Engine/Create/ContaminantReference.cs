/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.Engine.Geometry;

namespace BH.Engine.Ground
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an object containing the reference properties for a contaminant sample.")]
        [Input("reference", "Reference for the contaminant sample (SAMP_REF).")]
        [Input("id", "Unique identifier for the contaminant sample (SAMP_ID).")]
        [Input("receiptDate", "Receipt date at the labratory for the contaminant sample (ERES_RDAT). If no value is assigned, the default value will be 1/1/0001 12:00:00 AM.")]
        [Input("batchCode", "Batch code for the contaminant sample (ERES_SGRP).")]
        [Input("files", "Associated file reference including instructions and photographs (FILE_FSET).")]
        [Output("contaminantReference", "Reference properties related to the contaminant sample.")]
        public static ContaminantReference ContaminantReference(string reference = "", string id = "", DateTime receiptDate = default(DateTime), string batchCode = "", string files = "")
        {
            return new ContaminantReference() { Reference = reference, Id = id, ReceiptDate = receiptDate, BatchCode = batchCode, Files = files };
        }

        /***************************************************/
    }
}




