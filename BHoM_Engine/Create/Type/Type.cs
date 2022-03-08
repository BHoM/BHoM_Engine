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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a BHoM type that matches the given name.")]
        [Input("name", "Name to be searched for among all BHoM types.")]
        [Input("silent", "If true, the error about no type found will be suppressed, otherwise it will be raised.")]
        [Output("type", "BHoM type that matches the given name.")]
        [PreviousVersion("5.1", "BH.Engine.Reflection.Create.Type(System.String, System.Boolean)")]
        public static Type Type(string name, bool silent = false)
        {
            if (name == null)
            {
                Compute.RecordError("Cannot create a type from a null string.");
                return null;
            }

            Dictionary<string, List<Type>> typeDictionary = Query.BHoMTypeDictionary();

            if (name.Contains('<'))
                return GenericType(name, silent);

            List<Type> types = null;
            if (!typeDictionary.TryGetValue(name, out types))
            {
                Type type = System.Type.GetType(name);
                if (type == null && name.EndsWith("&"))
                {
                    type = Type(name.TrimEnd(new char[] { '&' }), true);
                    if (type != null)
                        type = type.MakeByRefType();
                }
                    

                if (type == null && !silent)
                    Compute.RecordError($"A type corresponding to {name} cannot be found.");

                return type;
            }
            else if (types.Count == 1)
                return types[0];
            else if (!silent)
            {
                string message = "Ambiguous match: Multiple types correspond the the name provided: \n";
                foreach (Type type in types)
                    message += "- " + type.FullName + "\n";

                Compute.RecordError(message);
            }

            return null;
        }

        /***************************************************/

        [Description("Creates a generic BHoM type that matches the given name.")]
        [Input("name", "Name to be searched for among all BHoM generic types.")]
        [Input("silent", "If true, the error about no type found will be suppressed, otherwise it will be raised.")]
        [Output("type", "BHoM generic type that matches the given name.")]
        [PreviousVersion("5.1", "BH.Engine.Reflection.Create.GenericType(System.String, System.Boolean)")]
        public static Type GenericType(string name, bool silent = false)
        {
            if (name == null)
            {
                Compute.RecordError("Cannot create a type from a null string.");
                return null;
            }

            //Looping through the string finding generic argument start and end
            int counter = 0;
            List<int> argsSplit = new List<int>();
            int genericCounter = 0; //Counter of how deep into the generic hiearchy the loop currently is. For checking generics of generics
            foreach (char c in name)
            {
                if (c == '<')   //Generic start char
                {
                    if (genericCounter == 0)    //If starting generic bracket, add as first splitpoint for the string
                        argsSplit.Add(counter);

                    genericCounter++;   //Increment the generic counter
                }
                else if (c == '>')  //Generic end char
                {
                    genericCounter--;   //Reduce the generic counter
                    if (genericCounter == 0)    //If counter is 0, closing bracket should correspond to the first generic and split point added
                        argsSplit.Add(counter);
                }
                else if (genericCounter == 1 && c == ',')   //If in the first level of generic heirachy, check for commas to add as additional splitting
                {
                    argsSplit.Add(counter);
                }
                counter++;
            }

            if (argsSplit.Count == 0)   //No split chars found, return null and error as type is not generic
            {
                Compute.RecordError("Provided string is not a generic type.");
                return null;
            }

            List<string> arguments = new List<string>();

            for (int i = 0; i < argsSplit.Count - 1; i++)
            {
                //Generic arguments will be defined between each split char
                //Starting point at position of char + 1, length is difference between the position of split chars - 1
                //Trim to remove starting and trailing whitespace
                arguments.Add(name.Substring(argsSplit[i] + 1, argsSplit[i + 1] - argsSplit[i] - 1).Trim());    
            }
            //Main type definition as string up until the first split char (first '<').
            //Number of generic arguments will be 1 less than the number of argsSplit count
            Type typeDefinition = Type(name.Substring(0, argsSplit[0]) + "`" + (argsSplit.Count - 1));
            if (typeDefinition == null)
                return null;

            try
            {
                return typeDefinition.MakeGenericType(arguments.Select(x => Type(x)).ToArray());    //Call to Type(x) will recursively call this method for inner generic types
            }
            catch
            {
                return null;
            }
        }

        /***************************************************/

        [Description("Creates an Engine type that matches the given name.")]
        [Input("name", "Name to be searched for among all Engine types.")]
        [Input("silent", "If true, the error about no type found will be suppressed, otherwise it will be raised.")]
        [Output("type", "BHoM Engine type that matches the given name.")]
        [PreviousVersion("5.1", "BH.Engine.Reflection.Create.EngineType(System.String, System.Boolean)")]
        public static Type EngineType(string name, bool silent = false)
        {
            if (name == null)
            {
                Compute.RecordError("Cannot create a type from a null string.");
                return null;
            }

            List<Type> methodTypeList = Query.EngineTypeList();

            List<Type> types = methodTypeList.Where(x => x.AssemblyQualifiedName.StartsWith(name)).ToList();

            if (types.Count == 1)
                return types[0];
            else
            {
                //Unique method not found in list, check if it can be extracted using the system Type
                Type type = System.Type.GetType(name, silent);
                if (type == null && !silent)
                {
                    if (types.Count == 0)
                        Compute.RecordError($"A type corresponding to {name} cannot be found.");
                    else
                    {
                        string message = "Ambiguous match: Multiple types correspond the the name provided: \n";
                        foreach (Type t in types)
                            message += "- " + t.FullName + "\n";

                        message += "To get a Engine type from a specific Assembly, try adding ', NameOfTheAssmebly' at the end of the name string, or use the AllEngineTypes method to retreive all the types.";

                        Compute.RecordError(message);
                    }
                }

                return type;
            }
        }

        /***************************************************/
    }
}

