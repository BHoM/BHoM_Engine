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

        [Description("Creates a Geology element based on its strata, descriptions and optional geological properties.")]
        [Input("strataTop", "A list of depths to the top of the strata based on the datum provided on the Borehole (GEOL_TOP).")]
        [Input("strataBottom", "A list of depths to the bottom of the strata based on the datum provided on the Borehole (GEOL_BOT).")]
        [Input("logDescription", "A list of general descriptions for each strata (GEOL_DESC).")]
        [Input("legend", "A list of legend codes summarising the LogDescription (GEOL_LEG).")]
        [Input("observedGeology", "A list of the observed geologies expressed as a GeologicalUnit (GEOL_GEOL).")]
        [Input("interpretedGeology", "A list of the interpreted geologies expressed as an EngineeringMaterial (GEOL_GEO2).")]
        [Input("optionalInterpretedGeology", "The optional interpreted geology expressed as an EngineeringMaterial(GEOL_GEO3).")]
        [Input("blankGeology", "The geology to use where blank spaces occur in the observedGeology parameter..")]
        [Output("geology", "Geology object containing information for each strata including descriptions, legend codes and optional geological properties.")]
        public static GeologicalReference GeologicalReference(List<string> remarks, List<string> lexiconCodes, List<string> stratumNames, List<string> Files = null)
        {
            if (remarks.Count != lexiconCodes.Count || remarks.Count != stratumNames.Count)
            {
                Base.Compute.RecordError("The list lengths for remarks, lexicon codes and stratum names must be equal.");
                return null;
            }

            if(remarks.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The remarks input is null or empty.");
                return null;
            }
            else if (lexiconCodes.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The lexicon codes input is null or empty.");
                return null;
            }
            else if (stratumNames.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The stratum names input is null or empty.");
                return null;
            }

            return new GeologicalReference()
            {
                Remarks = remarks,
                LexiconCodes = lexiconCodes,
                StratumNames = stratumNames,
                Files = Files
            };
        }

        /***************************************************/
    }
}




