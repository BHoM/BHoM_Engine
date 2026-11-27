/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Looks for an extension method applicable to the input object with the provided `methodName` and, if found, invokes it.\n" +
            "Extension methods are searched using Reflection through all BHoM assemblies.\n" +
            "If no method is found, this returns `false`, and the `result` is null.")]
        [Input("obj", "Object whose extension method is to be found, and to which the method will be applied in order to obtain the result.")]
        [Input("methodName", "Name of the extension method defined for the input object that is to be found in any of the BHoM assemblies.")]
        [Input("result", "Result of the method invocation, if the method had been invoked. If no method is found this is null.")]
        [Output("True if a method was found and an invocation was attempted. False otherwise.")]
        public static bool TryRunExtensionMethod(this object obj, string methodName, out object result)
        {
            return RunCompiledExtensionMethod(obj, methodName, null, out result);
        }

        /***************************************************/

        [Description("Looks for an extension method applicable to the input object with the provided `methodName` and, if found, invokes it.\n" +
            "Extension methods are searched using Reflection through all BHoM assemblies.\n" +
            "If no method is found, this returns `false`, and the `result` is null.")]
        [Input("obj", "Object whose extension method is to be found, and to which the method will be applied in order to obtain the result.")]
        [Input("methodName", "Name of the extension method defined for the input object that is to be found in any of the BHoM assemblies.")]
        [Input("parameters", "The additional arguments for the method call, excluding the first argument provided by 'obj'.")]
        [Input("result", "Result of the method invocation, if the method had been invoked. If no method is found this is null.")]
        [Output("True if a method was found and an invocation was attempted. False otherwise.")]
        public static bool TryRunExtensionMethod(this object obj, string methodName, object[] parameters, out object result)
        {
            return RunCompiledExtensionMethod(obj, methodName, parameters, out result);
        }

        /***************************************************/

        [Description("Looks for an extension method applicable to the input object with the provided `methodName` and, if found, invokes it asynchronously.\n" +
            "Extension methods are searched using Reflection through all BHoM assemblies.\n" +
            "If no method is found, this returns `false`, and the `result` is null.")]
        [Input("obj", "Object whose extension method is to be found, and to which the method will be applied in order to obtain the result.")]
        [Input("methodName", "Name of the extension method defined for the input object that is to be found in any of the BHoM assemblies.")]
        [Output("First output: true if a method was found and an invocation was attempted. False otherwise." +
                "\nSecond output: result of the call if an attempt was made.")]
        public static async Task<Output<bool, object>> TryRunExtensionMethodAsync(this object obj, string methodName)
        {
            return await RunCompiledExtensionMethodAsync(obj, methodName, null);
        }

        /***************************************************/

        [Description("Looks for an extension method applicable to the input object with the provided `methodName` and, if found, invokes it asynchronously.\n" +
            "Extension methods are searched using Reflection through all BHoM assemblies.\n" +
            "If no method is found, this returns `false`, and the `result` is null.")]
        [Input("obj", "Object whose extension method is to be found, and to which the method will be applied in order to obtain the result.")]
        [Input("methodName", "Name of the extension method defined for the input object that is to be found in any of the BHoM assemblies.")]
        [Input("parameters", "The additional arguments of the call to the method, skipping the first argument provided by 'target'.")]
        [Output("First output: true if a method was found and an invocation was attempted. False otherwise." +
                "\nSecond output: result of the call if an attempt was made.")]
        public static async Task<Output<bool, object>> TryRunExtensionMethodAsync(this object obj, string methodName, object[] parameters)
        {
            return await RunCompiledExtensionMethodAsync(obj, methodName, parameters);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Runs the requested method and returns the result. For performance reasons compiles the method to a function the first time it is run, then stores it for subsequent calls.")]
        private static bool RunCompiledExtensionMethod(object obj, string methodName, object[] parameters, out object result)
        {
            // Methods taking the main object only have their own, leaner, path
            if (parameters == null || parameters.Length == 0)
                return RunCompiledSingleArgumentExtensionMethod(obj, methodName, out result);

            Func<object, object[], object> function = FunctionToRun(obj, methodName, parameters);

            // Try calling the method
            try
            {
                if (function == null)
                {
                    result = null;
                    return false;
                }
                else
                {
                    result = function(obj, parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError($"Failed to run {methodName} extension method.\nError: {e.Message}");
                result = null;
                return false;
            }
        }

        /***************************************************/

        [Description("Asynchronously runs the requested method and returns the result. For performance reasons compiles the method to a function the first time it is run, then stores it for subsequent calls.")]
        private static async Task<Output<bool, object>> RunCompiledExtensionMethodAsync(object obj, string methodName, object[] parameters)
        {
            // Methods taking the main object only have their own, leaner, path
            if (parameters == null || parameters.Length == 0)
            {
                Func<object, object> singleArgumentFunction = SingleArgumentFunctionToRun(obj, methodName);
                try
                {
                    if (singleArgumentFunction == null)
                        return new Output<bool, object> { Item1 = false, Item2 = null };
                    else
                        return new Output<bool, object> { Item1 = true, Item2 = await (singleArgumentFunction(obj) as dynamic) };
                }
                catch (Exception e)
                {
                    BH.Engine.Base.Compute.RecordError($"Failed to run {methodName} extension method.\nError: {e.Message}");
                    return new Output<bool, object> { Item1 = false, Item2 = null };
                }
            }

            Func<object, object[], object> function = FunctionToRun(obj, methodName, parameters);

            // Try calling the method
            try
            {
                if (function == null)
                    return new Output<bool, object> { Item1 = false, Item2 = null };
                else
                    return new Output<bool, object> { Item1 = true, Item2 = await (function(obj, parameters) as dynamic) };
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError($"Failed to run {methodName} extension method.\nError: {e.Message}");
                return new Output<bool, object> { Item1 = false, Item2 = null };
            }
        }

        /***************************************************/

        [Description("Runs the requested extension method, that only takes the main object as input, and returns the result. Compiles the method to a function accepting the object directly the first time it is run, then stores it for subsequent calls.")]
        private static bool RunCompiledSingleArgumentExtensionMethod(object obj, string methodName, out object result)
        {
            Func<object, object> function = SingleArgumentFunctionToRun(obj, methodName);

            // Try calling the method
            try
            {
                if (function == null)
                {
                    result = null;
                    return false;
                }
                else
                {
                    result = function(obj);
                    return true;
                }
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError($"Failed to run {methodName} extension method.\nError: {e.Message}");
                result = null;
                return false;
            }
        }

        /***************************************************/

        [Description("Gets the compiled function for an extension method that only takes the main object as input. Returns null if no method is found.")]
        private static Func<object, object> SingleArgumentFunctionToRun(object obj, string methodName)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't find extension method to call because provided method arguments are not valid.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                BH.Engine.Base.Compute.RecordError("Can't find extension method to call because provided method name is not valid.");
                return null;
            }

            // Key is only the type of the object and the method name. Hit = a single dictionary lookup
            SingleArgumentKey key = new SingleArgumentKey(obj.GetType(), methodName);
            Func<object, object> function;
            if (Global.CompiledSingleArgumentExtensionMethods.TryGetValue(key, out function))
                return function;

            // Not found: resolve and compile the method. A null entry is stored when no method was found
            function = CompileSingleArgumentExtensionMethod(obj, methodName);
            return Global.CompiledSingleArgumentExtensionMethods.GetOrAdd(key, function);
        }

        /***************************************************/

        [Description("Finds the extension method with the given name for the provided object and compiles it to a function taking the object directly. Any default values of additional arguments are baked into the compiled function. Null is returned if no applicable method is found.")]
        private static Func<object, object> CompileSingleArgumentExtensionMethod(object obj, string methodName)
        {
            MethodInfo method = Query.ExtensionMethodToCall(methodName, new object[] { obj });
            if (method == null)
                return null;

            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (method.IsStatic && parameters.Length > 0 && !parameters.Any(x => x.ParameterType.IsByRef))
                {
                    ParameterExpression input = Expression.Parameter(typeof(object), "x");
                    Expression[] arguments = new Expression[parameters.Length];
                    arguments[0] = Expression.Convert(input, parameters[0].ParameterType);

                    bool allDefaultsFound = true;
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (!parameters[i].HasDefaultValue)
                        {
                            allDefaultsFound = false;
                            break;
                        }

                        arguments[i] = DefaultValueExpression(parameters[i]);
                    }

                    if (allDefaultsFound)
                    {
                        Expression call = Expression.Call(method, arguments);
                        Expression body;
                        if (method.ReturnType == typeof(void))
                            body = Expression.Block(call, Expression.Constant(true, typeof(object)));  //Same as ToFunc: methods returning void return true
                        else
                            body = Expression.Convert(call, typeof(object));

                        return Expression.Lambda<Func<object, object>>(body, input).Compile();
                    }
                }
            }
            catch (Exception)
            {
                // Fall through to the general compilation below
            }

            // Anything not handled above is compiled the general way, and wrapped
            Func<object, object[], object> function = CompileExtensionMethod(obj, methodName, null);
            if (function == null)
                return null;

            return x => function(x, null);
        }

        /***************************************************/

        [Description("Creates the expression for the default value of an optional parameter.")]
        private static Expression DefaultValueExpression(ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            object value = parameter.DefaultValue;

            if (value == null || value is DBNull)
                return Expression.Default(parameterType);

            // Default values of enums can be reported as their underlying type
            Type underlyingType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;
            if (underlyingType.IsEnum && value.GetType() != underlyingType)
                value = Enum.ToObject(underlyingType, value);

            return Expression.Constant(value, parameterType);
        }

        /***************************************************/

        [Description("Gets the compiled function to run for the provided object, method name and additional arguments. For performance reasons compiles the method to a function the first time it is requested, then stores it for subsequent calls. Returns null if no method is found.")]
        private static Func<object, object[], object> FunctionToRun(object obj, string methodName, object[] parameters)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't find extension method to call because provided method arguments are not valid.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(methodName))
            {
                BH.Engine.Base.Compute.RecordError("Can't find extension method to call because provided method name is not valid.");
                return null;
            }

            // Look for the method with a key built directly from the runtime types of the arguments. Hit = a single dictionary lookup
            ExtensionMethodKey key = new ExtensionMethodKey(methodName, obj, parameters);
            Func<object, object[], object> function;
            if (Global.CompiledExtensionMethods.TryGetValue(key, out function))
                return function;

            // Not found: resolve and compile the method (done once for each combination of method name and argument types)
            function = CompileExtensionMethod(obj, methodName, parameters);

            // Store the method. A null entry is stored when no method was found
            return Global.CompiledExtensionMethods.GetOrAdd(key, function);
        }

        /***************************************************/

        [Description("Finds the extension method with the given name and arguments and compiles it to a function taking the main object and the additional arguments as provided, adding the default values of any further parameters. Null is returned if no applicable method is found.")]
        private static Func<object, object[], object> CompileExtensionMethod(object obj, string methodName, object[] parameters)
        {
            // The lookup of the method requires the full set of arguments
            object[] allArguments = BuildArguments(obj, parameters, null);

            MethodInfo method = Query.ExtensionMethodToCall(methodName, allArguments);
            if (method == null)
                return null;

            //ExtensionMethodToCall allows for returning methods that have more arguments than the provided parameters, if those have default value.
            //Check that all parameters not provided have a default value.
            //This should always be true, based on the logic of the ExtensionMethodToCall but additional check here for safety
            ParameterInfo[] parameterInfo = method.GetParameters();
            int providedCount = allArguments.Length;
            for (int i = providedCount; i < parameterInfo.Length; i++)
            {
                if (!parameterInfo[i].HasDefaultValue)
                    return null;
            }

            //Compiles the method to a function for quicker execution
            Func<object[], object> methodFunc = method.ToFunc();
            if (methodFunc == null)
                return null;

            //The default values of the parameters not provided get appended to the arguments of every call
            object[] defaultArgs = parameterInfo.Skip(providedCount).Select(x => x.DefaultValue).ToArray();
            return (x, p) => methodFunc(BuildArguments(x, p, defaultArgs));
        }

        /***************************************************/

        [Description("Builds the array of arguments to call the compiled method with: the main object first, then the additional arguments, then any default values to fill in. Creates a single array of the exact size.")]
        private static object[] BuildArguments(object obj, object[] parameters, object[] defaultArguments)
        {
            int parameterCount = parameters?.Length ?? 0;
            int defaultCount = defaultArguments?.Length ?? 0;

            object[] arguments = new object[1 + parameterCount + defaultCount];
            arguments[0] = obj;

            if (parameterCount > 0)
                Array.Copy(parameters, 0, arguments, 1, parameterCount);

            if (defaultCount > 0)
                Array.Copy(defaultArguments, 0, arguments, 1 + parameterCount, defaultCount);

            return arguments;
        }

        /***************************************************/


    }
}
