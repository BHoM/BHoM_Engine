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
using System.Collections.Concurrent;
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

        [PreviousVersion("10.0", "BH.Engine.Adapters.OneClickLCA.Query.CategoryForLevel(BH.oM.Adapters.OneClickLCA.RICSCategory, System.Int32)")]
        [Description("Gets the RICS category that corresponds to the provided level, by rolling the category up to the coarser category whose code it starts with. If the category is already at or above the requested level, it is returned unchanged.")]
        [Input("category", "Original RICS category.")]
        [Input("level", "Level to roll the category up to, between 1 and 3.")]
        [Output("category", "RICS category for the provided level.")]
        public static RICSCategory CategoryForLevel(this RICSCategory category, int level)
        {
            if (level < 1 || level > 3)
            {
                BH.Engine.Base.Compute.RecordError("The category level should be between 1 and 3.");
                return category;
            }

            if (category == RICSCategory.Undefined)
                return category;

            // The roll up is done the first time a category and level combination is requested, and then reused
            return m_RICSCategoriesForLevel.GetOrAdd((category, level), RollUpToLevel);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static RICSCategory RollUpToLevel((RICSCategory category, int level) key)
        {
            RICSCategory category = key.category;
            int level = key.level;

            int currentLevel = category.CategoryLevel();
            string code = category.CategoryCode();

            while (currentLevel > level)
            {
                string[] split = code.Split('.');
                code = string.Join(".", split.Take(split.Length - 1));
                category = Create.RICSCategory(code);
                currentLevel = category.CategoryLevel();
            }

            return category;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        // Thread-safe, filled on demand by CategoryForLevel. Only valid levels (1 to 3) are ever stored, so the size is limited
        private static readonly ConcurrentDictionary<(RICSCategory category, int level), RICSCategory> m_RICSCategoriesForLevel = new ConcurrentDictionary<(RICSCategory category, int level), RICSCategory>();

        /***************************************************/
    }
}
