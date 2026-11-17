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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Classification
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("10.0", "BH.Engine.Adapters.OneClickLCA.Create.RICSCategory(System.String)")]
        [Description("Creates a RICS category based on its code.")]
        [Input("code", "Code of the category, for example 2.3 for Roof.")]
        [Output("category", "RICS category represented by an enum. Undefined if no category has that code.")]
        public static RICSCategory RICSCategory(string code)
        {
            if (code == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a RICS category from a null code.");
                return default(RICSCategory);
            }

            code = code.Replace(".", "_").Trim(new char[] { ' ', '_' });
            if (string.IsNullOrEmpty(code))
            {
                BH.Engine.Base.Compute.RecordWarning("Cannot create a RICS category from an empty code. Undefined category returned.");
                return default(RICSCategory);
            }

            code = "_" + code;

            // Reuse the result if this code has been resolved before
            RICSCategory category;
            if (m_RICSCategoriesByCode.TryGetValue(code, out category))
                return category;

            // The first category (in declaration order) whose name starts with the code is returned
            IEnumerable<RICSCategory> allCategories = Enum.GetValues(typeof(RICSCategory)).OfType<RICSCategory>();
            category = allCategories.FirstOrDefault(x => x.ToString().StartsWith(code));

            // Only matches are stored, which keeps the dictionary limited to valid codes however many different strings are provided
            if (category != default(RICSCategory))
                m_RICSCategoriesByCode[code] = category;

            return category;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        // Thread-safe, filled on demand by RICSCategory. Keyed on the normalised code, for example _2_1_2
        private static readonly ConcurrentDictionary<string, RICSCategory> m_RICSCategoriesByCode = new ConcurrentDictionary<string, RICSCategory>();

        /***************************************************/
    }
}
