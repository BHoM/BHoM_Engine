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

        [PreviousVersion("10.0", "BH.Engine.Adapters.OneClickLCA.Query.CategoryLevel(BH.oM.Adapters.OneClickLCA.RICSCategory)")]
        [Description("Gets the level of the provided RICS category. For RICSCategory the level is the reporting stage the category belongs to: 1 concept, 2 planning and 3 post-completion, with 0 for a grouping that is not itself selectable.")]
        [Input("category", "RICS category to get the level from.")]
        [Output("level", "Level of the category, or -1 if the category has none.")]
        public static int CategoryLevel(this RICSCategory category)
        {
            FieldInfo fi = typeof(RICSCategory).GetField(category.ToString());
            if (fi == null)
                return -1;

            CategoryLevelAttribute[] attributes = fi.GetCustomAttributes(typeof(CategoryLevelAttribute), false) as CategoryLevelAttribute[];

            if (attributes != null && attributes.Length > 0)
                return attributes.First().Level;
            else
                return -1;
        }

        /***************************************************/
    }
}
