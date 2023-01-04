/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Reflection.Compute.UnqualifiedName(System.String)")]
        [Description("Obtain the unqualified name for a given qualified name from a type.")]
        [Input("qualifiedName", "The qualified name to query the unqualified name from.")]
        [Output("unqualifiedName", "The unqualified name for the given name.")]
        public static string UnqualifiedName(string qualifiedName)
        {
            if (qualifiedName == null)
            {
                Base.Compute.RecordError("Cannot extract the unqualified name from a null string.");
                return "";
            }

            int openIndex = qualifiedName.IndexOf('[');
            int closeIndex = qualifiedName.LastIndexOf(']');

            if (openIndex < 0 || closeIndex < 0)
                return qualifiedName.Split(',').First();

            string inside = qualifiedName.Substring(openIndex + 1, closeIndex - openIndex - 1);
            List<int> cuts = FindLevelZero(inside, ',', '[', ']');
            List<string> parts = SplitByIndices(inside, cuts);

            for (int i = 0; i < parts.Count; i++)
            {
                parts[i] = UnqualifiedName(parts[i].Trim('[', ']', ' '));
            }

            return qualifiedName.Substring(0, openIndex + 1) + parts.Aggregate((a, b) => a + ',' + b) + "]";
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<int> FindLevelZero(string text, char splitChar, char levelUpChar, char levelDownChar)
        {
            int level = 0;
            List<int> indices = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];
                if (currentChar == levelUpChar)
                    level++;
                else if (currentChar == levelDownChar)
                    level--;
                else if (currentChar == splitChar && level == 0)
                    indices.Add(i);
            }

            return indices;
        }

        private static List<string> SplitByIndices(string text, List<int> indices)
        {
            if (text == null)
            {
                Base.Compute.RecordError("Cannot split a null string.");
                return new List<string>();
            }

            if (indices == null)
            {
                Base.Compute.RecordWarning("The 'indices' input is null and was replaced by an empty list");
                indices = new List<int>();
            }

            int previousIndex = 0;
            List<string> result = new List<string>();

            foreach (int index in indices.OrderBy(x => x))
            {
                result.Add(text.Substring(previousIndex, index - previousIndex));
                previousIndex = index + 1;
            }
            result.Add(text.Substring(previousIndex));

            return result;
        }
    }
}




