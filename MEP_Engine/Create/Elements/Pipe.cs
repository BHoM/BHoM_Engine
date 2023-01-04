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

using System.ComponentModel;
using BH.oM.MEP.System.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a Pipe object. Material that flows through this Pipe can be established at the system level.")]
        [Input("line", "A line that determines the Pipe's length and direction.")]
        [Input("flowRate", "The volume of fluid being conveyed by the Pipe per second (m3/s).")]
        [Input("sectionProperty", "Provide a pipeSectionProperty to prepare a composite Pipe section for accurate capacity and spatial quality.")]
        [Output("pipe", "A pipe object is a passageway which conveys material (water, waste, glycol).")]

        public static BH.oM.MEP.System.Pipe Pipe(Line line, double flowRate = 0, PipeSectionProperty sectionProperty = null)
        {
            if (line == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a pipe from an empty line.");
                return null;
            }

            return new BH.oM.MEP.System.Pipe
            {
                StartPoint = line.Start,
                EndPoint = line.End,
                SectionProperty = sectionProperty,
            };
        }
        /***************************************************/
    }
}



