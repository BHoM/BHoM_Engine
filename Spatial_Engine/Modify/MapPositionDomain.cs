/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Maps the positions of a TaperedProfile to a domain of 0 and 1.")]
        [Input("taperedProfile", "The TaperedProfile to modify the position domain.")]
        [Output("taperedProfile", "TaperedProfile with a position domain of 0 and 1")]
        public static TaperedProfile MapPositionDomain(this TaperedProfile taperedProfile)
        {
            TaperedProfile newTaperedProfile = null;
            List<double> positions = new List<double>(taperedProfile.Profiles.Keys);

            if (!positions.Contains(0) && !positions.Contains(1))
            {
                List<double> newPositions = Compute.MapDomain(positions, positions);
                newTaperedProfile = Create.TaperedProfile(newPositions, new List<IProfile>(taperedProfile.Profiles.Values), taperedProfile.InterpolationOrder);
            }
            else
            {
                newTaperedProfile = taperedProfile;
            }

            return newTaperedProfile;
        }
    }
}




