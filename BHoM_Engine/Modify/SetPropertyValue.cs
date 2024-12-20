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
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Set the value of a property with a given name for an object")]
        [Input("obj", "object to set the value for")]
        [Input("propName", "name of the property to set the value of")]
        [Input("value", "new value of the property. \nIf left empty, the value for that property will be cleared \n(enumerables will be emptied, primitives will be set to their default value, and objects will be set to null)")]
        [Output("result", "New object with its property changed to the new value")]
        public static object SetPropertyValue(this object obj, string propName, object value = null)
        {
            if(obj == null)
            {
                Compute.RecordError("Cannot set the property value of a null object.");
                return obj;
            }

            if(propName == null)
            {
                Compute.RecordError("Cannot set the property value where the property name is null.");
                return obj;
            }

            object toChange = obj;
            if (propName.Contains("."))
            {
                string[] props = propName.Split('.');
                for (int i = 0; i < props.Length - 1; i++)
                {
                    toChange = toChange.PropertyValue(props[i]);
                    if (toChange == null)
                        break;
                }
                propName = props[props.Length - 1];
            }

            System.Reflection.PropertyInfo prop = toChange.GetType().GetProperty(propName);
            if (prop != null)
            {
                if (!prop.CanWrite)
                {
                    Compute.RecordError("This property doesn't have a public setter so it is not possible to modify it.");
                    return obj;
                }

                Type propType = prop.PropertyType;
                if (value == null)
                {
                    if (propType == typeof(string))
                        value = "";
                    else if (propType.IsValueType || typeof(IEnumerable).IsAssignableFrom(propType))
                        value = Activator.CreateInstance(propType);
                }

                if (value is string)
                {
                    if (propType.IsEnum)
                    {
                        string enumName = (value as string).Split('.').Last();
                        try
                        {
                            object enumValue = Compute.ParseEnum(propType, enumName);
                            if (enumValue != null)
                                value = enumValue;
                        }
                        catch
                        {
                            Compute.RecordError($"An enum of type {propType.ToText(true)} does not have a value of {enumName}");
                        }
                    }

                    if (typeof(IEnum).IsAssignableFrom(propType))
                    {
                        value = CreateEnumeration(propType, value as string);
                    }

                    if (propType == typeof(Type))
                        value = Create.Type(value as string);
                    else if (propType == typeof(double))
                    {
                        double result = 0;
                        double.TryParse(value as string, out result);
                        value = result;
                    }
                    else if (propType == typeof(int))
                    {
                        int result = 0;
                        int.TryParse(value as string, out result);
                        value = result;
                    }
                    else if (propType == typeof(bool))
                    {
                        bool result = false;
                        bool.TryParse(value as string, out result);
                        value = result;
                    }
                }

                if (propType == typeof(DateTime))
                {
                    if (value is string)
                    {
                        DateTime date;
                        if (DateTime.TryParse(value as string, out date))
                            value = date;
                        else
                        {
                            Compute.RecordError($"The value provided for {propName} is not a valid DateTime.");
                            value = DateTime.MinValue;
                        }
                    }
                    else if (value is double)
                        value = DateTime.FromOADate((double)value);
                }

                if (propType == typeof(FragmentSet))
                {
                    if (value is IFragment)
                        value = new FragmentSet { value as IFragment };
                    else if (value is IEnumerable && !(value is FragmentSet))
                        value = new FragmentSet(((IEnumerable)value).OfType<IFragment>().ToList());
                }

                if (value != null)
                {
                    if (value.GetType() != propType && value.GetType().GenericTypeArguments.Length > 0 && propType.GenericTypeArguments.Length > 0)
                    {
                        value = Modify.CastGeneric(value as dynamic, propType.GenericTypeArguments[0]);
                    }
                    if (value.GetType() != propType)
                    {
                        ConstructorInfo constructor = propType.GetConstructor(new Type[] { value.GetType() });
                        if (constructor != null)
                            value = constructor.Invoke(new object[] { value });
                    }
                }
                
                try
                {
                    prop.SetValue(toChange, value);   
                }
                catch (Exception e)
                {
                    Compute.RecordError($"Failed to set the property {propName} of {obj.GetType().ToString()} to {value.ToString()}");
                }
                return obj;
            }
            else 
            {
                SetValue(toChange as dynamic, propName, value);
                return obj;
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool SetValue(this IBHoMObject obj, string propName, object value)
        {
            if (obj == null) return false;

            if (value is IFragment)
            {
                obj.Fragments.Add(value as IFragment);
                Compute.RecordWarning("The object does not contain any property with the name " + propName + ". The value is being set as a fragment.");
            }
            else
            {
                obj.CustomData[propName] = value;
                if (!(obj is CustomObject))
                    Compute.RecordWarning("The object does not contain any property with the name " + propName + ". The value is being set as custom data.");
            }

            return true;
        }

        /***************************************************/

        private static bool SetValue(this IDictionary dic, string propName, object value)
        {
            dic[propName] = value;
            return true;
        }

        /***************************************************/

        private static bool SetValue<T>(this IEnumerable<T> list, string propName, object value)
        {
            bool success = true;

            foreach (T item in list)
                success &= SetPropertyValue(item, propName, value) != null;

            return success;
        }

        /***************************************************/

        private static bool SetValue(this object obj, string propName, object value)
        {
            Compute.RecordWarning("The objects does not contain any property with the name " + propName + ".");
            return false;
        }

        /***************************************************/

        private static Enumeration CreateEnumeration(Type type, string value)
        {
            if (type == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create an enumeration from a null type");
                return null;
            }

            List<Enumeration> test = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .OfType<Enumeration>().ToList();

            Enumeration result = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .OfType<Enumeration>()
                     .Where(x => x.Value == value || x.Description == value)
                     .FirstOrDefault();

            if (result == null)
                BH.Engine.Base.Compute.RecordError($"Cannot create an enumeration from type {type.ToString()} and value {value}");

            return result;
        }

        /***************************************************/
    }
}




