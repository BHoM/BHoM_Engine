/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        [Description("Generate the text representing the input constructor")]
        [Input("type", "The type of object to create a constructor for.")]
        [Input("maxParams", "The maximum number of parameters to include in the text.")]
        [Input("maxChars", "The maximum number of characters for the output text.")]
        [Output("text", "The text corresponding to the description of the constructor generated for that type.")]
        public static string ConstructorText(this Type type, int maxParams = 5, int maxChars = 40)
        {
            string text = type.Namespace + "." + type.Name + "." + type.Name + "() {";

            try
            {
                string[] excluded = new string[] { "BHoM_Guid", "Fragments", "Tags", "CustomData" };
                PropertyInfo[] properties = type.GetProperties().Where(x => !excluded.Contains(x.Name)).ToArray();

                string propertiesText = "";
                if (properties.Length > 0)
                {
                    // Collect parameters text
                    for (int i = 0; i < properties.Count(); i++)
                    {
                        string singlePropertyText = properties[i].PropertyType.ToText() + " " + properties[i].Name;

                        if (i > 0)
                            propertiesText += ", ";

                        if (i >= maxParams || string.Join(propertiesText, singlePropertyText).Length > maxChars)
                        {
                            propertiesText += $"and {properties.Length - i} more inputs";
                            break;
                        }
                        else
                            propertiesText += singlePropertyText;
                    }
                }

                text += propertiesText;
            }
            catch (Exception e)
            {
                Engine.Base.Compute.RecordWarning("Type " + type.Name + " failed to load its properties.\nError: " + e.ToString());
                text += "?";
            }
            text += "}";

            return text;
        }

        /*************************************/
    }
}
