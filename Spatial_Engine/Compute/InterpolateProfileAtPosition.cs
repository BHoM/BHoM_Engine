/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Interpolates an IProfile at a given position. Non TaperedProfiles will be returned as they are constant and \n" +
            "and TaperedProfiles will be interpolated using their positions and profiles.")]
        [Input("profile", "The profile to interpolate.")]
        [Input("position", "The parametric position (between zero and one) to interpolate the profile.")]
        [Output("interpolatedProfile", "The IProfile interpolated at the given position.")]
        public static IProfile InterpolateProfileAtPosition(IProfile profile, double position)
        {
            if (!(profile is TaperedProfile))
                return profile;

            TaperedProfile taperedProfile = profile as TaperedProfile;

            List<double> positions = taperedProfile.Profiles.Keys.ToList();
            List<IProfile> profiles = taperedProfile.Profiles.Values.ToList();

            if (taperedProfile.Profiles.ContainsKey(position))
                return taperedProfile.Profiles[position];

            for (int i = 0; i < positions.Count - 1; i++)
            {
                if (position >= positions[i] && position <= positions[i + 1])
                {
                    double tLocal = (position - positions[i]) / (positions[i + 1] - positions[i]);
                    int order = taperedProfile.InterpolationOrder[i];
                    return IInterpolateProfile(profiles[i], profiles[i + 1], tLocal, order);
                }
            }

            Base.Compute.RecordError("Position is outside the range of the TaperedProfile.");
            return null;
        }
    }
}






