/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BHRE = BH.Engine.Reflection;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filters a list of objects to output only those with a property value matching that input")]
        [Input("objects", "List of objects to be filtered. All objects in the list should be of similar type")]
        [Input("propertyName", "The name of the property to filter the objects by")]
        [Input("value", "The value of the selected property to filter the objects by")]
        [Input("ignoreStringCase", "Ignore upper/lower case letters if the property filtered by is a text/string.")]
        [Input("exactStringMatch", "If true checks if the property filtered by contains the text value (filtering must be a string/text)")]
        public static List<T> FilterBy<T>(this List<T> objects, string propertyName = null, object value = null, bool ignoreStringCase = false, bool exactStringMatch = true)
        {
            if (objects == null || objects.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No objects submitted to filter");
                return new List<T>();
            }

            if (propertyName == null)
            {
                BH.Engine.Reflection.Compute.RecordError("propertyName cannot be null in order to filter the objects");
                return new List<T>();
            }

            if (value == null)
            {
                BH.Engine.Reflection.Compute.RecordError("value cannot be null to filter the objects");
                return new List<T>();
            }

            object firstObject = objects.Where(x => x != null).FirstOrDefault();
            if(firstObject == null)
            {
                BH.Engine.Reflection.Compute.RecordError("All objects in the list to filter are null, please try with valid objects");
                return new List<T>();
            }

            bool isNested = propertyName.Contains(".");
            bool isCustomData = false;

            System.Reflection.PropertyInfo prop = firstObject.GetType().GetProperty(propertyName);
            if (prop == null && firstObject is IBHoMObject)
            {
                if ((firstObject as IBHoMObject).CustomData.ContainsKey(propertyName))
                    isCustomData = true;
            }

            System.Type type = PropertyType(firstObject, propertyName);

            int groupedObjects = objects.GroupBy(x => x.GetType()).Count();
            if (groupedObjects != 1)
            {
                BHRE.Compute.RecordError("All objects in the list to be sorted should be of similiar type.");
                return null;
            }

            if (type == typeof(System.String))
            {
                string stringValue = value.ToString();
                if (ignoreStringCase && exactStringMatch)
                {
                    if (!isNested)
                        return objects.Where(x => string.Equals((string)prop.GetValue(x), stringValue, System.StringComparison.CurrentCultureIgnoreCase)).ToList();
                    else
                        return objects.Where(x => string.Equals((string)BHRE.Query.PropertyValue(x, propertyName), stringValue, System.StringComparison.CurrentCultureIgnoreCase)).ToList();
                }
                if (!exactStringMatch)
                {
                    if (!isNested)
                    {
                        if (ignoreStringCase)
                            return objects.Where(x => (prop.GetValue(x) as string).ToUpper().Contains(stringValue.ToUpper())).ToList();
                        else
                            return objects.Where(x => (prop.GetValue(x) as string).Contains(stringValue)).ToList();
                    }
                    else
                    {
                        if (ignoreStringCase)
                            return objects.Where(x => (BHRE.Query.PropertyValue(x, propertyName) as string).ToUpper().Contains(stringValue.ToUpper())).ToList();
                        else
                            return objects.Where(x => (BHRE.Query.PropertyValue(x, propertyName) as string).Contains(stringValue)).ToList();
                    }
                }
                if (!isNested)
                    return objects.Where(x => prop.GetValue(x).Equals(stringValue)).ToList();
                else
                    return objects.Where(x => BHRE.Query.PropertyValue(x, propertyName).Equals(stringValue)).ToList();
            }

            if (type == typeof(System.Double))
            {
                double doubleValue = 0.0;
                string searchValue = value.ToString();
                int rounding = searchValue.Contains('.') ? searchValue.Split('.').Last().Length : 0;

                try
                { 
                    double.TryParse(searchValue, out doubleValue); 
                }
                catch
                { 
                    BHRE.Compute.RecordError("Property value to search for cannot be converted to a number");
                }

                if (!isNested && !isCustomData)
                    return objects.Where(x => System.Math.Round((double)prop.GetValue(x), rounding) == doubleValue).ToList();
                else
                    return objects.Where(x => System.Math.Round((double)BHRE.Query.PropertyValue(x, propertyName), rounding) == doubleValue).ToList();
            }

            if (prop != null && !isNested)
            {
                dynamic val = System.Convert.ChangeType(value, prop.PropertyType);
                return objects.Where(x => (System.Convert.ChangeType(prop.GetValue(x), type) as dynamic) == val).ToList();
            }

            return objects.Where(x => BHRE.Query.PropertyValue(x, propertyName) == value).ToList();
        }

        /***************************************************/

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static System.Type PropertyType(this object obj, string propName)
        {
            if (obj == null || propName == null)
                return null;

            if (propName.Contains("."))
            {
                string[] props = propName.Split('.');
                for (int i = 0; i < props.Count() - 1; i++)
                {
                    obj = BHRE.Query.PropertyValue(obj, props[i]);
                    if (obj == null)
                        break;
                }
                return PropertyType(obj, props.Last());
            }
            
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            if (prop != null) return prop.PropertyType;

            if (obj is IBHoMObject)
            {
                IBHoMObject bhom = obj as IBHoMObject;
                if (bhom.CustomData.ContainsKey(propName))
                {
                    if (!(bhom is CustomObject))
                        BHRE.Compute.RecordNote($"{propName} is stored in CustomData");
                    return bhom.CustomData[propName].GetType();
                }
                else
                {
                    BHRE.Compute.RecordWarning($"{bhom} does not contain a property: {propName}, or: CustomData[{propName}]");
                    return null;
                }

            }
            else if (obj is IDictionary)
            {
                IDictionary dic = obj as IDictionary;
                if (dic.Contains(propName))
                {
                    return dic[propName].GetType();
                }
                else
                {
                    BHRE.Compute.RecordWarning($"{dic} does not contain the key: {propName}");
                    return null;
                }
            }
            else
            {
                BHRE.Compute.RecordWarning($"This instance of {obj.GetType()} does not contain the property: {propName}");
                return null;
            }
        }

        /***************************************************/
    }
}