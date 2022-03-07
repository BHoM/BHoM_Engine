/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
        public static Dictionary<string, Tuple<Func<T, double>, QuantityAttribute>> ResultItemValueProperties<T>(this T result) where T : IResultItem
        {
            if (result == null)
                return null;

            //Get the type of obejct to evaluate
            Type type = result.GetType();

            //Try extract previously evaluated properties
            object selectors;
            if (m_resultValueSelectors.TryGetValue(type, out selectors))
                return selectors as Dictionary<string, Tuple<Func<T, double>, QuantityAttribute>>;

            //Get all properties of type double declared on the class
            List<PropertyInfo> properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Where(x => x.PropertyType == typeof(double)).ToList();

            //Set up dictionary for storing the compiled functiones
            Dictionary<string, Tuple<Func<T, double>, QuantityAttribute>> selectorsDict = new Dictionary<string, Tuple<Func<T, double>, QuantityAttribute>>();

            //Loop through properties
            foreach (PropertyInfo info in properties)
            {
                //Compile the get method of the property into a function and store in dictionary, using the name of the property as the key.
                //Pro-compiling into functions significantly increases the performance when retreiving result values
                Func<T, double> func = (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), info.GetGetMethod());

                //Store units
                IEnumerable<QuantityAttribute> quantity = info.GetCustomAttributes<QuantityAttribute>();
                QuantityAttribute attr = null;
                if (quantity.Count() == 1)
                    attr = quantity.First();

                selectorsDict[info.Name] = new Tuple<Func<T, double>, QuantityAttribute>(func, attr);
            }

            //Check all available methods that takes a result compatible with the type and returns a double
            foreach (MethodInfo method in Base.Query.BHoMMethodList().Where(x => x.ReturnType == typeof(double)))
            {
                //Check that the method has exactly one argument
                ParameterInfo[] para = method.GetParameters();
                if (para.Length != 1)
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
                Func<T, double> func = (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), finalMethod);

                //Try get out and store quantity
                QuantityAttribute quantity = null;
                OutputAttribute outputAtr = method.GetCustomAttribute<OutputAttribute>();
                if (outputAtr != null)
                    quantity = outputAtr.Classification as QuantityAttribute;

                selectorsDict[method.Name] = new Tuple<Func<T, double>, QuantityAttribute>(func, quantity);
            }

            //Store in dictionary for further uses
            m_resultValueSelectors[type] = selectorsDict;
            return selectorsDict;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Type, object> m_resultValueSelectors = new Dictionary<Type, object>();
        
        /***************************************************/
    }
}
