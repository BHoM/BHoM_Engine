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

        [Description("Creates a Geology element based on its strata, descriptions and optional geological properties. The lists must be of equal length.")]
        [Input("strataTop", "A list of depths to the top of the strata based on the datum provided on the Borehole (GEOL_TOP).")]
        [Input("strataBottom", "A list of depths to the bottom of the strata based on the datum provided on the Borehole (GEOL_BOT).")]
        [Input("logDescription", "A list of general descriptions for each strata (GEOL_DESC).")]
        [Input("legend", "A list of legend codes summarising the LogDescription (GEOL_LEG).")]
        [Input("observedGeology", "A list of the observed geologies expressed as a GeologicalUnit (GEOL_GEOL).")]
        [Input("interpretedGeology", "A list of the interpreted geologies expressed as an EngineeringMaterial (GEOL_GEO2).")]
        [Input("optionalInterpretedGeology", "The optional interpreted geology expressed as an EngineeringMaterial(GEOL_GEO3).")]
        [Input("blankGeology", "The geology to use where blank spaces occur in the observedGeology parameter..")]
        [Output("geology", "Geology object containing information for each strata including descriptions, legend codes and optional geological properties.")]
        public static Geology Geology(List<double> strataTop, List<double> strataBottom, List<string> logDescription, List<int> legend,
            List<string> observedGeology, List<string> interpretedGeology = null, List<string> optionalInterpretedGeology = null, string blankGeology = "")
        {
            // Null checks, list lengths and empty lists
            if (strataTop.IsNullOrEmpty() || strataBottom.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The StrataTop and/or the StataBottom lists are empty or null. ");
                return null;
            }
            else if (strataBottom.Count != strataTop.Count)
            {
                Base.Compute.RecordError("The StrataTop and StrataBotom do not have equal list lengths.");
                return null;
            }

            if (logDescription.IsNullOrEmpty() || legend.IsNullOrEmpty() || observedGeology.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The LogDescription, Legend and ObservedGeology are empty or null.");
                return null;
            }
            else if (logDescription.Count != legend.Count || logDescription.Count != interpretedGeology.Count)
            {
                Base.Compute.RecordError("The LogDescription, Legend and InterpretedGeology do not have equal list lengths.");
                return null;
            }
            else if (strataTop.Count != logDescription.Count)
            {
                Base.Compute.RecordError("The strata depths and log parameters do not have equal list lengths.");
                return null;
            }

            // Check if the top and bottom depths are equal (each strata is sequential)
            for(int i = 1; i < strataTop.Count; i++)
            {
                if(strataTop[i] != strataBottom[i-1])
                {
                    Base.Compute.RecordError("The bottom of the strata does not correspond to the top of the next strata.");
                    return null;
                }
            }

            if (!blankGeology.IsNullOrEmpty())
            {
                for (int i = 0; i < observedGeology.Count; i++)
                {
                    if (observedGeology[i].Trim().IsNullOrEmpty())
                    {
                        observedGeology[i] = blankGeology;
                    }
                }
            }

            return new Geology()
            {
                StrataTop = strataTop,
                StrataBottom = strataBottom,
                LogDescription = logDescription,
                Legend = legend,
                ObservedGeology = observedGeology,
                InterpretedGeology = interpretedGeology,
                OptionalInterpretedGeology = optionalInterpretedGeology
            };

        }

        /***************************************************/
    }
}




