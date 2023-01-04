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

using BH.oM.Geometry;
using BH.oM.Humans;
using System.Collections.Generic;


namespace BH.Engine.Humans
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, Line> TrackingLines(this Skeleton skeleton)
        {
            if(skeleton == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the tracking lines of a null skeleton.");
                return null;
            }

            Dictionary<string, Line> lines = new Dictionary<string, Line>
            {
                { "Neck", skeleton.Neck.TrackingLine },
                { "RightShoulder", skeleton.RightShoulder.TrackingLine },
                { "LeftShoulder", skeleton.LeftShoulder.TrackingLine },
                { "UppperArmRight", skeleton.RightUpperArm.TrackingLine },
                { "LeftUpperArm", skeleton.LeftUpperArm.TrackingLine },
                { "RightLowerArm", skeleton.RightLowerArm.TrackingLine },
                { "LeftLowerArm", skeleton.LeftLowerArm.TrackingLine },
                { "RightHand", skeleton.RightHand.TrackingLine },
                { "LeftHand", skeleton.LeftHand.TrackingLine },
                { "RightThumb", skeleton.RightThumb.TrackingLine },
                { "LeftThumb", skeleton.LeftThumb.TrackingLine },
                { "Spine", skeleton.Spine.TrackingLine },
                { "RightHip", skeleton.RightHip.TrackingLine },
                { "LeftHip", skeleton.LeftHip.TrackingLine },
                { "RightUpperLeg", skeleton.RightUpperLeg.TrackingLine },
                { "RightLowerLeg", skeleton.RightLowerLeg.TrackingLine },
                { "LeftUpperLeg", skeleton.LeftUpperLeg.TrackingLine },
                { "LeftLowerLeg", skeleton.LeftLowerLeg.TrackingLine },
                { "RightFoot", skeleton.RightFoot.TrackingLine },
                { "LeftFoot", skeleton.LeftFoot.TrackingLine }
            };

            return lines;
        }

        /***************************************************/
    }
}




