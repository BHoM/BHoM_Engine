/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.Engine.Diffing;
using BH.Engine.Serialiser;
using BH.Engine.Test;
using BH.oM.Base.Debugging;
using BH.oM.Test;
using BH.oM.Test.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Serialiser
{
    public static partial class Verify
    {
        /*************************************/
        /**** Test Methods                ****/
        /*************************************/

        public static TestResult TypesToFromJson()
        {
            // Test all the BHoM types available
            List<Type> types = Engine.Base.Query.BHoMTypeList()
                .Concat(Engine.Base.Query.BHoMInterfaceTypeList())
                .Concat(Engine.Base.Query.AdapterTypeList())
                .ToList();
            List<TestResult> results = types.Select(x => TypeToFromJson(x)).ToList();

            // Generate the result message
            int errorCount = results.Where(x => x.Status == TestStatus.Error).Count();
            int warningCount = results.Where(x => x.Status == TestStatus.Warning).Count();

            // Returns a summary result 
            return new TestResult()
            {
                ID = "TypeSerialiserToFromJson",
                Description = $"Serialisation of Types via json: {results.Count} types available.",
                Message = $"{errorCount} errors and {warningCount} warnings reported.",
                Status = results.MostSevereStatus(),
                Information = results.Where(x => x.Status != TestStatus.Pass).ToList<ITestInformation>(),
                UTCTime = DateTime.UtcNow,
            };
        }

        /*************************************/

        public static TestResult TypeToFromJson(Type type)
        {
            string typeDescription = type.IToText(true);

            // To Json
            string json = "";
            try
            {
                Engine.Base.Compute.ClearCurrentEvents();
                json = type.ToJson();
            }
            catch (Exception e)
            {
                Engine.Base.Compute.RecordError(e.Message);
            }

            if (string.IsNullOrWhiteSpace(json))
                return new TestResult
                {
                    Description = typeDescription,
                    Status = TestStatus.Error,
                    Message = $"Error: Failed to convert type {typeDescription} to json.",
                    Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                };

            // From Json
            Type copy = null;
            try
            {
                Engine.Base.Compute.ClearCurrentEvents();
                copy = Engine.Serialiser.Convert.FromJson(json) as Type;
            }
            catch (Exception e)
            {
                Engine.Base.Compute.RecordError(e.Message);
            }

            if (!type.IsEqual(copy))
                return new TestResult
                {
                    Description = typeDescription,
                    Status = TestStatus.Error,
                    Message = $"Error: Type {typeDescription} is not equal to the original after serialisation.",
                    Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                };

            // All test objects passed the test
            return Engine.Test.Create.PassResult(typeDescription);
        }

        /*************************************/
    }
}




