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

        [Description("Creates an object containing the detection properties for a contaminant sample.")]
        [Input("detectionLimit", "Reporting detection limit (ERES_RDLM). If no value is assigned, the default value will be 0.")]
        [Input("methodDetectionLimit", "Method detection limit (ERES_MDLM). If no value is assigned, the default value will be 0.")]
        [Input("quantificationLimit", "Quanification limit (ERES_QLM). If no value is assigned, the default value will be 0.")]
        [Input("TICProbability", "Tentatively Identified Compound (TIC) probability (ERES_TICP). If no value is assigned, the default value will be 0.")]
        [Input("TICRetention", "Tentatively Identified Compound (ERES_TICT) retention time. If no value is assigned, the default value will be 0.")]
        [Output("detectionProperties", "Detection properties related to the contaminant sample. If no value is assigned, the default value will be 0.")]
        public static DetectionProperties DetectionProperties(double detectionLimit = 0, double methodDetectionLimit = 0, double quantificationLimit = 0, double TICProbability = 0, double TICRetention = 0)
        {
            if(TICProbability < 0)
            {
                Base.Compute.RecordError("The Tentatively Identified Compound (TIC) probability cannot be less than zero.");
                return null;
            }

            return new DetectionProperties() { DetectionLimit = detectionLimit, MethodDetectionLimit = methodDetectionLimit, QuantificationLimit = quantificationLimit, TICProbability = TICProbability,
            TICRetention = TICRetention};
        }

        /***************************************************/
    }
}




