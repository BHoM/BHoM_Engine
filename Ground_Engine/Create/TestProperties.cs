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

        [Description("Creates an object containing the test properties for a contaminant sample.")]
        [Input("testName", "Test name as defined in LBST_TEST during electrical scheduling (ERES_TEST).")]
        [Input("labTestName", "Labratory test name for the contaminant sample (ERES_TNAM).")]
        [Input("reference", "Test reference (TEST_TESN).")]
        [Input("runType", "Run type description, i.e. initial or reanalysis (ERES_RTYP).")]
        [Input("testMatrix", "Labratory test matrix (ERES_MATX).")]
        [Input("method", "Test method (ERES_METH).")]
        [Input("analysisDate", "Analysis time and date for the contaminant sample (ERES_DTIM).")]
        [Input("description", "Description of the specimen from the contaminant sample(SPEC_DESC).")]
        [Input("remarks", "Remarks about the test or specimen from the contaminant sample (ERES_REM).")]
        [Input("testStatus", "The status of the test (TEST_STAT).")]
        [Output("testProperties", "Test properties related to the contaminant sample.")]
        public static TestProperties TestProperties(string testName, string labTestName, string reference, string runType, string testMatrix, string method, DateTime analysisDate,
            string description, string remarks, string testStatus)
        {
            if (testName.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The type input is null or empty.");
                return null;
            }

            return new TestProperties() { Name = testName, LabTestName = labTestName, Reference = reference, RunType = runType, TestMatrix = testMatrix, Method = method, AnalysisDate = analysisDate,
            Description = description, Remarks = remarks, TestStatus = testStatus};
        }

        /***************************************************/
    }
}




