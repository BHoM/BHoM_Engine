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

