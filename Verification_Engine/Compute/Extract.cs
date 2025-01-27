/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Verification.Extraction;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Extracts the objects from the input set based on the extraction instruction." +
            "\nExtraction means either simple filtering from the input set or generation of derived objects based on the input set.")]
        [Input("objects", "Set of objects to extract from.")]
        [Input("extraction", "Instruction how to extract the objects from the input set.")]
        [Output("extracted", "Collection of objects extracted from the input set based on the extraction instruction.")]
        public static List<object> IExtract(this IEnumerable<object> objects, IExtraction extraction)
        {
            if (objects == null)
            {
                BH.Engine.Base.Compute.RecordError($"Extraction failed because the provided objects to extract from are null.");
                return null;
            }

            if (extraction == null)
            {
                BH.Engine.Base.Compute.RecordNote("No filter provided, all input objects have been verified against the requirements.");
                return objects.ToList();
            }

            object filtered;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(objects, nameof(Extract), new object[] { extraction }, out filtered))
            {
                BH.Engine.Base.Compute.RecordError($"Extraction failed because extraction type {extraction.GetType().Name} is currently not supported.");
                return null;
            }

            return filtered as List<object>;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Extracts the objects from the input set based on the condition based filter.")]
        [Input("objects", "Set of objects to extract from.")]
        [Input("extraction", "Condition based filter, i.e. set of conditions for each object to pass in order to be returned.")]
        [Output("extracted", "Subset of the input objects that passed the conditions embedded in the input filter.")]
        public static List<object> Extract(this IEnumerable<object> objects, ConditionBasedFilter extraction)
        {
            return objects.Where(x => x.IPasses(extraction.Condition) == true).ToList();
        }

        /***************************************************/
    }
}

