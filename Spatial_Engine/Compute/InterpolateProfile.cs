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
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Spatial
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Compute an IProfile by using parabolic interpolation at a given parameter between two IProfile objects of the same type.")]
        [Input("startProfile", "The IProfile at the start.")]
        [Input("endProfile", "The IProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the line.")]
        [Input("interpolationOrder", "The value of the polynomimal function used the describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The profile evaluated at the parameter given using interpolation between the startProfile and endProfile using a function with the given interpolation order.")]
        public static IProfile IInterpolateProfile(IProfile startProfile, IProfile endProfile, double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            return InterpolateProfile(startProfile as dynamic, endProfile as dynamic, parameter, interpolationOrder, domainStart, domainEnd);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Compute an AngleProfile by interpolating between two AngleProfile objects at the given parameter.")]
        [Input("startProfile", "The AngleProfile at the start.")]
        [Input("endProfile", "The AngleProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The AngleProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a BoxProfile by interpolating between two BoxProfile objects at the given parameter.")]
        [Input("startProfile", "The BoxProfile at the start.")]
        [Input("endProfile", "The BoxProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The BoxProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a ChannelProfile by interpolating between two ChannelProfile objects at the given parameter.")]
        [Input("startProfile", "The ChannelProfile at the start.")]
        [Input("endProfile", "The ChannelProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The ChannelProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a CircleProfile by interpolating between two CircleProfile objects at the given parameter.")]
        [Input("startProfile", "The CircleProfile at the start.")]
        [Input("endProfile", "The CircleProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The CircleProfile evaluated at the given parameter using interpolation.")]
        public static CircleProfile InterpolateProfile(CircleProfile startProfile, CircleProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.CircleProfile(Interpolate(startProfile.Diameter, endProfile.Diameter, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a FabricatedBoxProfile by interpolating between two FabricatedBoxProfile objects at the given parameter.")]
        [Input("startProfile", "The FabricatedBoxProfile at the start.")]
        [Input("endProfile", "The FabricatedBoxProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The FabricatedBoxProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a FabricatedISectionProfile by interpolating between two FabricatedISectionProfile objects at the given parameter.")]
        [Input("startProfile", "The FabricatedISectionProfile at the start.")]
        [Input("endProfile", "The FabricatedISectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The FabricatedISectionProfile evaluated at the given parameter using interpolation.")]
        public static FabricatedISectionProfile InterpolateProfile(FabricatedISectionProfile startProfile, FabricatedISectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.FabricatedISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.TopFlangeWidth, endProfile.TopFlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.BotFlangeWidth, endProfile.BotFlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.TopFlangeThickness, endProfile.TopFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.BotFlangeThickness, endProfile.BotFlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WeldSize, endProfile.WeldSize, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a GeneralisedFabricatedBoxProfile by interpolating between two GeneralisedFabricatedBoxProfile objects at the given parameter.")]
        [Input("startProfile", "The GeneralisedFabricatedBoxProfile at the start.")]
        [Input("endProfile", "The GeneralisedFabricatedBoxProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The GeneralisedFabricatedBoxProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a GeneralisedTSectionProfile by interpolating between two GeneralisedTSectionProfile objects at the given parameter.")]
        [Input("startProfile", "The GeneralisedTSectionProfile at the start.")]
        [Input("endProfile", "The GeneralisedTSectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The GeneralisedTSectionProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute an ISectionProfile by interpolating between two ISectionProfile objects at the given parameter.")]
        [Input("startProfile", "The ISectionProfile at the start.")]
        [Input("endProfile", "The ISectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The ISectionProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a KiteProfile by interpolating between two KiteProfile objects at the given parameter.")]
        [Input("startProfile", "The KiteProfile at the start.")]
        [Input("endProfile", "The KiteProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The KiteProfile evaluated at the given parameter using interpolation.")]
        public static KiteProfile InterpolateProfile(KiteProfile startProfile, KiteProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.KiteProfile(
                Interpolate(startProfile.Width1, endProfile.Width1, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Angle1, endProfile.Angle1, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a RectangleProfile by interpolating between two RectangleProfile objects at the given parameter.")]
        [Input("startProfile", "The RectangleProfile at the start.")]
        [Input("endProfile", "The RectangleProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The RectangleProfile evaluated at the given parameter using interpolation.")]
        public static RectangleProfile InterpolateProfile(RectangleProfile startProfile, RectangleProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.RectangleProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.CornerRadius, endProfile.CornerRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a TSectionProfile by interpolating between two TSectionProfile objects at the given parameter.")]
        [Input("startProfile", "The TSectionProfile at the start.")]
        [Input("endProfile", "The TSectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The TSectionProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a TubeProfile by interpolating between two TubeProfile objects at the given parameter.")]
        [Input("startProfile", "The TubeProfile at the start.")]
        [Input("endProfile", "The TubeProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The TubeProfile evaluated at the given parameter using interpolation.")]
        public static TubeProfile InterpolateProfile(TubeProfile startProfile, TubeProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.TubeProfile(
                Interpolate(startProfile.Diameter, endProfile.Diameter, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Thickness, endProfile.Thickness, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a ZSectionProfile by interpolating between two ZSectionProfile objects at the given parameter.")]
        [Input("startProfile", "The ZSectionProfile at the start.")]
        [Input("endProfile", "The ZSectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The ZSectionProfile evaluated at the given parameter using interpolation.")]
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

        [Description("Compute a TaperFlangeISectionProfile by interpolating between two TaperFlangeISectionProfile objects at the given parameter.")]
        [Input("startProfile", "The TaperFlangeISectionProfile at the start.")]
        [Input("endProfile", "The TaperFlangeISectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The TaperFlangeISectionProfile evaluated at the given parameter using interpolation.")]
        public static TaperFlangeISectionProfile InterpolateProfile(TaperFlangeISectionProfile startProfile, TaperFlangeISectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.TaperFlangeISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeSlope, endProfile.FlangeSlope, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a TaperFlangeChannelProfile by interpolating between two TaperFlangeChannelProfile objects at the given parameter.")]
        [Input("startProfile", "The TaperFlangeChannelProfile at the start.")]
        [Input("endProfile", "The TaperFlangeChannelProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The TaperFlangeChannelProfile evaluated at the given parameter using interpolation.")]
        public static TaperFlangeChannelProfile InterpolateProfile(TaperFlangeChannelProfile startProfile, TaperFlangeChannelProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.TaperFlangeChannelProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeWidth, endProfile.FlangeWidth, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeSlope, endProfile.FlangeSlope, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd),
                startProfile.MirrorAboutLocalZ);
        }

        /***************************************************/

        [Description("Compute a VoidedISectionProfile by interpolating between two VoidedISectionProfile objects at the given parameter.")]
        [Input("startProfile", "The VoidedISectionProfile at the start.")]
        [Input("endProfile", "The VoidedISectionProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The VoidedISectionProfile evaluated at the given parameter using interpolation.")]
        public static VoidedISectionProfile InterpolateProfile(VoidedISectionProfile startProfile, VoidedISectionProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            return Create.VoidedISectionProfile(
                Interpolate(startProfile.Height, endProfile.Height, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.OpeningHeight, endProfile.OpeningHeight, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.Width, endProfile.Width, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.WebThickness, endProfile.WebThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.FlangeThickness, endProfile.FlangeThickness, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.RootRadius, endProfile.RootRadius, parameter, interpolationOrder, domainStart, domainEnd),
                Interpolate(startProfile.ToeRadius, endProfile.ToeRadius, parameter, interpolationOrder, domainStart, domainEnd));
        }

        /***************************************************/

        [Description("Compute a FreeFormProfile by interpolating between two FreeFormProfile objects at the given parameter. \n" +
            "The profiles must have the same number of edges, and each corresponding edge must have the same number of control points.")]
        [Input("startProfile", "The FreeFormProfile at the start.")]
        [Input("endProfile", "The FreeFormProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the element.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the startProfile and endProfile.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedProfile", "The FreeFormProfile evaluated at the given parameter using interpolation.")]
        public static FreeFormProfile InterpolateProfile(FreeFormProfile startProfile, FreeFormProfile endProfile, double parameter, int interpolationOrder,
            double domainStart = 0, double domainEnd = 1)
        {
            List<ICurve> startEdges = startProfile.Edges.ToList();
            List<ICurve> endEdges = endProfile.Edges.ToList();

            if (startEdges.Count != endEdges.Count)
            {
                Base.Compute.RecordError("Cannot interpolate FreeFormProfiles with different numbers of edges.");
                return null;
            }

            List<ICurve> interpolatedEdges = new List<ICurve>();
            for (int i = 0; i < startEdges.Count; i++)
            {
                interpolatedEdges.Add(InterpolateEdge(startEdges[i], endEdges[i], parameter, interpolationOrder, domainStart, domainEnd));
            }

            Engine.Base.Compute.RecordWarning("Freeform profiles are centred by default when interpolating profiles");

            return Create.FreeFormProfile(interpolatedEdges, true);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Interpolate between two double values at the given parameter using a polynomial function of the given order, scaled to the provided domain.")]
        [Input("startValue", "The value at the start of the interpolation.")]
        [Input("endValue", "The value at the end of the interpolation.")]
        [Input("parameter", "A number between 0 and 1 that describes the position along the interpolation.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the start and end values.")]
        [Input("domainStartParameter", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEndParameter", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedValue", "The value evaluated at the given parameter using polynomial interpolation.")]
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

        [Description("Interpolate between two ICurve edges at the given parameter by interpolating their control points using a polynomial function of the given order.")]
        [Input("startEdge", "The ICurve at the start of the interpolation.")]
        [Input("endEdge", "The ICurve at the end of the interpolation.")]
        [Input("parameter", "A number between 0 and 1 that describes the position along the interpolation.")]
        [Input("interpolationOrder", "The order of the polynomial function used to describe the transition between the start and end edges.")]
        [Input("domainStart", "The start of the domain for the interpolation. Defaults to 0.")]
        [Input("domainEnd", "The end of the domain for the interpolation. Defaults to 1.")]
        [Output("interpolatedEdge", "The ICurve edge evaluated at the given parameter using polynomial interpolation of the control points.")]
        private static ICurve InterpolateEdge(ICurve startEdge, ICurve endEdge, double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            List<Point> startPoints = startEdge.IControlPoints();
            List<Point> endPoints = endEdge.IControlPoints();

            if (startPoints.Count != endPoints.Count)
            {
                Base.Compute.RecordError("Cannot interpolate edges with different numbers of control points.");
                return null;
            }

            List<Point> interpolatedPoints = new List<Point>();
            for (int i = 0; i < startPoints.Count; i++)
            {
                double x = Interpolate(startPoints[i].X, endPoints[i].X, parameter, interpolationOrder, domainStart, domainEnd);
                double y = Interpolate(startPoints[i].Y, endPoints[i].Y, parameter, interpolationOrder, domainStart, domainEnd);
                double z = Interpolate(startPoints[i].Z, endPoints[i].Z, parameter, interpolationOrder, domainStart, domainEnd);
                interpolatedPoints.Add(new Point { X = x, Y = y, Z = z });
            }

            if (interpolatedPoints.Count == 2)
                return new Line { Start = interpolatedPoints[0], End = interpolatedPoints[1] };

            return new Polyline { ControlPoints = interpolatedPoints };
        }

        /***************************************************/
        /**** Private fallback method                   ****/
        /***************************************************/

        [Description("Compute an IProfile by using parabolic interpolation at a given parameter between two IProfile objects of the same type.")]
        [Input("startProfile", "The IProfile at the start.")]
        [Input("endProfile", "The IProfile at the end.")]
        [Input("parameter", "A number between 0 and 1 that describes the distance along the line.")]
        [Input("interpolationOrder", "The value of the polynomimal function used the describe the transition between the startProfile and endProfile.")]
        [Output("interpolatedProfile", "The profile evaluated at the parameter given using interpolation between the startProfile and endProfile using a function with the given interpolation order.")]
        public static IProfile InterpolateProfile(IProfile startProfile, IProfile endProfile, double parameter, int interpolationOrder, double domainStart = 0, double domainEnd = 1)
        {
            Base.Compute.RecordError("The profile provided is not supported for interpolation.");
            return null;
        }

        /***************************************************/

    }
}
