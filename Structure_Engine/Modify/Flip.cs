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
using System.ComponentModel;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Offsets;
using System;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Spatial;
using System.Data;
using BH.oM.Structure.Reinforcement;
using BH.oM.Spatial.Layouts;


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

            // Flip offsets
            if (bar.Offset != null)
            {
                Offset flippedOffset = new Offset()
                {
                    Start = new Vector()
                    {
                        X = -bar.Offset.End.X,
                        Y = -bar.Offset.End.Y,
                        Z = bar.Offset.End.Z
                    },
                    End = new Vector()
                    {
                        X = -bar.Offset.Start.X,
                        Y = -bar.Offset.Start.Y,
                        Z = bar.Offset.Start.Z
                    }
                };

                flipped.Offset = flippedOffset;
            }



            return flipped;
        }

        /***************************************************/

        [Description("Flips the curve layout, i.e. the start becomes the end and vice versa.")]
        [Input("layout", "The curvlayout to flip.")]
        [Output("layout", "The layout with a flipped location curve.")]
        public static ICurveLayout Flip(this ICurveLayout layout)
        {
            if (layout.IsNull())
                return null;

            if (layout is ExplicitCurveLayout)
            {
                ExplicitCurveLayout explicitLayout = layout as ExplicitCurveLayout;
                return new ExplicitCurveLayout(explicitLayout.Curves.Select(x => x.IMirror(Plane.YZ)));

            }
            else if (layout is OffsetCurveLayout)
            {
                return layout;
            }
            else
            {
                Engine.Base.Compute.RecordError("ICurveLayout type not recognised. Unflipped curve layout returned.");
                return layout;
            }
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
            return FlipSection(section as dynamic);
        }

        /***************************************************/

        private static ISectionProperty FlipSection(AluminiumSection section)
        {
            IProfile flippedProfile = IFlipProfile(section.SectionProfile);
            IMaterialFragment material = section.Material;
            AluminiumSection flippedSection = Create.AluminiumSectionFromProfile(flippedProfile, (Aluminium)material, section.Name);
            flippedSection.Fragments = section.Fragments;

            return flippedSection;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(CableSection section)
        {
            return section;
        }

        /***************************************************/
        private static ISectionProperty FlipSection(CellularSection section)
        {
            return section;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(ConcreteSection section)
        {
            IProfile flippedProfile = IFlipProfile(section.SectionProfile);
            IMaterialFragment material = section.Material;

            // Flip reinforcement layouts
            ConcreteSection concSection = section as ConcreteSection;
            List<IBarReinforcement> flippedReinforcements = new List<IBarReinforcement>();
            foreach (IBarReinforcement barReinforcement in concSection.RebarIntent.BarReinforcement)
            {
                IBarReinforcement flipped = barReinforcement.ShallowClone();
                flipped.StartLocation = 1 - barReinforcement.EndLocation;
                flipped.EndLocation = 1 - barReinforcement.StartLocation;
                if (flipped is TransverseReinforcement transverseReinforcement)
                {
                    transverseReinforcement.CenterlineLayout = transverseReinforcement.CenterlineLayout.Flip();
                    flippedReinforcements.Add(transverseReinforcement);
                }
                else if (flipped is LongitudinalReinforcement longitudinalReinforcement)
                {
                    longitudinalReinforcement.RebarLayout = longitudinalReinforcement.RebarLayout.Flip();
                    flippedReinforcements.Add(longitudinalReinforcement);
                }
                else
                    flippedReinforcements.Add(flipped);
            }

            BarRebarIntent flippedRebarIntent = concSection.RebarIntent.ShallowClone();
            flippedRebarIntent.BarReinforcement = flippedReinforcements;

            ConcreteSection flippedSection = Create.ConcreteSectionFromProfile(flippedProfile, (Concrete)material, section.Name, flippedRebarIntent);
            flippedSection.Fragments = section.Fragments;

            return flippedSection;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(ExplicitSection section)
        {
            ExplicitSection flippedSection = section.ShallowClone();
            flippedSection.Vy = section.Vpy;
            flippedSection.Vpy = section.Vy;

            return flippedSection;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(GenericSection section)
        {
            IProfile flippedProfile = IFlipProfile(section.SectionProfile);
            GenericSection flippedSection = Create.GenericSectionFromProfile(flippedProfile, section.Material, section.Name);
            flippedSection.Fragments = section.Fragments;

            return flippedSection;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(SteelSection section)
        {
            IProfile flippedProfile = IFlipProfile(section.SectionProfile);
            IMaterialFragment material = section.Material;
            SteelSection flippedSection = Create.SteelSectionFromProfile(flippedProfile, (Steel)material, section.Name);
            flippedSection.Fragments = section.Fragments;
            flippedSection.Fabrication = section.Fabrication;
            flippedSection.PlateRestraint = section.PlateRestraint;

            return flippedSection;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(TimberSection section)
        {
            IProfile flippedProfile = IFlipProfile(section.SectionProfile);
            IMaterialFragment material = section.Material;
            TimberSection flippedSection = Create.TimberSectionFromProfile(flippedProfile, (ITimber)material, section.Name);
            flippedSection.Fragments = section.Fragments;

            return flippedSection;
        }

        /***************************************************/

        private static ISectionProperty FlipSection(ISectionProperty section)
        {
            Base.Compute.RecordWarning($"The given ISectionProperty {section.Name} of type {section.GetType()}" +
                $"does not have a FlipProfile method implemented, the original section is returned.");
            return section;
        }


        [Description("Flips the ILayout2D for it to be used with a flipped bar. This means flipping the layout around the XZ plane.")]
        [Input("layout", "The ILayout2D to flip.")]
        [Output("layout", "The layout with a flipped.")]
        public static ILayout2D Flip(this ILayout2D layout)
        {
            if (layout.IsNull())
                return null;
            if (layout is ExplicitLayout explicitLayout)
            {
                return new ExplicitLayout(explicitLayout.Points.Select(x => x.Mirror(Plane.YZ)));
            }
            if (layout is LinearLayout linearLayout)
            {
                return new LinearLayout(linearLayout.NumberOfPoints, linearLayout.Direction.Mirror(Plane.XZ), linearLayout.Offset, linearLayout.ReferencePoint.Flip());
            }
            if (layout is MultiLinearLayout multiLayout)
            {
                return new MultiLinearLayout(multiLayout.NumberOfPoints, multiLayout.ParallelMinimumSpacing, multiLayout.PerpendicularMinimumSpacing, multiLayout.Direction.Mirror(Plane.XZ), multiLayout.Offset, multiLayout.ReferencePoint.Flip());
            }
            return layout; //TODO: Implement flipping for other layouts as needed. Might require flipping of direction vectors as well as offset points, but for the most common usecase of horizontal rebars, this is not a factor.
        }

        /***************************************************/

        private static ReferencePoint Flip(this ReferencePoint referencePoint)
        {
            switch (referencePoint)
            {
                case ReferencePoint.BottomLeft:
                    return ReferencePoint.BottomRight;
                case ReferencePoint.BottomRight:
                    return ReferencePoint.BottomLeft;
                case ReferencePoint.MiddleLeft:
                    return ReferencePoint.MiddleRight;
                case ReferencePoint.MiddleRight:
                    return ReferencePoint.MiddleLeft;
                case ReferencePoint.TopLeft:
                    return ReferencePoint.TopRight;
                case ReferencePoint.TopRight:
                    return ReferencePoint.TopLeft;
                case ReferencePoint.TopCenter:
                case ReferencePoint.MiddleCenter:
                case ReferencePoint.Centroid:
                case ReferencePoint.BottomCenter:
                default:
                    return referencePoint;
            }
        }

        /***************************************************/

        private static IProfile IFlipProfile(IProfile profile)
        {
            if (profile.ISymmetry() == Symmetry.DoublySymmetric)
            {
                return profile;
            }
            else
            {
                IProfile flippedProfile = FlipProfile(profile as dynamic);
                flippedProfile.Name = profile.Name;
                flippedProfile.Fragments = profile.Fragments;

                return flippedProfile;
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

        private static IProfile FlipProfile(FabricatedISectionProfile oldProfile)
        {
            return oldProfile;
        }

        /***************************************************/

        private static IProfile FlipProfile(FreeFormProfile oldProfile)
        {
            List<ICurve> curves = oldProfile.Edges.ToList();
            return Spatial.Create.FreeFormProfile(curves.Select(x => x.IMirror(Geometry.Create.Plane(new Point(), Vector.XAxis))));
        }

        /***************************************************/

        private static IProfile FlipProfile(GeneralisedFabricatedBoxProfile oldProfile)
        {
            return oldProfile;
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
            return Spatial.Create.KiteProfile(oldProfile.Width1 * Math.Tan(oldProfile.Angle1 / 2), Math.PI - oldProfile.Angle1, oldProfile.Thickness);
        }

        /***************************************************/

        private static IProfile FlipProfile(TSectionProfile oldProfile)
        {
            return oldProfile;
        }

        /***************************************************/

        private static IProfile FlipProfile(TaperedProfile oldProfile)
        {
            List<IProfile> newProfiles = new List<IProfile>();
            List<double> newPositions = new List<double>();
            List<double> keys = oldProfile.Profiles.Keys.ToList();

            for (int i = 0; i < oldProfile.Profiles.Values.Count; i++)
            {
                double key = keys[i];
                newPositions.Add(1 - key);
                oldProfile.Profiles.TryGetValue(key, out IProfile newProfile);
                newProfiles.Add(FlipProfile(newProfile as dynamic));
            }

            newProfiles.Reverse();
            newPositions.Reverse();

            return Spatial.Create.TaperedProfile(newPositions, newProfiles, oldProfile.InterpolationOrder);
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
            Base.Compute.RecordWarning($"The given shape profile {oldProfile.Name} of type {oldProfile.GetType()} " +
                $"does not have a FlipProfile method implemented, the original profile is returned.");
            return oldProfile;
        }

        /***************************************************/

    }
}






