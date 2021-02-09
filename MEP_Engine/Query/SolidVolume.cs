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

using BH.Engine.Spatial;
using BH.oM.MEP.System;
using BH.oM.Reflection.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Dimensional;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the solid volume of an object by multiplying the section profile's solid area by the element's length. Note this element may contain a composite section of multiple areas, but this query method returns a single summed value. For section specific volumes, please use GetSectionProfile and Query the Areas manually.")]
        [Input("obj", "The object to query solid volume.")]
        [Output("solidVolume", "Combined SolidVolume of the Element's SectionProfiles.")]

        public static double SolidVolume(this IFlow obj)
        {
            double length = (obj as IElement1D).Length();
            double area = 0;

            List<IProfile> profiles = GetSectionProfiles(obj);

            if (profiles.Count() <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The object must contain at least one section profile to be evaluated.");
                return double.NaN;
            }

            for (int i = 0; i < profiles.Count(); i++)
            {
                area = profiles.Select(x => x.Area()).Sum();
            }

            if (length <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The object has no length. Returning Double NaN.");
                return double.NaN;
            }

            return (length * area);
        }

        /***************************************************/
    }
}

