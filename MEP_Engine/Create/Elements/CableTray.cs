/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.MEP.Elements;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.SectionProperties;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a Cable Tray object.")]
        [Input("line", "A line that determines the Cable Tray's length and direction.")]        
        [Input("sectionProperty", "Provide a cableTraySectionProperty to prepare a composite Cable Tray section for accurate capacity and spatial quality.")]
        [Input("orientationAngle", "This is the Cable Tray's planometric orientation angle (the rotation around its central axis created about the profile centroid).")]        
        [Output("cableTray", "A Cable Tray object is a passageway which conveys material (typically cables)")]
        public static CableTray CableTray(Line line, CableTraySectionProperty sectionProperty = null, double orientationAngle = 0)
        {
            return new CableTray
            {
                StartNode = (Node)line.Start,
                EndNode = (Node)line.End,
                SectionProperty = sectionProperty,
                OrientationAngle = orientationAngle,
            };
        }
        /***************************************************/
    }
}
