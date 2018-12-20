/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using Engine_Explore.BHoM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Convert
{
    public static partial class Json 
    {
        public static bool ReadBool(string json)
        {
            return Boolean.Parse(json);
        }

        /**************************************/

        public static char ReadChar(string json)
        {
            return Char.Parse(json);
        }

        /**************************************/

        public static int ReadInt(string json)
        {
            return Int32.Parse(json);
        }

        /**************************************/

        public static double ReadDouble(string json)
        {
            return Double.Parse(json);
        }

        /**************************************/

        public static Guid ReadGuid(string json)
        {
            return Guid.Parse(json);
        }

        /**************************************/

        public static string ReadString(string json)
        {
            return json.Replace("\\\\", "\\").Trim(new char[] { ' ', '\"' });
        }

        /**************************************/

        public static Dictionary<string, T> ReadStringDictionary<T>(string json)
        {
            if (json == "null" || !json.StartsWith("{") || !json.EndsWith("}"))
                return null;

            // Get definition and type name
            Dictionary<string, string> def = GetDefinitionFromJSON(json);

            // Create the output dictionary
            Type type = typeof(T);
            Dictionary<string, T> result = new Dictionary<string, T>();
            foreach (KeyValuePair<string, string> kvp in def)
                result[kvp.Key] = (T)ReadValue(type, kvp.Value);
            return result;
        }

        /**************************************/

        public static List<string> ReadStringArray(string json)
        {
            if (json == "null" || !json.StartsWith("[") || !json.EndsWith("]"))
                return null;

            return GetArrayFromJSON(json);
        }

        /**************************************/

        public static object ReadObject(string json)
        {
            char[] toTrim = { ' ', '\"' };

            if (json == "null")
                return null;

            if (json.StartsWith("["))
                return ReadArray(json);

            if (!json.StartsWith("{"))
                return ReadString(json);

            // Get definition and type name
            Dictionary<string, string> def = GetDefinitionFromJSON(json);
            string typeName = "";
            if (def.TryGetValue("__Type__", out typeName))
                typeName = typeName.Trim(toTrim);
            else
            {
                typeName = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
                def["__Type__"] = typeName;
            }

            // Process separately if this is a dictionary
            if (typeName.StartsWith("System.Collections.Generic.Dictionary"))
                return ReadDictionary(def);

            // Get the type
            Type type = Type.GetType(typeName);
            //if (type == null) type = TypeDictionary.GetType(typeName);  //TODO: Restore TypeDictionary ?
            if (type == null) return null;

            // Create the object and set the properties
            object newObject = Activator.CreateInstance(type, true);
            foreach (KeyValuePair<string, string> kvp in def)
            {
                string propString = kvp.Key.Trim(toTrim);
                string valueString = kvp.Value.Trim(toTrim);
                if (propString.StartsWith("__")) continue;

                PropertyInfo pInfo = newObject.GetType().GetProperty(propString);
                if (pInfo != null)
                {
                    var val = ReadValue(pInfo.PropertyType, valueString);
                    if (val != null)
                        pInfo.SetValue(newObject, val);
                }
            }

            return newObject;
        }


        /**************************************/
        /****  Private Methods             ****/
        /**************************************/

        private static object ReadValue(Type type, string json)
        {
            MethodInfo parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });

            if (type == typeof(string))
                return ReadString(json);
            else if (type.IsValueType && parseMethod != null)
                return parseMethod.Invoke(null, new object[] { json });
            else if (type.IsEnum)
                return Enum.Parse(type, json);
            else if (type.GetInterface("IList") != null)
                return ReadCollection(type, json);
            else
                return ReadObject(json);
        }

        /**************************************/

        private static IList ReadCollection(Type type, string json)
        {
            List<String> jsonList = GetArrayFromJSON(json);

            Type itemType = type.GetElementType();
            if (itemType == null)
            {
                Type[] argTypes = type.GetGenericArguments();
                if (argTypes.Length > 0 && argTypes[0] != null)
                    itemType = argTypes[0];
                else
                    itemType = typeof(object);

            }
            IList collection = Activator.CreateInstance(type, jsonList.Count) as IList;

            for (int i = 0; i < jsonList.Count; i++)
            {
                if (i >= collection.Count)
                    collection.Add(ReadValue(itemType, jsonList[i]));
                else
                    collection[i] = (ReadValue(itemType, jsonList[i]));
            }


            return collection;
        }

        /**************************************/

        private static IList ReadArray(string json)
        {
            List<String> jsonList = GetArrayFromJSON(json);
            return jsonList.Select(x => ReadObject(x)).ToList();
        }

        /**************************************/

        private static IDictionary ReadDictionary(Dictionary<string, string> def)
        {
            char[] toTrim = { ' ', '\"' };

            Type dicType = System.Type.GetType(def["__Type__"].Trim(toTrim));
            Type keyType = dicType.GetGenericArguments()[0];
            Type valueType = dicType.GetGenericArguments()[1];
            IDictionary collection = Activator.CreateInstance(dicType) as IDictionary;

            foreach (KeyValuePair<string, string> kvp in def)
            {
                string keyString = kvp.Key.Trim(toTrim);
                string valueString = kvp.Value.Trim(toTrim);
                if (keyString.StartsWith("_")) continue;

                collection.Add(ReadValue(keyType, keyString), ReadValue(valueType, valueString));
            }
            return collection;
        }


        /**************************************/
        /****  Utility Stuff               ****/
        /**************************************/

        private static Dictionary<string, string> GetDefinitionFromJSON(string json)
        {
            int level = 0;
            string key = "";
            string value = "";
            int i0 = json.IndexOf('{') + 1;
            string inside = json.Substring(i0, json.LastIndexOf('}') - i0);
            i0 = 0;
            int index = 0;
            Dictionary<string, string> definition = new Dictionary<string, string>();
            for (int i = 0; i < inside.Length; i++)
            {
                if (inside[i] == '{' || inside[i] == '[')
                    level++;
                else if (inside[i] == '}' || inside[i] == ']')
                    level--;
                else if (level == 0 && inside[i] == ':')
                {
                    key = inside.Substring(i0, i - i0).Trim().Replace("\"", "");
                    i0 = i + 1;
                }
                else if (level == 0 && inside[i] == ',')
                {
                    value = inside.Substring(i0, i - i0).Trim();
                    definition.Add(key == "" ? index++.ToString() : key, value);
                    i0 = i + 1;
                }
                if (i == inside.Length - 1)
                {
                    value = inside.Substring(i0, i + 1 - i0).Trim();
                    definition.Add(key == "" ? index++.ToString() : key, value);
                    i0 = i + 1;
                }
            }

            return definition;
        }

        /**************************************/

        private static List<String> GetArrayFromJSON(string json)
        {
            int level = 0;
            int i0 = json.IndexOf('[') + 1;
            string inside = json.Substring(i0, json.LastIndexOf(']') - i0);
            i0 = 0;
            List<string> array = new List<string>();
            for (int i = 0; i < inside.Length; i++)
            {
                if (inside[i] == '{' || inside[i] == '[')
                    level++;
                else if (inside[i] == '}' || inside[i] == ']')
                    level--;
                else if (level == 0 && inside[i] == ',')
                {
                    array.Add(inside.Substring(i0, i - i0).Trim());
                    i0 = i + 1;
                }
                if (i == inside.Length - 1)
                {
                    array.Add(inside.Substring(i0, i + 1 - i0).Trim());
                    i0 = i + 1;
                }
            }

            return array;
        }
    }
}
