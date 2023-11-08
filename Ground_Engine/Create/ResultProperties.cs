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
        [Input("type", "The result type for the contaminant sample (ERES_RTCD).")]
        [Input("reportable", "Is the result reportable (ERES_RRES).")]
        [Input("detectFlag", "Detect flag (ERES_DETF).")]
        [Input("organic", "Is the contaminant sample organic (ERES_ORG).")]
        [Output("resultProperties", "Result properties related to the contaminant sample.")]
        public static ResultProperties ResultProperties(string type, bool reportable, string detectFlag, bool organic)
        {
            if (type.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The type input is null or empty.");
                return null;
            }

            return new ResultProperties() { Type = type, Reportable = reportable, DetectFlag = detectFlag, Organic = organic };
        }

        /***************************************************/
    }
}




