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

using BH.oM.Geometry;
using BH.oM.Architecture.Theatron;
using System;
using BH.Engine.Base;
using System.Linq;
using BH.Engine.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Modify
    {
        [Description("Scale, rotate and translate a TierProfile")]
        [Input("originalSection", "TierProfile to transform")]
        [Input("scale", "Scaling amount")]
        [Input("source", "Origin point for the transform")]
        [Input("target", "Target point for the transform")]
        [Input("angle", "Rotation angle")]
        [PreviousVersion("4.2", "BH.Engine.Architecture.Theatron.Create.TransformProfile(BH.oM.Architecture.Theatron.TierProfile, BH.oM.Geometry.Vector, BH.oM.Geometry.Point, BH.oM.Geometry.Point, System.Double)")]
        public static TierProfile TransformProfile(TierProfile originalSection, Vector scale, Point source, Point target, double angle)
        {
            if (originalSection == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot modify a null tier profile.");
                return originalSection;
            }

            if(scale == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot transform a tier profile using a null scale vector.");
                return originalSection;
            }

            if(source == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot transform a tier profile from a null source point.");
                return originalSection;
            }

            if(target == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot transform a tier profile from a null target point.");
                return originalSection;
            }

            TierProfile transformedTier = (TierProfile)originalSection.DeepClone();
            var xScale = Geometry.Create.ScaleMatrix(source, scale);
            var xRotate = Geometry.Create.RotationMatrix(source, Vector.ZAxis, angle);
            var xTrans = Geometry.Create.TranslationMatrix(target - source);
            TransformTier(ref transformedTier, xScale);
            TransformTier(ref transformedTier, xRotate);
            TransformTier(ref transformedTier, xTrans);
            return transformedTier;
        }

        /***************************************************/

        public static ProfileOrigin DefineTierOrigin(List<Point> flrPoints)
        {
            ProfileOrigin profOrigin = Create.ProfileOrigin(flrPoints[0], flrPoints[1] - flrPoints[0]);
            return profOrigin;
        }

        /***************************************************/

        public static void TransformTier(ref TierProfile profile, TransformMatrix xTrans)
        {
            if(profile == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot modify a null tier profile.");
                return;
            }

            if(xTrans == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot modify a tier profile with a null transformation matrix.");
                return;
            }

            profile.FloorPoints = profile.FloorPoints.Select(p => p.Transform(xTrans)).ToList();
            profile.EyePoints = profile.EyePoints.Select(p => p.Transform(xTrans)).ToList();
            profile.Profile.ControlPoints = profile.FloorPoints;
            profile.FocalPoint = profile.FocalPoint.Transform(xTrans);
            profile.SectionOrigin = DefineTierOrigin(profile.FloorPoints);
        }
    }
}
