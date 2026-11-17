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

            code = "_" + code.Replace(".", "_").Trim(new char[] { ' ', '_' });
            return Enum.GetValues(typeof(RICSCategory)).OfType<RICSCategory>().FirstOrDefault(x => x.ToString().StartsWith(code));
        }

        /***************************************************/
    }
}
