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

using BH.oM.Geometry;
using BH.oM.Humans;
using BH.oM.Humans.BodyParts;
using System.Collections.Generic;

namespace BH.Engine.Humans
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Skeleton Skeleton(Dictionary<JointName, Point> trackingPoints)
        {
            Skeleton skeleton = new Skeleton();

            if (trackingPoints.ContainsKey(JointName.Head))
                skeleton.Head = new Head { TrackingPoint = trackingPoints[JointName.Head] };
            if (trackingPoints.ContainsKey(JointName.WristRight) && trackingPoints.ContainsKey(JointName.RightHand))
                skeleton.RightHand = new RightHand { TrackingLine = new Line { Start = trackingPoints[JointName.WristRight], End = trackingPoints[JointName.RightHand] } }; 
            if (trackingPoints.ContainsKey(JointName.WristLeft) && trackingPoints.ContainsKey(JointName.LeftHand))
                skeleton.LeftHand = new LeftHand { TrackingLine = new Line { Start = trackingPoints[JointName.WristLeft], End = trackingPoints[JointName.LeftHand] } }; 
            if (trackingPoints.ContainsKey(JointName.RightHand) && trackingPoints.ContainsKey(JointName.RightThumb))
                skeleton.RightThumb = new RightThumb { TrackingLine = new Line { Start = trackingPoints[JointName.RightHand], End = trackingPoints[JointName.RightThumb] } }; 
            if (trackingPoints.ContainsKey(JointName.LeftHand) && trackingPoints.ContainsKey(JointName.LeftThumb))
                skeleton.LeftThumb = new LeftThumb { TrackingLine = new Line { Start = trackingPoints[JointName.LeftHand], End = trackingPoints[JointName.LeftThumb] } }; 
            if (trackingPoints.ContainsKey(JointName.Head) && trackingPoints.ContainsKey(JointName.SpineShoulder))
                skeleton.Neck = new Neck { TrackingLine = new Line { Start = trackingPoints[JointName.Head], End = trackingPoints[JointName.SpineShoulder] } }; 
            if (trackingPoints.ContainsKey(JointName.SpineShoulder) && trackingPoints.ContainsKey(JointName.RightShoulder))
                skeleton.RightShoulder = new RightShoulder { TrackingLine = new Line { Start = trackingPoints[JointName.SpineShoulder], End = trackingPoints[JointName.RightShoulder] } }; 
            if (trackingPoints.ContainsKey(JointName.LeftShoulder) && trackingPoints.ContainsKey(JointName.SpineShoulder))
                skeleton.LeftShoulder = new LeftShoulder { TrackingLine = new Line { Start = trackingPoints[JointName.SpineShoulder], End = trackingPoints[JointName.LeftShoulder] } }; 
            if (trackingPoints.ContainsKey(JointName.SpineShoulder) && trackingPoints.ContainsKey(JointName.SpineBase))
                skeleton.Spine = new Spine { TrackingLine = new Line { Start = trackingPoints[JointName.SpineShoulder], End = trackingPoints[JointName.SpineBase] } };
            if (trackingPoints.ContainsKey(JointName.SpineBase) && trackingPoints.ContainsKey(JointName.RightHip))
                skeleton.RightHip = new RightHip { TrackingLine = new Line { Start = trackingPoints[JointName.SpineBase], End = trackingPoints[JointName.RightHip] } };
            if (trackingPoints.ContainsKey(JointName.SpineBase) && trackingPoints.ContainsKey(JointName.LeftHip))
                skeleton.LeftHip = new LeftHip { TrackingLine = new Line { Start = trackingPoints[JointName.SpineBase], End = trackingPoints[JointName.LeftHip] } };
            if (trackingPoints.ContainsKey(JointName.RightShoulder) && trackingPoints.ContainsKey(JointName.ElbowRight))
                skeleton.RightUpperArm = new RightUpperArm { TrackingLine = new Line { Start = trackingPoints[JointName.RightShoulder], End = trackingPoints[JointName.ElbowRight] } };
            if (trackingPoints.ContainsKey(JointName.LeftShoulder) && trackingPoints.ContainsKey(JointName.ElbowLeft))
                skeleton.LeftUpperArm = new LeftUpperArm { TrackingLine = new Line { Start = trackingPoints[JointName.LeftShoulder], End = trackingPoints[JointName.ElbowLeft] } };
            if (trackingPoints.ContainsKey(JointName.ElbowRight) && trackingPoints.ContainsKey(JointName.WristRight))
                skeleton.RightLowerArm = new RightLowerArm { TrackingLine = new Line { Start = trackingPoints[JointName.ElbowRight], End = trackingPoints[JointName.WristRight] } };
            if (trackingPoints.ContainsKey(JointName.ElbowLeft) && trackingPoints.ContainsKey(JointName.WristLeft))
                skeleton.LeftLowerArm = new LeftLowerArm { TrackingLine = new Line { Start = trackingPoints[JointName.ElbowLeft], End = trackingPoints[JointName.WristLeft] } };
            if (trackingPoints.ContainsKey(JointName.RightHip) && trackingPoints.ContainsKey(JointName.KneeRight))
                skeleton.RightUpperLeg = new RightUpperLeg { TrackingLine = new Line { Start = trackingPoints[JointName.RightHip], End = trackingPoints[JointName.KneeRight] } };
            if (trackingPoints.ContainsKey(JointName.LeftHip) && trackingPoints.ContainsKey(JointName.KneeLeft))
                skeleton.LeftUpperLeg = new LeftUpperLeg { TrackingLine = new Line { Start = trackingPoints[JointName.LeftHip], End = trackingPoints[JointName.KneeLeft] } };
            if (trackingPoints.ContainsKey(JointName.KneeRight) && trackingPoints.ContainsKey(JointName.AnkleRight))
                skeleton.RightLowerLeg = new RightLowerLeg { TrackingLine = new Line { Start = trackingPoints[JointName.KneeRight], End = trackingPoints[JointName.AnkleRight] } };
            if (trackingPoints.ContainsKey(JointName.KneeLeft) && trackingPoints.ContainsKey(JointName.AnkleLeft))
                skeleton.LeftLowerLeg = new LeftLowerLeg { TrackingLine = new Line { Start = trackingPoints[JointName.KneeLeft], End = trackingPoints[JointName.AnkleLeft] } };
            if (trackingPoints.ContainsKey(JointName.AnkleRight) && trackingPoints.ContainsKey(JointName.RightFoot))
                skeleton.RightFoot = new RightFoot { TrackingLine = new Line { Start = trackingPoints[JointName.AnkleRight], End = trackingPoints[JointName.RightFoot] } };
            if (trackingPoints.ContainsKey(JointName.AnkleLeft) && trackingPoints.ContainsKey(JointName.LeftFoot))
                skeleton.LeftFoot = new LeftFoot { TrackingLine = new Line { Start = trackingPoints[JointName.AnkleLeft], End = trackingPoints[JointName.LeftFoot] } };

            return skeleton;
        }

        /***************************************************/
    }
}





