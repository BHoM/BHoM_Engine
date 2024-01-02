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

using BH.Engine.Base;
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

        public static TestResult ObjectsToFromJson()
        {
            // Test all the BHoM types available
            List<Type> types = Helpers.ObjectTypesToTest();
            List<TestResult> results = types.Select(x => ObjectToFromJson(x)).ToList();

            // Generate the result message
            int errorCount = results.Where(x => x.Status == TestStatus.Error).Count();
            int warningCount = results.Where(x => x.Status == TestStatus.Warning).Count();

            // Returns a summary result 
            return new TestResult()
            {
                ID = "ObjectSerialiserToFromJson",
                Description = $"Serialisation of Objects via json: {results.Count} types of objects available in BH.oM.",
                Message = $"{errorCount} errors and {warningCount} warnings reported.",
                Status = results.MostSevereStatus(),
                Information = results.Where(x => x.Status != TestStatus.Pass).ToList<ITestInformation>(),
                UTCTime = DateTime.UtcNow,
            };
        }

        /*************************************/

        public static TestResult ObjectToFromJson(Type type)
        {
            string typeDescription = type.IToText(true);

            // Create the test objects of the given type
            List<object> testObjects = Helpers.TestObjects(type);
            if (testObjects.Count == 0)
            {
                object dummy = null;
                try
                {
                    Engine.Base.Compute.ClearCurrentEvents();
                    dummy = Engine.Test.Compute.DummyObject(type);
                }
                catch (Exception e)
                {
                    Engine.Base.Compute.RecordWarning(e.Message);
                }

                if (dummy == null)
                    return new TestResult
                    {
                        Description = typeDescription,
                        Status = TestStatus.Warning,
                        Message = $"Warning: Failed to create a dummy object of type {typeDescription}.",
                        Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                    };
                else
                    testObjects.Add(dummy);
            }

            // Test each object in the list
            foreach (object testObject in testObjects)
            {
                // To Json
                string json = "";
                try
                {
                    Engine.Base.Compute.ClearCurrentEvents();
                    json = testObject.ToJson();
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
                        Message = $"Error: Failed to convert object of type {typeDescription} to json.",
                        Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                    };

                // From Json
                object copy = null;
                try
                {
                    Engine.Base.Compute.ClearCurrentEvents();
                    copy = Engine.Serialiser.Convert.FromJson(json);
                }
                catch (Exception e)
                {
                    Engine.Base.Compute.RecordError(e.Message);
                }

                if (!testObject.IsEqual(copy))
                    return new TestResult
                    {
                        Description = typeDescription,
                        Status = TestStatus.Error,
                        Message = $"Error: Object of type {typeDescription} is not equal to the original after serialisation.",
                        Information = Engine.Base.Query.CurrentEvents().Select(x => x.ToEventMessage()).ToList<ITestInformation>()
                    };
            }

            // All test objects passed the test
            return Engine.Test.Create.PassResult(typeDescription);
        }

        /*************************************/
    }
}



