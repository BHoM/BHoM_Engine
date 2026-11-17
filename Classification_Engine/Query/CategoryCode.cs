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

using BH.oM.Base.Attributes;
using BH.oM.Classification.Attributes;
using BH.oM.Classification.Element;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Classification
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("10.0", "BH.Engine.Adapters.OneClickLCA.Query.CategoryCode(BH.oM.Adapters.OneClickLCA.RICSCategory)")]
        [Description("Gets the code of the provided RICS category, for example 2.1.2.")]
        [Input("category", "RICS category to get the code from.")]
        [Output("code", "Code of the category, or an empty string if the category has none.")]
        public static string CategoryCode(this RICSCategory category)
        {
            FieldInfo fi = typeof(RICSCategory).GetField(category.ToString());
            if (fi == null)
                return "";

            CategoryCodeAttribute[] attributes = fi.GetCustomAttributes(typeof(CategoryCodeAttribute), false) as CategoryCodeAttribute[];

            if (attributes != null && attributes.Length > 0)
                return attributes.First().Code;
            else
                return "";
        }

        /***************************************************/
    }
}
