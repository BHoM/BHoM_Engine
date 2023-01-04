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

using BH.Engine.Reflection;
using BH.Engine.Serialiser;
using BH.Engine.Base;
using BH.Engine.Test;
using BH.oM.Base.Debugging;
using BH.oM.Test;
using BH.oM.Test.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Serialiser
{
    public static partial class Verify
    {
        /*************************************/
        /**** Test Methods                ****/
        /*************************************/

        public static TestResult MethodsToFromJson()
        {
            // Test all the BHoM types available
            List<MethodInfo> methods = Engine.Base.Query.BHoMMethodList().Where(x => !x.IsDeprecated()).ToList();
            List<TestResult> results = methods.Select(x => MethodToFromJson(x)).ToList();

            // Generate the result message
            int errorCount = results.Where(x => x.Status == TestStatus.Error).Count();
            int warningCount = results.Where(x => x.Status == TestStatus.Warning).Count();

            // Returns a summary result 
            return new TestResult()
            {
                ID = "MethodSerialiserToFromJson",
                Description = $"Serialisation of Methods via json: {results.Count} methods available.",
                Message = $"{errorCount} errors and {warningCount} warnings reported.",
                Status = results.MostSevereStatus(),
                Information = results.Where(x => x.Status != TestStatus.Pass).ToList<ITestInformation>(),
                UTCTime = DateTime.UtcNow,
            };
        }

        /*************************************/

        public static TestResult MethodToFromJson(MethodBase method)
        {
            string methodDescription = method.IToText(true);

            // To Json
            string json = "";
            try
            {
                Engine.Base.Compute.ClearCurrentEvents();
                json = method.ToJson();
            }
            catch (Exception e)
            {
                Engine.Base.Compute.RecordError(e.Message);
            }

            if (string.IsNullOrWhiteSpace(json))
                return new TestResult
                {
                    Description = methodDescription,
                    Status = TestStatus.Error,
                    Message = $"Error: Failed to convert method {methodDescription} to json.",
                    Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                };

            // From Json
            MethodInfo copy = null;
            try
            {
                Engine.Base.Compute.ClearCurrentEvents();
                copy = Engine.Serialiser.Convert.FromJson(json) as MethodInfo;
            }
            catch (Exception e)
            {
                Engine.Base.Compute.RecordError(e.Message);
            }

            if (!method.IsEqual(copy))
                return new TestResult
                {
                    Description = methodDescription,
                    Status = TestStatus.Error,
                    Message = $"Error: Method {methodDescription} is not equal to the original after serialisation.",
                    Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                };

            // All test objects passed the test
            return Engine.Test.Create.PassResult(methodDescription);
        }

        /*************************************/
    }
}


