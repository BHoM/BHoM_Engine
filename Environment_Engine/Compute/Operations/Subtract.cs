/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Environment.Results;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Adds a number to each element in an IAnalysisResult")]
        [Input("analysisResult", "An IAnalysisResult object containing results")]
        [Input("number", "A number to subtract from each of the elements in the IAnalysisResult.Result")]
        [Output("analysisResult", "A modified IAnalysisResult")]
        public static IAnalysisResult Subtract(this IAnalysisResult analysisResult, double number)
        {
            IAnalysisResult modifiedAnalysisResult = analysisResult.DeepClone();
            List<double> modifiedValues = new List<double>();
            foreach (double x in analysisResult.Result)
            {
                modifiedValues.Add(x - number);
            }
            modifiedAnalysisResult.Result = modifiedValues;
            return modifiedAnalysisResult;
        }

        [Description("Subtract on IAnalysisResult object from another")]
        [Input("analysisResult1", "An IAnalysisResult object containing results")]
        [Input("analysisResult2", "An IAnalysisResult object containing results")]
        [Output("analysisResult", "A modified IAnalysisResult")]
        public static IAnalysisResult Subtract(this IAnalysisResult analysisResult1, IAnalysisResult analysisResult2)
        {
            IAnalysisResult modifiedAnalysisResult = analysisResult1.DeepClone();
            int resultCount = modifiedAnalysisResult.AnalysisResultLength();

            if (analysisResult1.AnalysisResultLength() != analysisResult2.AnalysisResultLength())
            {
                BH.Engine.Reflection.Compute.RecordError("Analysis result lengths do not match");
                return null;
            }

            for (int i = 0; i < resultCount; i++)
            {
                modifiedAnalysisResult.Result[i] -= analysisResult2.Result[i];
            }

            return modifiedAnalysisResult;
        }
    }
}


