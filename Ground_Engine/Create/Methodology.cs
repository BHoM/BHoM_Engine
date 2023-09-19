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

        [Description("Creates a Methodology object with properties that can be added to a Borehole object. ")]
        [Input("type", "Type of activity (LOCA_TYPE).")]
        [Input("status", "Status of information relating to this positio (LOCA_STAT).")]
        [Input("remarks", "General remarks for the investigation (LOCA_REM).")]
        [Input("purpose", "Purpose of the activity (LOCA_PURP).")]
        [Input("termination", "Reason for activity termination.")]
        [Output("methodology", "Methodology for the borehole, remarks and comments (e.g. for termination).")]
        public static Methodology Methodology(string type, string status, string remarks, string purpose, string termination)
        {
            return new Methodology()
            {
                Type = type,
                Status = status,
                Remarks = remarks,
                Purpose = purpose,
                Termination = termination
            };
        }

        /***************************************************/
    }
}




