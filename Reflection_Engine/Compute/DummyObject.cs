/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Debugging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object DummyObject(Type type)
        {
            if (m_ImplementingTypes.Count == 0)
                LinkInterfaces(Query.BHoMTypeList());

            return InitialiseObject(type, 0);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static object InitialiseObject(Type type, int depth = 0)
        {
            try
            {
                // Create object
                object obj;
                if (type.GetInterface("IImmutable") != null)
                    return CreateImmutable(type, depth);
                else if (type.IsGenericType)
                {
                    type = GetType(type);
                    obj = Activator.CreateInstance(type);
                }
                else if (type.IsEnum)
                {
                    Array values = Enum.GetValues(type);
                    return values.GetValue(values.Length - 1);
                }
                else
                    obj = Activator.CreateInstance(type);

                // Set its public properties
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    if (prop.CanWrite && prop.SetMethod.GetParameters().Count() == 1)
                        prop.SetValue(obj, GetValue(prop.PropertyType, depth));
                }

                return obj;
            }
            catch (Exception e)
            {
                Reflection.Compute.RecordWarning("Failed to generate object for type " + type.FullName);
                return null;
            }
            
        }

        /*******************************************/

        private static object GetValue(Type type, int depth)
        {
            try
            {
                if (type.IsPrimitive)
                {
                    if (type == typeof(bool))
                        return true;
                    else if (type == typeof(int))
                        return 42;
                    else if (type == typeof(double))
                        return 42.42;
                    else if (type == typeof(float))
                        return 42.42f;
                    else if (type == typeof(char))
                        return 't';
                    else
                        return Activator.CreateInstance(type);
                }
                else if (type == typeof(decimal))
                    return new Decimal(42.42);
                else if (type == typeof(Guid))
                    return Guid.NewGuid();
                else if (type == typeof(string))
                    return "test";
                else if (type.IsEnum)
                {
                    Array values = Enum.GetValues(type);
                    return values.GetValue(values.Length - 1);
                }
                else if (type == typeof(System.Drawing.Color))
                    return System.Drawing.Color.FromArgb(1, 2, 3, 4);
                else if (type == typeof(System.Drawing.Bitmap))
                    return new System.Drawing.Bitmap(10, 10);
                else if (type == typeof(System.Data.DataTable))
                {
                    System.Data.DataTable table = new System.Data.DataTable("test");
                    table.Columns.AddRange(new System.Data.DataColumn[] {
                        new System.Data.DataColumn("col1", typeof(int)),
                        new System.Data.DataColumn("col2", typeof(string))
                    });
                    System.Data.DataRow row = table.NewRow();
                    row[0] = 4;
                    row[1] = "test";
                    table.Rows.Add(row);
                    return table;
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    IDictionary dic = Activator.CreateInstance(GetType(type)) as IDictionary;
                    Type[] typeArguments = type.GetGenericArguments();
                    object key = GetValue(typeArguments[0], depth + 1);
                    object val = GetValue(typeArguments[1], depth + 1);
                    if (key != null && val != null)
                        dic.Add(key, val);
                    return dic;
                }
                else if (type.IsArray)
                {
                    ConstructorInfo constructor = type.GetConstructors().First();
                    object[] dims = constructor.GetParameters().Select(x => (object)1).ToArray();
                    Array array = constructor.Invoke(dims) as Array;

                    if (dims.Length == 1)
                    {
                        object val = GetValue(type.GetElementType(), depth + 1);
                        if (val != null)
                            array.SetValue(val, 0);
                    }
                    else if (dims.Length == 2)
                    {
                        object val = GetValue(type.GetElementType(), depth + 1);
                        if (val != null)
                            array.SetValue(val, 0, 0);
                    }
                    return array;
                }
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    if (type == typeof(FragmentSet))
                    {
                        return new FragmentSet();
                    }
                    else if (type.Name == "ReadOnlyCollection`1")
                    {
                        Type listType = typeof(List<>).MakeGenericType(type.GetGenericArguments());
                        IList list = Activator.CreateInstance(listType) as IList;
                        object val = GetValue(type.GetGenericArguments()[0], depth + 1);
                        if (val != null)
                            list.Add(val);
                        return type.GetConstructors().First().Invoke(new object[] { list });
                    }
                    {
                        IList list = Activator.CreateInstance(GetType(type)) as IList;
                        object val = GetValue(type.GetGenericArguments()[0], depth + 1);
                        if (val != null)
                            list.Add(val);
                        return list;
                    }
                }
                else if (type.Name == "HashSet`1")
                {
                    return Activator.CreateInstance(GetType(type));
                }
                else if (type.Name.StartsWith("Tuple`"))
                {
                    Type[] keys = type.GetGenericArguments();
                    Type tupleType = Type.GetType("System.Tuple`" + keys.Length);
                    Type constructedType = tupleType.MakeGenericType(keys);
                    return Activator.CreateInstance(constructedType, keys.Select(x => GetValue(x, depth + 1)).ToArray());

                }
                else if (type.Name == "IDictionary`2")
                {
                    var itemTypes = type.GetGenericArguments();
                    var dicType = typeof(Dictionary<,>);
                    var constructedDicType = dicType.MakeGenericType(itemTypes);
                    IDictionary dic = Activator.CreateInstance(constructedDicType) as IDictionary;
                    object key = GetValue(itemTypes[0], depth + 1);
                    object value = GetValue(itemTypes[1], depth + 1);
                    if (key != null && value != null)
                        dic.Add(key, value);
                    return dic;
                }
                else if (type.Name == "IEnumerable`1" || type.Name == "IList`1")
                {
                    var itemType = GetType(type.GetGenericArguments()[0]);
                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(itemType);
                    IList list = Activator.CreateInstance(constructedListType) as IList;
                    object val = GetValue(itemType, depth + 1);
                    if (val != null)
                        list.Add(val);
                    return list;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return Activator.CreateInstance(type);
                }
                else if (type == typeof(object))
                {
                    return new BHoMObject { Name = "test" };
                }
                else if (type == typeof(Type))
                {
                    return typeof(BHoMObject);
                }
                else if (type == typeof(MethodBase))
                {
                    return typeof(BH.Engine.Reflection.Query).GetMethods().First();
                }
                else if (type.IsInterface || type.IsAbstract)
                {
                    if (depth > 50 || !m_ImplementingTypes.ContainsKey(type))
                        return null;
                    else
                        return GetValue(m_ImplementingTypes[type], depth + 1);
                }
                else if (type == typeof(DateTimeOffset))
                {
                    return new DateTimeOffset(636694884850000000, new TimeSpan());
                }
                else
                {
                    if (depth > 20) return null;
                    return InitialiseObject(type, depth + 1);
                }
            }
            catch (Exception e)
            {
                Reflection.Compute.RecordWarning("Failed to generate value for type " + type.FullName);
                return null;
            }
        }

        /***************************************************/

        private static object CreateImmutable(Type type, int depth)
        {
            ConstructorInfo ctor = type.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
            object[] parameters = ctor.GetParameters().Select(x => GetValue(x.ParameterType, depth)).ToArray();
            return ctor.Invoke(parameters);
        }

        /***************************************************/

        private static Type GetType(Type type)
        {
            try
            {
                if (type.Name.StartsWith("IComparable`1"))
                    return typeof(int);
                else if (type.IsInterface || type.IsAbstract)
                    return m_ImplementingTypes[type];
                else if (type.ContainsGenericParameters)
                {
                    List<Type> actuals = new List<Type>();
                    foreach (Type generic in type.GetGenericArguments())
                    {
                        if (generic.GetGenericParameterConstraints().Count() > 0)
                            actuals.Add(GetType(generic.GetGenericParameterConstraints()[0]));
                        else
                            actuals.Add(typeof(object));
                    }

                    return type.MakeGenericType(actuals.ToArray());
                }
                else
                    return type;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /***************************************************/

        private static void LinkInterfaces(List<Type> types)
        {
            foreach (Type type in types)
            {
                try
                {
                    if (!type.IsAbstract && !type.IsInterface && !type.IsEnum)
                    {
                        foreach (Type inter in type.GetInterfaces())
                        {
                            if (!m_ImplementingTypes.ContainsKey(inter))
                                m_ImplementingTypes[inter] = type;
                        }

                        Type baseType = type.BaseType;
                        if (baseType != null && !m_ImplementingTypes.ContainsKey(baseType))
                            m_ImplementingTypes[baseType] = type;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        /*******************************************/
        /**** Private fields                    ****/
        /*******************************************/

        private static Dictionary<Type, Type> m_ImplementingTypes = new Dictionary<Type, Type>();

        /***************************************************/
    }
}

