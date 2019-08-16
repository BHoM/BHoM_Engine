/*
 * parameters file is part of the Buildings and Habitats object Model (BHoM)
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
 * along with parameters code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
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
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a single TierProfile from a single ProfileParameters and setting out point. 0,0,0 is used as the focal point.")]
        [Input("parameters", "ProfileParameters")]
        [Input("lastPointPrevTier", "Spectator eye point from previous tier or 0,0,0 on first tier")]
        public static TierProfile TierProfile(ProfileParameters parameters, Point lastPointPrevTier)
        {
            TierProfile tierProfile = new TierProfile();

            SetEyePoints(ref tierProfile, parameters, lastPointPrevTier.X, lastPointPrevTier.Z);

            SectionSurfPoints(ref tierProfile, parameters);

            tierProfile.SectionOrigin = DefineTierOrigin(tierProfile.FloorPoints);

            tierProfile.Profile = Geometry.Create.Polyline(tierProfile.FloorPoints);

            return tierProfile;
        }


        /***************************************************/
        [Description("Scale, rotate and translate a TierProfile")]
        [Input("originalSection", "TierProfile to transform")]
        [Input("scale", "Scaling amount")]
        [Input("source", "Origin point for the transform")]
        [Input("target", "Target point for the transform")]
        [Input("angle", "Rotation angle")]
        //this should be a modify method
        public static TierProfile TransformProfile(TierProfile originalSection, Vector scale, Point source, Point target, double angle)
        {
            TierProfile transformedTier = (TierProfile)originalSection.DeepClone();
            var xScale = Geometry.Create.ScaleMatrix(source, scale);
            var xRotate = Geometry.Create.RotationMatrix(source, Vector.ZAxis, angle);
            var xTrans = Geometry.Create.TranslationMatrix(target-source);
            TransformTier(ref transformedTier, xScale);
            TransformTier(ref transformedTier, xRotate);
            TransformTier(ref transformedTier, xTrans);
            return transformedTier;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static TierProfile MirrorTierYZ(TierProfile originalSection)
        {
            //need a clone
            TierProfile theMappedTier = originalSection.DeepClone();
            double x, y, z;
            for (var p = 0; p < theMappedTier.FloorPoints.Count; p++)
            {

                x = theMappedTier.FloorPoints[p].X;
                y = -theMappedTier.FloorPoints[p].Y;
                z = theMappedTier.FloorPoints[p].Z;
                theMappedTier.FloorPoints[p] = Geometry.Create.Point(x, y, z);

            }
            for (var p = 0; p < theMappedTier.EyePoints.Count; p++)
            {

                x = theMappedTier.EyePoints[p].X;
                y = -theMappedTier.EyePoints[p].Y;
                z = theMappedTier.EyePoints[p].Z;
                theMappedTier.EyePoints[p] = Geometry.Create.Point(x, y, z);

            }
            theMappedTier.Profile.ControlPoints = theMappedTier.FloorPoints;
            theMappedTier.SectionOrigin = DefineTierOrigin(theMappedTier.FloorPoints);
            
            return theMappedTier;

        }
        /***************************************************/
        private static void SetEyePoints(ref TierProfile tierProfile, ProfileParameters parameters, double lastX, double lastZ)
        {
            double prevX, prevZ;
            for (int i = 0; i < parameters.NumRows; i++)
            {
                double x = 0;
                double y = 0;
                double z = 0;
                if (parameters.SuperRiserParameters.SuperRiser && i == parameters.SuperRiserParameters.SuperRiserStartRow)
                {
                    double deltaSHoriz = parameters.EyePositionParameters.StandingEyePositionX - parameters.EyePositionParameters.EyePositionX;//differences between standing and seated eye posiitons
                    double deltaSVert = parameters.EyePositionParameters.StandingEyePositionZ - parameters.EyePositionParameters.EyePositionZ;
                    //shift the previous positions to give standing eye position and add in the super riser specific horiznotal	
                    prevX = tierProfile.EyePoints[i - 1].X - (deltaSHoriz);
                    prevZ = tierProfile.EyePoints[i - 1].Z + (deltaSVert);
                    x = prevX + parameters.EyePositionParameters.StandingEyePositionX + parameters.SuperRiserParameters.SuperRiserKerbWidth + parameters.EyePositionParameters.WheelChairEyePositionX;
                    z = prevZ + parameters.TargetCValue + parameters.RowWidth * ((prevZ + parameters.TargetCValue) / prevX);
                }
                else
                {
                    if (parameters.SuperRiserParameters.SuperRiser && i == parameters.SuperRiserParameters.SuperRiserStartRow + 1)//row after the super riser
                    {
                        //x shifts to include 3 rows for super platform back nib wall nib and row less horiz position
                        //also a wider row is required
                        double widthSp = (2 * parameters.RowWidth) + parameters.SuperRiserParameters.SuperRiserKerbWidth + parameters.RowWidth - parameters.EyePositionParameters.EyePositionX;
                        x = tierProfile.EyePoints[i - 1].X + widthSp;
                        //z is with standard c over the wheel chair posiiton but could be over the handrail
                        z = tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue + widthSp * ((tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue) / tierProfile.EyePoints[i - 1].X);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            x = parameters.StartX + lastX;

                            z = parameters.StartZ + lastZ;
                        }
                        else
                        {
                            x = tierProfile.EyePoints[i - 1].X + parameters.RowWidth;
                            z = tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue + parameters.RowWidth * ((tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue) / tierProfile.EyePoints[i - 1].X);

                        }
                    }

                }
                if (parameters.RiserHeightRounding > 0) z = Math.Round(z / parameters.RiserHeightRounding) * parameters.RiserHeightRounding;
                var p = Geometry.Create.Point(x, y, z);
                tierProfile.EyePoints.Add(p);

            }

        }
        /***************************************************/

        private static void SectionSurfPoints(ref TierProfile tierProfile, ProfileParameters parameters)
        {

            double x = 0; double y = 0; double z = 0;

            Point p;
            for (int i = 0; i < tierProfile.EyePoints.Count; i++)
            {
                if (parameters.SuperRiserParameters.SuperRiser && i == parameters.SuperRiserParameters.SuperRiserStartRow)
                {
                    //4 surface points are needed beneath the wheel chair eye point
                    //p1 x is same as previous
                    z = tierProfile.EyePoints[i - 1].Z - parameters.EyePositionParameters.EyePositionZ + 0.1;//z is previous eye - eyeH + something
                    p = Engine.Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                    //p2 z is the same a sprevious
                    x = x + parameters.SuperRiserParameters.SuperRiserKerbWidth;
                    p = Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                    //p3 x is unchanged
                    z = tierProfile.EyePoints[i].Z - parameters.EyePositionParameters.WheelChairEyePositionZ;
                    p = Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                    //p4 z unchnaged
                    x = x + 3 * parameters.RowWidth;
                    p = Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                }
                else
                {
                    if (parameters.SuperRiserParameters.SuperRiser && i == parameters.SuperRiserParameters.SuperRiserStartRow + 1)//row after the super riser
                    {
                        //4 surface points are needed beneath the wheel chair eye point
                        //p1 x is same as previous
                        z = tierProfile.EyePoints[i].Z - parameters.EyePositionParameters.EyePositionZ+ parameters.BoardHeight;//boardheight is handrail height
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                        //p2 z unchanged
                        x = x + parameters.SuperRiserParameters.SuperRiserKerbWidth;
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                        //p3 x unchanged
                        z = tierProfile.EyePoints[i].Z - parameters.EyePositionParameters.EyePositionZ;
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                        //p4 z unchanged
                        x = tierProfile.EyePoints[i].X + parameters.EyePositionParameters.EyePositionX;
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                    }
                    else
                    {//standard two points per eye
                        for (var j = 0; j < 2; j++)
                        {

                            if (j == 0)
                            {
                                z = tierProfile.EyePoints[i].Z - parameters.EyePositionParameters.EyePositionZ;
                                x = tierProfile.EyePoints[i].X - parameters.RowWidth + parameters.EyePositionParameters.EyePositionX;

                            }
                            else
                            {
                                x = tierProfile.EyePoints[i].X + parameters.EyePositionParameters.EyePositionX;
                            }
                            p = Geometry.Create.Point(x, y, z);
                            tierProfile.FloorPoints.Add(p);

                        }
                    }
                }

            }

        }

        /***************************************************/

        private static ProfileOrigin DefineTierOrigin(List<Point> flrPoints)
        {
            ProfileOrigin profOrigin = Create.ProfileOrigin(flrPoints[0], flrPoints[1] - flrPoints[0]);
            return profOrigin;
        }

        /***************************************************/

        private static void TransformTier(ref TierProfile profile, TransformMatrix xTrans)
        {
            profile.FloorPoints = profile.FloorPoints.Select(p => p.Transform(xTrans)).ToList();
            profile.EyePoints = profile.EyePoints.Select(p => p.Transform(xTrans)).ToList();
            profile.Profile.ControlPoints = profile.FloorPoints;
            profile.FocalPoint = profile.FocalPoint.Transform(xTrans);
            profile.SectionOrigin = DefineTierOrigin(profile.FloorPoints);
        }

        /***************************************************/

        
    }
}
