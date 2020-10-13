/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.ShapeProfiles;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Compute an IProfile by using parabolic interpolation at a given parameter between two IProfile objects of the same type.")]
        [Input("startProfile", "The IProfile at the start.")]
        [Input("endProfile", "The IProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the line.")]
        [Input("interpolationOrder", "The value of the polynomimal function used the describe the transition between the startProfile and endProfile.")]
        [Output("interpolatedProfile", "The profile evaluated at the parameter given using interpolation between the startProfile and endProfile using a function with the given interpolation order.")]
        public static IProfile InterpolateProfile(IProfile startProfile, IProfile endProfile, double parameter, int interpolationOrder)
        {
            return InterpolateProfile(startProfile as dynamic, endProfile as dynamic, parameter, interpolationOrder);
        }

        /***************************************************/

        private static AngleProfile InterpolateProfile(AngleProfile startProfile, AngleProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.AngleProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder),
                startProfile.MirrorAboutLocalZ, startProfile.MirrorAboutLocalY);
        }

        /***************************************************/

        public static BoxProfile InterpolateProfile(BoxProfile startProfile, BoxProfile endProfile, double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            return Create.BoxProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.OuterRadius, endProfile.OuterRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.InnerRadius, endProfile.InnerRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        private static ChannelProfile InterpolateProfile(ChannelProfile startProfile, ChannelProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.ChannelProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeWidth, endProfile.FlangeWidth, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder),
                startProfile.MirrorAboutLocalZ);
        }

        /***************************************************/

        private static CircleProfile InterpolateProfile(CircleProfile startProfile, CircleProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.CircleProfile(Interpolate(startProfile.Diameter, endProfile.Diameter, parameter, interpolationOrder));
        }

        /***************************************************/

        private static FabricatedBoxProfile InterpolateProfile(FabricatedBoxProfile startProfile, FabricatedBoxProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.FabricatedBoxProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.BotFlangeThickness, endProfile.BotFlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.WeldSize, endProfile.WeldSize, parameter, interpolationOrder));
        }

        /***************************************************/

        private static FabricatedISectionProfile InterpolateProfile(FabricatedISectionProfile startProfile, FabricatedISectionProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.FabricatedISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.TopFlangeWidth, endProfile.TopFlangeWidth, parameter, interpolationOrder),
                Interpolate(startProfile.BotFlangeWidth, endProfile.BotFlangeWidth, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.BotFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.WeldSize, endProfile.WeldSize, parameter, interpolationOrder));
        }

        /***************************************************/

        private static GeneralisedFabricatedBoxProfile InterpolateProfile(GeneralisedFabricatedBoxProfile startProfile, GeneralisedFabricatedBoxProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.GeneralisedFabricatedBoxProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.BotFlangeThickness, endProfile.BotFlangeThickness, parameter, interpolationOrder),
                Interpolate(Math.Max(startProfile.TopLeftCorbelWidth, startProfile.TopRightCorbelWidth), Math.Max(endProfile.TopLeftCorbelWidth, endProfile.TopRightCorbelWidth), parameter, interpolationOrder),
                Interpolate(Math.Max(startProfile.BotLeftCorbelWidth, startProfile.BotRightCorbelWidth), Math.Max(endProfile.BotLeftCorbelWidth, endProfile.BotRightCorbelWidth), parameter, interpolationOrder));
        }

        /***************************************************/

        private static GeneralisedTSectionProfile InterpolateProfile(GeneralisedTSectionProfile startProfile, GeneralisedTSectionProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.GeneralisedTSectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.LeftOutstandWidth, endProfile.LeftOutstandWidth, parameter, interpolationOrder),
                Interpolate(startProfile.LeftOutstandThickness, endProfile.LeftOutstandThickness, parameter, interpolationOrder),
                Interpolate(startProfile.RightOutstandWidth, endProfile.RightOutstandWidth, parameter, interpolationOrder),
                Interpolate(startProfile.RightOutstandThickness, endProfile.RightOutstandThickness, parameter, interpolationOrder),
                startProfile.MirrorAboutLocalY);
        }

        /***************************************************/
        private static ISectionProfile InterpolateProfile(ISectionProfile startProfile, ISectionProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.ISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder));
        }

        /***************************************************/

        private static KiteProfile InterpolateProfile(KiteProfile startProfile, KiteProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.KiteProfile(
                Interpolate(startProfile.Width1, endProfile.Width1, parameter, interpolationOrder),
                Interpolate(startProfile.Angle1, endProfile.Angle1, parameter, interpolationOrder),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder));
        }

        /***************************************************/

        private static RectangleProfile InterpolateProfile(RectangleProfile startProfile, RectangleProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.RectangleProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder),
                Interpolate(startProfile.CornerRadius, endProfile.CornerRadius, parameter, interpolationOrder));
        }

        /***************************************************/


        private static TSectionProfile InterpolateProfile(TSectionProfile startProfile, TSectionProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.TSectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder),
                startProfile.MirrorAboutLocalY);
        }

        /***************************************************/

        private static TubeProfile InterpolateProfile(TubeProfile startProfile, TubeProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.TubeProfile(
                Interpolate(startProfile.Diameter, endProfile.Diameter, parameter, interpolationOrder),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder));
        }

        /***************************************************/

        private static ZSectionProfile InterpolateProfile(ZSectionProfile startProfile, ZSectionProfile endProfile, double parameter, int interpolationOrder)
        {
            return Create.ZSectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeWidth, endProfile.FlangeWidth, parameter, interpolationOrder),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder));
        }

        /***************************************************/

        private static double Interpolate(double start, double end, double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            if (Math.Abs(start - end) < double.Epsilon)
                return start;

            double rescaledParameter = domainStart + (domainEnd - domainStart) * parameter;

            double interpolation =  end + (start - end) * Math.Pow(1 - rescaledParameter, interpolationOrder);

            double interpolationStart = end + (start - end) * Math.Pow(1 - domainStart, interpolationOrder);

            double interpolationEnd = end + (start - end) * Math.Pow(1 - domainEnd, interpolationOrder);

            double normalisedParameter = (interpolation - interpolationStart) / (interpolationEnd - interpolationStart);

            return start + (end - start) * normalisedParameter;

        }

        /***************************************************/

    }
}
