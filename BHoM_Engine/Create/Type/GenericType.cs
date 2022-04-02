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

        [Description("Creates a generic BHoM type that matches the given name.")]
        [Input("name", "Name to be searched for among all BHoM generic types.")]
        [Input("silent", "If true, the error about no type found will be suppressed, otherwise it will be raised.")]
        [Output("type", "BHoM generic type that matches the given name.")]
        public static Type GenericType(string name, bool silent = false)
        {
            if (string.IsNullOrWhiteSpace(name))
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
    }
}

