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
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static AngleProfile InterpolateProfile(AngleProfile startProfile, AngleProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.AngleProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd),
                startProfile.MirrorAboutLocalZ, startProfile.MirrorAboutLocalY);
        }

        /***************************************************/

        public static BoxProfile InterpolateProfile(BoxProfile startProfile, BoxProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.BoxProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.OuterRadius, endProfile.OuterRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.InnerRadius, endProfile.InnerRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static ChannelProfile InterpolateProfile(ChannelProfile startProfile, ChannelProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.ChannelProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeWidth, endProfile.FlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd),
                startProfile.MirrorAboutLocalZ);
        }

        /***************************************************/

        public static CircleProfile InterpolateProfile(CircleProfile startProfile, CircleProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.CircleProfile(Interpolate(startProfile.Diameter, endProfile.Diameter, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static FabricatedBoxProfile InterpolateProfile(FabricatedBoxProfile startProfile, FabricatedBoxProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.FabricatedBoxProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.BotFlangeThickness, endProfile.BotFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WeldSize, endProfile.WeldSize, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static FabricatedISectionProfile InterpolateProfile(FabricatedISectionProfile startProfile, FabricatedISectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.FabricatedISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.TopFlangeWidth, endProfile.TopFlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.BotFlangeWidth, endProfile.BotFlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.BotFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WeldSize, endProfile.WeldSize, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static GeneralisedFabricatedBoxProfile InterpolateProfile(GeneralisedFabricatedBoxProfile startProfile, GeneralisedFabricatedBoxProfile endProfile,
            double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            return Create.GeneralisedFabricatedBoxProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.BotFlangeThickness, endProfile.BotFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(Math.Max(startProfile.TopLeftCorbelWidth, startProfile.TopRightCorbelWidth), Math.Max(endProfile.TopLeftCorbelWidth, endProfile.TopRightCorbelWidth), parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(Math.Max(startProfile.BotLeftCorbelWidth, startProfile.BotRightCorbelWidth), Math.Max(endProfile.BotLeftCorbelWidth, endProfile.BotRightCorbelWidth), parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static GeneralisedTSectionProfile InterpolateProfile(GeneralisedTSectionProfile startProfile, GeneralisedTSectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.GeneralisedTSectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.LeftOutstandWidth, endProfile.LeftOutstandWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.LeftOutstandThickness, endProfile.LeftOutstandThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RightOutstandWidth, endProfile.RightOutstandWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RightOutstandThickness, endProfile.RightOutstandThickness, parameter, interpolationOrder, domainStart, domainEnd),
                startProfile.MirrorAboutLocalY);
        }

        /***************************************************/
        public static ISectionProfile InterpolateProfile(ISectionProfile startProfile, ISectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.ISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static KiteProfile InterpolateProfile(KiteProfile startProfile, KiteProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.KiteProfile(
                Interpolate(startProfile.Width1, endProfile.Width1, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Angle1, endProfile.Angle1, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static RectangleProfile InterpolateProfile(RectangleProfile startProfile, RectangleProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.RectangleProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.CornerRadius, endProfile.CornerRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/


        public static TSectionProfile InterpolateProfile(TSectionProfile startProfile, TSectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.TSectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd),
                startProfile.MirrorAboutLocalY);
        }

        /***************************************************/

        public static TubeProfile InterpolateProfile(TubeProfile startProfile, TubeProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.TubeProfile(
                Interpolate(startProfile.Diameter, endProfile.Diameter, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        public static ZSectionProfile InterpolateProfile(ZSectionProfile startProfile, ZSectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.ZSectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeWidth, endProfile.FlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double Interpolate(double startValue, double endValue, double parameter, int interpolationOrder, double domainStartParameter = 0, double domainEndParameter = 1)
        {
            //Check if the interpolation is not required (i.e. the range is constant)
            if (Math.Abs(startValue - endValue) < double.Epsilon)
                return startValue;

            //Scale parameter to the domain
            double scaledParameter = domainStartParameter + (domainEndParameter - domainStartParameter) * parameter;

            //Interpolate between the start and end using the scaled parameter
            double interpolatedValue = endValue + (startValue - endValue) * Math.Pow(1 - scaledParameter, interpolationOrder);

            //Determine the values at the domain start and domain end
            double domainStartValue = endValue + (startValue - endValue) * Math.Pow(1 - domainStartParameter, interpolationOrder);
            double domainEndValue = endValue + (startValue - endValue) * Math.Pow(1 - domainEndParameter, interpolationOrder);

            //Scale interpolated value to the domain range
            double scaledInterpolationValue = (interpolatedValue - domainStartValue) / (domainEndValue - domainStartValue);

            return startValue + (endValue - startValue) * scaledInterpolationValue;

        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Compute an IProfile by using parabolic interpolation at a given parameter between two IProfile objects of the same type.")]
        [Input("startProfile", "The IProfile at the start.")]
        [Input("endProfile", "The IProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the line.")]
        [Input("interpolationOrder", "The value of the polynomimal function used the describe the transition between the startProfile and endProfile.")]
        [Output("interpolatedProfile", "The profile evaluated at the parameter given using interpolation between the startProfile and endProfile using a function with the given interpolation order.")]
        public static IProfile IInterpolateProfile(IProfile startProfile, IProfile endProfile, double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            return InterpolateProfile(startProfile as dynamic, endProfile as dynamic, parameter, interpolationOrder, domainStart, domainEnd);
        }

        /***************************************************/

    }
}




