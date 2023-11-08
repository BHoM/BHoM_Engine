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
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Ground;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Ground
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a reference object for a stratum.")]
        [Input("remarks", "General remarks for the investigation (GEOL_REF).")]
        [Input("lexiconCode", "BGS Lexicon code for the strata (GEOL_BGS).")]
        [Input("strataName", "Geological formation or strata name.")]
        [Input("Files", "Associated file reference including instructions and photographs (FILE_FSET).")]
        [Output("reference", "References to associated files, storage links or previous boreholes.")]
        public static StratumReference StratumReference(string remarks = "", string lexiconCode = "", string strataName = "", string files = "")
        {
            if(remarks.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The remarks input is null or empty.");
                return null;
            }
            else if (lexiconCode.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The lexicon code is null or empty.");
                return null;
            }
            else if (strataName.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The strata name is null or empty.");
                return null;
            }

            StratumReference reference = new StratumReference()
            {
                Remarks = remarks,
                LexiconCode = lexiconCode,
                Files = files
            };

            reference.Name = strataName;

            return reference;
        }

        /***************************************************/
    }
}




