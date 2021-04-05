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

using BH.Engine.Geometry;
using BH.Engine.MEP;
using BH.Engine.Spatial;
using BH.oM.MEP.System;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.MEP.System.SectionProperties;
using BH.oM.MEP.Enums;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /******************************************/
        /****            Public Methods        ****/
        /******************************************/

        [Description("Gets the area of an IProfile. This assumes that the outermost curve(s) are solid. Curves inside a solid region are assumed to be openings, and curves within openings are assumed to be solid, etc. Also, for TaperedProfiles, the average area is returned.")]
        [Input("obj", "The object to evaluate.")]
        [Output("area", "The net area of the solid regions in the profile", typeof(Area))]
        public static double Area(this IFlow obj)
        {
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

            return area;

            /******************************************/
        }
    }
}

