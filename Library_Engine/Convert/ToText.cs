/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using BH.oM.Reflection.Attributes;
using BH.oM.Data.Library;
using BH.oM.Base;

namespace BH.Engine.Library
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract all set properties from a source and generates a line separated string of the information.")]
        [Input("source", "Source to turn to text information.")]
        [Output("sourceText", "Text representation of the source.")]
        public static string ToText(this Source source)
        {
            if(source == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot convert a null source object to a string.");
                return "";
            }

            string sourceDesc = "";
            IEnumerable<PropertyInfo> properties = source.GetType().GetProperties().Where(x => x.PropertyType == typeof(string));

            foreach (PropertyInfo prop in properties)
            {
                string sourceText = prop.GetGetMethod().Invoke(source, null) as string;
                if (string.IsNullOrWhiteSpace(sourceText))
                    continue;

                sourceDesc += prop.Name + ": " + sourceText + Environment.NewLine;
            }

            return sourceDesc;
        }

        /***************************************************/
    }
}

