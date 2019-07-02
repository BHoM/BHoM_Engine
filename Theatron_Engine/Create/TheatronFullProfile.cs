/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Theatron.Elements;
using BH.oM.Theatron.Parameters;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters)
        {
            //this assumes no relation with the plan geometry setting out is from the origin
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            Point lastpoint = new Point();

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i == 0 )
                {
                    parameters[i].StartX = parameters[i].RowWidth - parameters[i].EyePositionX;
                }
                TierProfile tierSection = Create.TierProfile(parameters[i], lastpoint);
                fullProfile.BaseTierProfiles.Add(tierSection);

                lastpoint = tierSection.FloorPoints[tierSection.FloorPoints.Count - 1];

            }
            return fullProfile;
        }

        /***************************************************/

        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters, TheatronPlan planGeometry)
        {
            //this assumes no relation with the plan geometry setting out is from the origin
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            Point lastpoint = new Point();
            fullProfile.FocalPoint = planGeometry.CValueFocalPoint;
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i == 0)
                {
                    parameters[i].StartX = planGeometry.MinDistToFocalCurve+parameters[i].RowWidth - parameters[i].EyePositionX;
                }
                TierProfile tierSection = Create.TierProfile(parameters[i], lastpoint);
                fullProfile.TierProfiles.Add(tierSection);

                lastpoint = tierSection.FloorPoints[tierSection.FloorPoints.Count - 1];

            }
            return fullProfile;
        }
    }
}
