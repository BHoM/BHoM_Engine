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

        [Description("Perform an element-wise sum for all IAnalysisResult objects in a list")]
        [Input("analysisResults", "A list of IAnalysisResult objects containing results")]
        [Output("analysisResult", "A modified IAnalysisResult")]
        public static IAnalysisResult Sum(this List<IAnalysisResult> analysisResults)
        {
            int resultCount = analysisResults[0].AnalysisResultLength();
            
            foreach (IAnalysisResult ar in analysisResults)
            {
                if (ar.AnalysisResultLength() != resultCount)
                {
                    BH.Engine.Reflection.Compute.RecordError("Analysis result lengths do not match");
                    return null;
                }
            }

            IAnalysisResult modifiedAnalysisResult = analysisResults[0].DeepClone();
            foreach (IAnalysisResult ar in analysisResults.Skip(1))
            {
                modifiedAnalysisResult = modifiedAnalysisResult.Add(ar);
            }

            return modifiedAnalysisResult;
        }
    }
}


