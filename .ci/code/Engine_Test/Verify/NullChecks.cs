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

using BH.Engine.Reflection;
using BH.Engine.Base;
using BH.Engine.Test;
using BH.oM.Base.Debugging;
using BH.oM.Test;
using BH.oM.Test.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Engine
{
    public static partial class Verify
    {
        /*************************************/
        /**** Test Methods                ****/
        /*************************************/

        public static TestResult NullChecks()
        {
            BH.Engine.Base.Compute.LoadAllAssemblies();

            // Ignore because of stack overflow or need to install external libraries
            List<string> toIgnore = new List<string> { "MachineLearning", "Python", "LifeCycleAssessment.Query.GetEvaluationValue", "Forms.Compute.FilePathSelectionDialog"  };

            // Test all the BHoM types available
            List<MethodInfo> methods = BH.Engine.Base.Query.BHoMMethodList()
                .Where(x => !x.IsDeprecated())
                .Where(x => !toIgnore.Any(i => $"{x.DeclaringType.Namespace}.{x.DeclaringType.Name}.{x.Name}".Contains(i)))
                .ToList();

            List<TestResult> results = methods.Select(x => NullChecks(x)).ToList();

            // Generate the result message
            int errorCount = results.Where(x => x.Status == TestStatus.Error).Count();
            int warningCount = results.Where(x => x.Status == TestStatus.Warning).Count();

            // Uncomment if you want the results written in a file for convenience
            //List<string> failingMethods = results.Where(x => x.Status == TestStatus.Error).Select(x => x.Description).ToList();
            //File.WriteAllLines(@"C:\Temp\MethodsFaillingNullChecks.txt", failingMethods);

            // Returns a summary result 
            return new TestResult()
            {
                ID = "NullChecks",
                Description = $"All methods are running the necessary null checks: {results.Count} methods available.",
                Message = $"{errorCount} errors and {warningCount} warnings reported.",
                Status = results.MostSevereStatus(),
                Information = results.Where(x => x.Status != TestStatus.Pass).ToList<ITestInformation>(),
                UTCTime = DateTime.UtcNow,
            };
        }

        /*************************************/

        public static TestResult NullChecks(MethodInfo method)
        {
            //Check if the method provided is null
            if (method == null)
            {
                return new TestResult
                {
                    Description = "",
                    Status = TestStatus.Warning,
                    Message = $"Warning: The provided method is null and cannot be tested.",
                };
            }

            string methodDescription = method.IToText(true);

            //Check if method is generic type, and if so, make it generic based on its constraints
            try
            {
                if (method.IsGenericMethodDefinition)
                    method = method.MakeFromGeneric();

                if (method == null)
                    return GenericsFailedResult(methodDescription);
            }
            catch (Exception e)
            {
                return GenericsFailedResult(methodDescription, e);
            }

            // Collect the inputs setting themm to null when relevant
            object[] inputs = new object[0];
            try
            {
                inputs = method.GetParameters().Select(x => x.ParameterType).Select(t =>
                {
                    
                    if (t == typeof(string))
                        return "";
                    else if (t.IsValueType) // It is acceptable to consider that lists will not be null
                        return Activator.CreateInstance(t);

                    if (t.Name == $"HashSet`1")
                    {
                        Type listType = typeof(HashSet<>).MakeGenericType(new Type[] { t.GetGenericArguments().First() });
                        return Activator.CreateInstance(listType);
                    }
                    else if (t.Name == "Dictionary`2")
                    {
                        Type listType = typeof(Dictionary<,>).MakeGenericType(t.GetGenericArguments());
                        return Activator.CreateInstance(listType);
                    }
                    else if (t.GetInterfaces().Any(x => x.Name == "IEnumerable`1"))
                    {
                        Type listType = typeof(List<>).MakeGenericType(new Type[] { t.GetGenericArguments().First() });
                        return Activator.CreateInstance(listType);
                    }

                    else
                        return null;
                }).ToArray();
            }
            catch (Exception e)
            {
                return new TestResult
                {
                    Description = methodDescription,
                    Status = TestStatus.Warning,
                    Message = $"Warning: Failed to generate inputs for method {methodDescription}. It will not be tested.",
                    Information = new List<ITestInformation> { new EventMessage { Message = e.Message, StackTrace = e.StackTrace } }
                };
            }

            // Try invoking the method
            try
            {
                method.Invoke(null, inputs);
            }
            catch (Exception e)
            {
                while (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                if (e is NullReferenceException)
                {
                    return new TestResult
                    {
                        Description = methodDescription,
                        Status = TestStatus.Error,
                        Message = $"Error: A NullReferenceException was received from method {methodDescription}.",
                    };
                }
                else if (e is ArgumentNullException)
                {
                    return new TestResult
                    {
                        Description = methodDescription,
                        Status = TestStatus.Error,
                        Message = $"Error: A ArgumentNullException was received from method {methodDescription}.",
                    };
                }
                else if (e is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    return new TestResult
                    {
                        Description = methodDescription,
                        Status = TestStatus.Error,
                        Message = $"Error: A RuntimeBinderException was received from method {methodDescription}. This indicates trying to cast a null as dynamic.",
                    };
                }
                else
                {
                    return new TestResult
                    {
                        Description = methodDescription,
                        Status = TestStatus.Warning,
                        Message = $"Warning: Failed to generate test method {methodDescription} for null checks because it thew another type of exception.",
                        Information = new List<ITestInformation> { new EventMessage { Message = e.Message, StackTrace = e.StackTrace } }
                    };
                }
            }

            // All test objects passed the test
            return BH.Engine.Test.Create.PassResult(methodDescription);
        }

        /*************************************/

        private static TestResult GenericsFailedResult(string methodDescription, Exception e = null)
        {
            List<ITestInformation> information = new List<ITestInformation>();
            if (e != null)
                information.Add(new EventMessage { Message = e.Message, StackTrace = e.StackTrace });

            return new TestResult
            {
                Description = methodDescription,
                Status = TestStatus.Warning,
                Message = $"Warning: Failed to make method {methodDescription} into a generic method. It will not be tested.",
                Information = information
            };
        }

        /*************************************/
    }
}



