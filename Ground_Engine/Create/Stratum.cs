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

        [Description("Creates a Stratum element based on its strata, descriptions and optional geological properties. The lists must be of equal length.")]
        [Input("strataTop", "Depth to the top of the strata based on the datum provided on the Borehole (GEOL_TOP).")]
        [Input("strataBottom", "Depth to the bottom of the strata based on the datum provided on the Borehole (GEOL_BOT).")]
        [Input("logDescription", "General descriptions for each strata (GEOL_DESC).")]
        [Input("legend", "Legend codes summarising the LogDescription (GEOL_LEG).")]
        [Input("observedGeology", "Observed geologies expressed as a GeologicalUnit (GEOL_GEOL).")]
        [Input("interpretedGeology", "Interpreted geologies expressed as an EngineeringMaterial (GEOL_GEO2).")]
        [Input("optionalInterpretedGeology", "The optional interpreted geology expressed as an EngineeringMaterial(GEOL_GEO3).")]
        [Input("blankGeology", "The geology to use where blank spaces occur in the observedGeology parameter..")]
        [Output("geology", "Stratum object containing information for each strata including descriptions, legend codes and optional geological properties.")]
        public static Stratum Stratum(double top, double bottom, string logDescription, int legend,
            string observedGeology, string interpretedGeology = null, string optionalInterpretedGeology = null, string blankGeology = "")
        {
            if (logDescription.Trim() == "")
            {
                Base.Compute.RecordError("The LogDescription is empty.");
                return null;
            }

            if (!blankGeology.Trim().IsNullOrEmpty())
            {
                if (observedGeology.Trim().IsNullOrEmpty())
                    observedGeology = blankGeology;
            }

            return new Stratum()
            {
                Top = top,
                Bottom = bottom,
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




