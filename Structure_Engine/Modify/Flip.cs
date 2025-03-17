/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using System;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using BH.oM.Structure.MaterialFragments;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Spatial;
using System.Data;
using Mono.Reflection;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Flips the Bar geometry and the properties including releases, sectionproperty and orientation angle.")]
        [Input("bar", "The Bar to flip.")]
        [Output("bar", "The Bar with flipped end Nodes.")]
        public static Bar Flip(this Bar bar)
        {
            if (bar.IsNull())
                return null;

            Bar flipped = bar.ShallowClone();

            // Flip Nodes
            Node tempNode = flipped.Start;
            flipped.Start = flipped.End;
            flipped.End = tempNode;

            if (bar.IsVertical())
                flipped.OrientationAngle = -bar.OrientationAngle + Math.PI;
            else
                flipped.OrientationAngle = -bar.OrientationAngle;

            // Flip releases
            BarRelease flippedRelease = bar.Release?.ShallowClone();
            if (flippedRelease != null)
            {
                Constraint6DOF tempRelease = flippedRelease.StartRelease;
                flippedRelease.StartRelease = flippedRelease.EndRelease;
                flippedRelease.EndRelease = tempRelease;
                flipped.Release = flippedRelease;
            }

            // Flip section property
            if (bar.SectionProperty != null)
            {
                ISectionProperty flippedSectionProperty = Flip(bar.SectionProperty);
                flipped.SectionProperty = flippedSectionProperty;
            }

            return flipped;
        }

        /***************************************************/

        [Description("Flips the location curve of the Edge, i.e. the start becomes the end and vice versa.")]
        [Input("edge", "The Edge to flip.")]
        [Output("edge", "The Edge with a flipped location curve.")]
        public static Edge Flip(this Edge edge)
        {
            if (edge.IsNull())
                return null;

            Edge flipped = edge.ShallowClone();
            flipped.Curve = flipped.Curve.IFlip();

            return flipped;
        }

        /***************************************************/

        [Description("Flips the normal of the Stem.")]
        [Input("stem", "The Stem to flip.")]
        [Output("stem", "The Stem with flipped Normal.")]
        public static Stem Flip(this Stem stem)
        {
            if (stem.IsNull())
                return null;
            if (stem.Normal.IsNull("The Normal of the Stem is null and could not be flipped.", "Flip"))
                return null;

            Stem flipped = stem.ShallowClone();

            flipped.Normal = flipped.Normal * -1;

            return flipped;
        }

        /***************************************************/

        private static ISectionProperty Flip(this ISectionProperty section)
        {
            if (section is IGeometricalSection geometricalSection)
            {
                ISectionProperty tempSection = null;
                IProfile profile = geometricalSection.SectionProfile;
                IProfile tempProfile = IFlipProfile(profile);
                IMaterialFragment material = section.Material;

                tempProfile.Name = profile.Name;
                tempProfile.Fragments = profile.Fragments;
                tempSection = Create.GenericSectionFromProfile(tempProfile, material, section.Name);
                return tempSection;
            }
            else
            {
                Base.Compute.RecordWarning("The given shape profile is not an IGeometricalSection.");
                return section;
            }
        }

        /***************************************************/

        private static IProfile IFlipProfile(IProfile profile)
        {
            if (profile is BoxProfile || profile is ICellularOpening || profile is CircleProfile || profile is FabricatedBoxProfile ||
                    profile is FabricatedISectionProfile || profile is GeneralisedFabricatedBoxProfile || profile is ISectionProfile || profile is KiteProfile
                    || profile is RectangleProfile || profile is TaperFlangeISectionProfile || profile is TSectionProfile ||
                    profile is TubeProfile || profile is VoidedISectionProfile)
            {
                return profile;
            }
            else
            {
                return FlipProfile(profile as dynamic);
            }
        }

        /***************************************************/

        private static IProfile FlipProfile(AngleProfile oldProfile)
        {
            return Spatial.Create.AngleProfile(oldProfile.Height, oldProfile.Width, oldProfile.WebThickness, oldProfile.FlangeThickness, oldProfile.RootRadius,
                oldProfile.ToeRadius, !oldProfile.MirrorAboutLocalZ, oldProfile.MirrorAboutLocalY);
        }

        /***************************************************/

        private static IProfile FlipProfile(ChannelProfile oldProfile)
        {
            return Spatial.Create.ChannelProfile(oldProfile.Height, oldProfile.FlangeWidth, oldProfile.WebThickness, oldProfile.FlangeThickness,
                oldProfile.RootRadius, oldProfile.ToeRadius, !oldProfile.MirrorAboutLocalZ);
        }

        /***************************************************/

        private static IProfile FlipProfile(FreeFormProfile oldProfile)
        {
            List<ICurve> curves = oldProfile.Edges.ToList();
            return Spatial.Create.FreeFormProfile(curves.Select(x => x.IMirror(Geometry.Create.Plane(new Point(), Vector.XAxis))));
        }

        /***************************************************/

        private static IProfile FlipProfile(GeneralisedTSectionProfile oldProfile)
        {
            return Spatial.Create.GeneralisedTSectionProfile(oldProfile.Height, oldProfile.WebThickness, oldProfile.RightOutstandWidth, oldProfile.RightOutstandThickness,
                oldProfile.LeftOutstandWidth, oldProfile.LeftOutstandThickness, oldProfile.MirrorAboutLocalY);
        }

        /***************************************************/

        private static IProfile FlipProfile(KiteProfile oldProfile)
        {
            Base.Compute.RecordWarning("Kite profile will not be flipped.");
            return oldProfile;
        }

        /***************************************************/

        private static IProfile FlipProfile(TaperedProfile oldProfile)
        {
            List<IProfile> newProfiles = new List<IProfile>();
            foreach (IProfile profile in oldProfile.Profiles.Values.Reverse())
            {
                newProfiles.Add(IFlipProfile(profile as dynamic));
            }

            return Spatial.Create.TaperedProfile(oldProfile.Profiles.Keys.ToList(), newProfiles, oldProfile.InterpolationOrder);
        }

        /***************************************************/

        private static IProfile FlipProfile(TaperFlangeChannelProfile oldProfile)
        {
            return Spatial.Create.TaperFlangeChannelProfile(oldProfile.Height, oldProfile.FlangeWidth, oldProfile.WebThickness, oldProfile.FlangeThickness, oldProfile.FlangeSlope,
                oldProfile.RootRadius, oldProfile.ToeRadius, !oldProfile.MirrorAboutLocalZ);
        }

        /***************************************************/

        private static IProfile FlipProfile(IProfile oldProfile)
        {
            Base.Compute.RecordError("The given shape profile does not have a FlipProfile method implemented.");
            return null;
        }

        /***************************************************/

    }
}






