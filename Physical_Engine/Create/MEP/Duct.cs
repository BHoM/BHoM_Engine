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
using BH.oM.Physical.Elements;
using BH.oM.Physical.ConduitProperties;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a Duct object. Material that flows through this Duct can be established at the system level.")]
        [Input("polyLine", "A polyLine that determines the Duct's length and direction.")]
        [Input("flowRate", "The volume of fluid being conveyed by the Duct per second (m3/s).")]
        [Input("sectionProperty", "Provide a ductSectionProperty to prepare a composite Duct section for accurate capacity and spatial quality.")]
        [Input("orientationAngle", "This is the Duct's planometric orientation angle (the rotation around its central axis created about the profile centroid).")]
        [Output("duct", "A duct object is a passageway which conveys material (typically air).")]
        public static Duct Duct(Polyline polyLine, double flowRate = 0, DuctSectionProperty sectionProperty = null, double orientationAngle = 0)
        {
            if(polyLine == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a duct from an empty line.");
                return null;
            }
            
            return new Duct
            {
               Location = polyLine,
                SectionProperty = sectionProperty,
                OrientationAngle = orientationAngle,
            };
        }
        /***************************************************/
    }
}


