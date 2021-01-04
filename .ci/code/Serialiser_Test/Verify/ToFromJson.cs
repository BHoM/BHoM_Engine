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

using BH.Engine.Reflection;
using BH.Engine.Serialiser;
using BH.Engine.Test;
using BH.oM.Reflection.Debugging;
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

        public static TestResult ToFromJson()
        {
            // Test all the BHoM types available
            List<Type> types = Helpers.TypesToTest();
            List<TestResult> results = types.Select(x => ToFromJson(x)).ToList();

            // Returns a summary result 
            List<TestResult> fails = results.Where(x => x.Status == ResultStatus.Fail).ToList();
            string description = $"{results.Count} types of objects available in BH.oM.";
            if (fails.Count == 0)
                return Engine.Test.Create.PassResult(description);
            else
                return new TestResult(ResultStatus.Fail, fails.SelectMany(x => x.Events).ToList(), description);
        }

        /*************************************/

        public static TestResult ToFromJson(Type type)
        {
            string typeDescription = type.IToText(true);

            // Create the test objects of the given type
            List<object> testObjects = Helpers.TestObjects(type);
            if (testObjects.Count == 0)
            {
                object dummy = null;
                try
                {
                    dummy = Engine.Test.Compute.DummyObject(type);
                }
                catch { }

                if (dummy == null)
                    return Engine.Test.Create.FailResult($"Failed to create a dummy object of type {typeDescription}.", typeDescription);
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
                    json = testObject.ToJson();
                }
                catch { }

                if (string.IsNullOrWhiteSpace(json))
                    return Engine.Test.Create.FailResult($"Failed to convert object of type {typeDescription} to json.", typeDescription);

                // From Json
                object copy = null;
                try
                {
                    copy = Engine.Serialiser.Convert.FromJson(json);
                }
                catch { }

                if (!copy.IsEqual(testObject))
                    return Engine.Test.Create.FailResult($"Object of type {typeDescription} is not equal to the original after serialisation.", typeDescription);
            }

            // All test objects passed the test
            return Engine.Test.Create.PassResult(typeDescription);
        }

        /*************************************/
    }
}
