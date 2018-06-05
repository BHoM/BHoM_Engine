﻿using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IObject RandomObject(Type type)
        {
            return RandomObject(type, m_RandomSeed.Next());
        }

        /***************************************************/

        public static IObject RandomObject(Type type, int seed)
        {
            // make sure the type inherits from IObject
            if (!typeof(IObject).IsAssignableFrom(type))
            {
                Reflection.Compute.RecordError("RandomObject method can only create type inheriting from IObjects");
                return null;
            }

            // Making sure the interfaces are linked
            if (m_ImplementingTypes.Count == 0)
                LinkInterfaces();

            return InitialiseObject(type, new Random(seed)) as IObject;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void LinkInterfaces()
        {
            List<Type> types = BH.Engine.Reflection.Query.BHoMTypeList();

            foreach (Type type in types)
            {
                try
                {
                    if (!type.IsAbstract && !type.IsInterface && !type.IsEnum)
                    {
                        foreach (Type inter in type.GetInterfaces())
                            m_ImplementingTypes[inter] = type;

                        Type baseType = type.BaseType;
                        if (baseType != null)
                            m_ImplementingTypes[baseType] = type;
                    }
                }
                catch (Exception e)
                {
                    Reflection.Compute.RecordWarning(e.ToString());
                }
            }
        }

        /***************************************************/

        private static object InitialiseObject(Type type, Random rnd, int depth = 0)
        {
            // Create object
            object obj;
            if (type.GetInterface("IImmutable") != null)
                return CreateImmutable(type, rnd, depth);
            else if (type.IsGenericType)
            {
                type = GetType(type);
                obj = Activator.CreateInstance(type);
            }
            else
                obj = Activator.CreateInstance(type);

            // Set its public properties
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanWrite)
                    prop.SetValue(obj, GetValue(prop.PropertyType, rnd, depth));
            }

            return obj;
        }

        /***************************************************/

        private static object CreateImmutable(Type type, Random rnd, int depth)
        {
            ConstructorInfo ctor = type.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
            object[] parameters = ctor.GetParameters().Select(x => GetValue(x.ParameterType, rnd, depth)).ToArray();
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
                            actuals.Add(typeof(Point));
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

        /*******************************************/

        private static object GetValue(Type type, Random rnd, int depth)
        {
            switch (type.Name)
            {
                case "Boolean":
                    return rnd.NextDouble() >= 0.5;
                case "Int32":
                case "Int64":
                    return rnd.Next();
                case "Double":
                    return rnd.NextDouble();
                case "Single":
                    return Convert.ToSingle(rnd.NextDouble());
                case "Char":
                    return (char)('a' + rnd.Next(0, 26));
                case "Guid":
                    return Guid.NewGuid();
                case "String":
                    return Path.GetRandomFileName().Replace(".", "");
                case "Color":
                    return System.Drawing.Color.FromArgb(1, 2, 3, 4);
                case "Dictionary`2":
                    return RandomDictionary(type, rnd, depth);
                case "List`1":
                    return RandomList(type, rnd, depth);
                case "HashSet`1":
                    return RandomHashSet(type, rnd, depth);
                case "Object":
                    return new Point { X = rnd.NextDouble(), Y = rnd.NextDouble(), Z = rnd.NextDouble() };
                case "Type":
                    return typeof(BHoMObject);
            }

            if (type.IsPrimitive)
                return Activator.CreateInstance(type);
            else if (type.IsEnum)
            {
                Array values = Enum.GetValues(type);
                return Enum.GetValues(type).GetValue(rnd.Next(values.Length - 1));
            }
            else if (type.IsArray)
                return RandomArray(type, rnd, depth);
            else if (type.Name.StartsWith("Tuple`"))
                return RandomTuple(type, rnd, depth);
            else if (type.Name == "IEnumerable`1")
                return RandomEnumerable(type, rnd, depth);
            else if (typeof(IEnumerable).IsAssignableFrom(type))
                return Activator.CreateInstance(type);
            else if (type.IsInterface || type.IsAbstract)
            {
                if (depth > 50) return null;
                return GetValue(m_ImplementingTypes[type], rnd, depth + 1);
            }
            else
            {
                if (depth > 50) return null;
                return InitialiseObject(type, rnd, depth + 1);
            }
        }

        /*******************************************/

        private static IDictionary RandomDictionary(Type type, Random rnd, int depth)
        {
            IDictionary dic = Activator.CreateInstance(GetType(type)) as IDictionary;
            Type[] typeArguments = type.GetGenericArguments();
            for (int i = 0; i < rnd.Next(20); i++)
                dic[GetValue(typeArguments[0], rnd, depth + 1)] = GetValue(typeArguments[1], rnd, depth + 1);
            return dic;
        }

        /*******************************************/

        private static Array RandomArray(Type type, Random rnd, int depth)
        {
            ConstructorInfo constructor = type.GetConstructors().First();
            object[] dims = constructor.GetParameters().Select(x => (object)1).ToArray();
            Array array = constructor.Invoke(dims) as Array;

            if (dims.Length == 1)
                array.SetValue(GetValue(type.GetElementType(), rnd, depth + 1), 0);
            else if (dims.Length == 2)
                array.SetValue(GetValue(type.GetElementType(), rnd, depth + 1), 0, 0);
            return array;
        }

        /*******************************************/

        private static IList RandomList(Type type, Random rnd, int depth)
        {
            Type subType = type.GetGenericArguments()[0];
            IList list = Activator.CreateInstance(GetType(type)) as IList;
            for (int i = 0; i < rnd.Next(20); i++)
                list.Add(GetValue(subType, rnd, depth + 1));
            return list;
        }

        /*******************************************/

        private static IEnumerable RandomHashSet(Type type, Random rnd, int depth)
        {
            Type subType = type.GetGenericArguments()[0];
            IEnumerable set = Activator.CreateInstance(GetType(type)) as IEnumerable;
            return set;
        }

        /*******************************************/

        private static object RandomTuple(Type type, Random rnd, int depth)
        {
            Type[] keys = type.GetGenericArguments();
            Type tupleType = Type.GetType("System.Tuple`" + keys.Length);
            Type constructedType = tupleType.MakeGenericType(keys);
            return Activator.CreateInstance(constructedType, keys.Select(x => GetValue(x, rnd, depth + 1)).ToArray()) as object;
        }

        /*******************************************/

        private static IEnumerable RandomEnumerable(Type type, Random rnd, int depth)
        {
            var itemType = GetType(type.GetGenericArguments()[0]);
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(itemType);
            IList list = Activator.CreateInstance(constructedListType) as IList;
            for (int i = 0; i < rnd.Next(20); i++)
                list.Add(GetValue(itemType, rnd, depth + 1));
            return list;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Random m_RandomSeed = new Random();
        private static Dictionary<Type, Type> m_ImplementingTypes = new Dictionary<Type, Type>();

        /***************************************************/
    }
}
