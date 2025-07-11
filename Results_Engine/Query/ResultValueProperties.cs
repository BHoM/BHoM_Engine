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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Analytical.Results;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.Engine.Base;
using System.Collections.ObjectModel;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract all potential result carrying property getters from a result class, i.e. properties of type double that is defined on the class. Properties defined on a base class are ignored.\n" +
                     "Also extracts methods returning a double that has the result type as the only argument.\n" +
                     "Func<T,double> returned together with corresponding QuantityAttribute in a Dictionary with the property or method name as the Key.")]
        [Input("result", "The result type to extract property functions from.")]
        [Output("func", "Dictionary of string, Tuple<Func,Quantity> where the Key is the name of the property or method and the value is the compiled selector for extracting the value from a result of the type with matching QuantityAttribute.")]
        public static Dictionary<string, Tuple<Func<T, double>, QuantityAttribute>> ResultValuePropertiesItem<T>(this T result) where T : IResultItem
        {
            if (result == null)
                return null;

            return ResultValueProperties<T, double>(result);
        }

        /***************************************************/

        [Description("Extract all potential result carrying property getters from a result class, i.e. properties of type IReadOnlyList<double> that is defined on the class. Properties defined on a base class are ignored.\n" +
                     "Also extracts methods returning a double that has the result type as the only argument.\n" +
                     "Func<T,IReadOnlyList<double>> returned together with corresponding QuantityAttribute in a Dictionary with the property or method name as the Key.")]
        [Input("result", "The result type to extract property functions from.")]
        [Output("func", "Dictionary of string, Tuple<Func,Quantity> where the Key is the name of the property or method and the value is the compiled selector for extracting the value from a result of the type with matching QuantityAttribute.")]
        public static Dictionary<string, Tuple<Func<T, IReadOnlyList<double>>, QuantityAttribute>> ResultValuePropertiesSeries<T>(this T result) where T : IResultSeries
        {
            if (result == null)
                return null;

            return ResultValueProperties<T, IReadOnlyList<double>>(result);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Extract all potential result carrying property getters from a result class, i.e. properties of type P that is defined on the class. Properties defined on a base class are ignored.\n" +
                     "Also extracts methods returning a double that has the result type as the only argument.\n" +
                     "Func<T,P> returned together with corresponding QuantityAttribute in a Dictionary with the property or method name as the Key.")]
        [Input("result", "The result type to extract property functions from.")]
        [Output("func", "Dictionary of string, Tuple<Func,Quantity> where the Key is the name of the property or method and the value is the compiled selector for extracting the value from a result of the type with matching QuantityAttribute.")]
        private static Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>> ResultValueProperties<T, P>(this T result) where T : IResult
        {
            if (result == null)
                return null;

            //Get the type of object to evaluate
            Type type = result.GetType();

            //Try extract previously evaluated properties
            object selectors;
            if (m_resultValueSelectors.TryGetValue(type, out selectors))
                return selectors as Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>>;

            //Get all properties of type double declared on the class
            List<PropertyInfo> properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(P).IsAssignableFrom(x.PropertyType)).ToList();

            //Set up dictionary for storing the compiled functiones
            Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>> selectorsDict = new Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>>(StringComparer.OrdinalIgnoreCase);

            //Loop through properties
            foreach (PropertyInfo info in properties)
            {
                //Compile the get method of the property into a function and store in dictionary, using the name of the property as the key.
                //Pro-compiling into functions significantly increases the performance when retreiving result values
                Func<T, P> func = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), info.GetGetMethod());

                //Store units
                IEnumerable<QuantityAttribute> quantity = info.GetCustomAttributes<QuantityAttribute>();
                QuantityAttribute attr = null;
                if (quantity.Count() == 1)
                    attr = quantity.First();

                selectorsDict[info.Name] = new Tuple<Func<T, P>, QuantityAttribute>(func, attr);
            }

            //Add dynamic properties
            if(result is IDynamicObject)
                AddDynamicProperties(result, selectorsDict);

            //Check all available methods that takes a result compatible with the type and returns a double
            foreach (MethodInfo method in Base.Query.BHoMMethodList().Where(x => x.GetParameters().Length == 1).Where(x => typeof(P).IsAssignableFrom(x.ReturnType)).OrderBy(x => x.Name))
            {
                //Check that the method has exactly one argument
                ParameterInfo[] para = method.GetParameters();
                if (para.Length != 1)
                    continue;

                Type firstPara = para[0].ParameterType;
                //check that the first argument is a result argument (to filter out methods accepting object for example)
                if (firstPara == typeof(object) || firstPara == typeof(IObject) || firstPara == typeof(IBHoMObject))
                    continue;

                //Check that this argument is asignable from the type
                if (!para[0].ParameterType.IsAssignableFromIncludeGenerics(type))
                {
                    if (para[0].ParameterType.ContainsGenericParameters)
                    {
                        Type[] genericParams = para[0].ParameterType.GetGenericParameterConstraints();
                        if (!(genericParams.Length == 1 && genericParams[0].IsAssignableFrom(type)))
                            continue;
                    }
                    else
                        continue;
                }

                //If the method is generic, it needs to be instaciated as generic.
                //If the method is not generic, no action will be taken by MakeGenericFromInputs
                MethodInfo finalMethod = method.MakeGenericFromInputs(new List<Type> { type });

                //Turn to a function and store
                Func<T, P> func = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), finalMethod);

                //Try get out and store quantity
                QuantityAttribute quantity = null;
                OutputAttribute outputAtr = method.GetCustomAttribute<OutputAttribute>();
                if (outputAtr != null)
                    quantity = outputAtr.Classification as QuantityAttribute;

                selectorsDict[method.Name] = new Tuple<Func<T, P>, QuantityAttribute>(func, quantity);
            }

            //Store in dictionary for further uses
            m_resultValueSelectors[type] = selectorsDict;
            return selectorsDict;
        }

        /***************************************************/

        private static void AddDynamicProperties<T, P>(this T result, Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>> selectorsDict) where T : IResult
        {
            if (!(result is IDynamicObject))
                return;

            //Get relevant properties out the reference a dictionary type and has the attribute assigned
            List<PropertyInfo> dynamicProps = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Where(prop =>
            {
                if (prop.GetCustomAttribute<DynamicPropertyAttribute>() == null)
                    return false;

                if (!typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
                    return false;

                Type[] args = prop.PropertyType.GenericTypeArguments;
                if (args.Length != 2)
                    return false;

                if (!args[0].IsEnum)
                    return false;

                return typeof(P) == args[1];

            }).ToList();

            //Loop through the dynamic properties
            foreach (var prop in dynamicProps)
            {
                //Initialise an empty instance to help with the dynamic casting. Required to get the correct enum type. Cannot use Activator.CreateInstance as the type could be a ReadOnly type
                var typeObj = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(prop.PropertyType);
                var dynSelectorsDict = AddDynamicProperties(result, typeObj as dynamic, prop) as Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>>;  //Gets all the possible dynamic properties from the dictionary, essentially, one per enum value

                //Loop through and add to main selector
                if (dynSelectorsDict != null)
                { 
                    foreach (var selector in dynSelectorsDict)
                        selectorsDict[selector.Key] = selector.Value;
                }
            }

        }

        /***************************************************/

        private static Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>> AddDynamicProperties<T, P, K>(this T result, IDictionary<K, P> dic, PropertyInfo prop) where T : IResult where K : Enum
        {
            //Func for getting out the dictionary
            Func<T, IDictionary<K, P>> dictFunc = (Func<T, IDictionary<K, P>>)Delegate.CreateDelegate(typeof(Func<T, IDictionary<K,P>>), prop.GetGetMethod());

            //Store units
            IEnumerable<QuantityAttribute> quantity = prop.GetCustomAttributes<QuantityAttribute>();
            QuantityAttribute attr = null;
            if (quantity.Count() == 1)
                attr = quantity.First();

            Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>> selectorsDict = new Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>>();

            //Loop through the enum values to set up a selector for each option
            foreach (var enumItem in Enum.GetValues(typeof(K)))
            {
                K enItm = (K)enumItem;
                string propName = enItm.ToString();

                //Creates the function to access the item from the dictionary based on the string key
                Func<T, P> func = t =>
                {
                    IDictionary<K, P> dict = dictFunc(t);   //Get the dictionary out from the item
                    if (dict.TryGetValue(enItm, out P value))   //Get the value out from the dictioanry
                        return value;
                    else
                    {
                        BH.Engine.Base.Compute.RecordWarning($"Result does not contain the dynamic property {propName}");
                        return default(P);
                    }
                };

                //Store
                selectorsDict[propName] = new Tuple<Func<T, P>, QuantityAttribute>(func, attr);
            }

            return selectorsDict;
        }

        /***************************************************/

        private static void AddDynamicProperties<T>(this T result, object obj, PropertyInfo prop) where T : IResult
        {
            //fallback to avoid full crash
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Type, object> m_resultValueSelectors = new Dictionary<Type, object>();

        /***************************************************/
    }
}



