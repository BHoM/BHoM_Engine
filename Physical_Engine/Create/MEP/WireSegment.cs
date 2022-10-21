/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Physical.ConduitProperties;
using BH.oM.Physical.Elements;

namespace BH.Engine.Physical.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a Wire object. Current that flows through this wire can be established at the system level.")]
        [Input("polyline", "A polyline that determines the Wire's length and direction.")]
        [Input("flowRate", "The current carried by the Wire.")]
        [Input("sectionProperty", "Provide a wireSectionProperty to prepare a composite Wire section for accurate capacity and spatial quality.")]
        [Input("orientationAngle", "This is the wire's planometric orientation angle (the rotation around its central axis created about the profile centroid).")]
        [Output("wireSegment", "Wire object to work within an MEP systems.")]
        public static WireSegment WireSegment(Polyline polyline, double flowRate = 0, WireSectionProperty sectionProperty = null, double orientationAngle = 0)
        {
            if (polyline == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a wire segment from an empty line.");
                return null;
            }

            return new WireSegment
            {
                Location = polyline,
                SectionProperty = sectionProperty,
                OrientationAngle = orientationAngle,
            };
        }
        /***************************************************/
    }
}


