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

        [Description("Creates an object containing the analysis properties for a contaminant sample.")]
        [Input("totalOrDissolved", "Whether the specimen is total or dissolved (ERES_TORD).")]
        [Input("accreditingBody", "Accrediting body and reference number (when appropriate) (ERES_CRED).")]
        [Input("labName", "Name of testing labratory or organisation (ERES_LAB).")]
        [Input("percentageRemoved", "Percentage of material removed (ERES_PERP). If no value is assigned, the default value will be 0.")]
        [Input("sizeRemoved", "Size of material removed prior to test. Value represents lowest sized material removed (ERES_SIZE). If no value is assigned, the default value will be 0.")]
        [Input("instrumentReference", "Instrument reference number or identifier (ERES_IREF).")]
        [Input("leachateDate", "Leachate preperation date and time (ERES_LDTM). If no value is assigned, the default value will be 1/1/0001 12:00:00 AM.")]
        [Input("leachateMethod", "Leachate preperation method (ERES_LMTH).")]
        [Input("diluationFactor", "Dilution factor (ERES_DIL). If no value is assigned, the default value will be 0.")]
        [Input("basis", "Basis (ERES_BAS).")]
        [Input("location", "Analysis location (ERES_LOCN).")]
        [Output("analysisProperties", "Properties related to the analysis undertaken on the contaminant.")]
        public static AnalysisProperties AnalysisProperties(string totalOrDissolved = "", string accreditingBody = "", string labName = "", 
            double percentageRemoved = 0, double sizeRemoved = 0, string instrumentReference = "", DateTime leachateDate = default(DateTime), string leachateMethod = "",
            int diluationFactor = 0, string basis = "", string location = "")
        {
            if(totalOrDissolved.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The remarks input is null or empty.");
                return null;
            }

            return new AnalysisProperties() { TotalOrDissolved = totalOrDissolved, AccreditingBody = accreditingBody, LabName = labName, PercentageRemoved =percentageRemoved,
            SizeRemoved = sizeRemoved, InstrumentReference = instrumentReference, LeachateDate = leachateDate, LeachateMethod = leachateMethod, DilutionFactor = diluationFactor, 
            Basis = basis, Location = location};
        }

        /***************************************************/
    }
}




