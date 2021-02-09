/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using BH.oM.MEP.System.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.System;
using System.Collections.Generic;
using BH.oM.MEP.System.ConnectionProperties;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a cableTray object. Material that flows through this cableTray can be established at the system level.")]
        [Input("line", "A line that determines the cableTray's length and direction.")]
        [Input("sectionProfile", "Provide a sectionProfile to prepare a composite cableTray section for accurate capacity and spatial quality.")]
        [Input("connectionProperty", "The connection properties of the element.")]
        [Output("cableTray", "A CableTray object is a passageway which conveys material (typically cables).")]

        public static CableTray cableTray(Line line, List<SectionProfile> sectionProfile = null, ConnectionProperty connectionProperty = null)
        {
            return new CableTray
            {
                StartPoint = (Node)line.Start,
                EndPoint = (Node)line.End,
                ConnectionProperty = connectionProperty,
                SectionProfile = sectionProfile,
            };
        }
        /***************************************************/
    }
}

