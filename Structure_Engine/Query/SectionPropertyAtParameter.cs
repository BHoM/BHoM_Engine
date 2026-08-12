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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the section property at a normalised position along the Bar. For Bars with TaperedProfiles, this interpolates the profile at the given position and returns a new section property with the interpolated profile. For non-tapered sections, the original section property is returned.")]
        [Input("bar", "The Bar to get the section property from.")]
        [Input("position", "Normalised position along the Bar between 0 (start) and 1 (end).")]
        [Output("sectionProperty", "The section property at the given position.")]
        public static ISectionProperty SectionPropertyAtParameter(this Bar bar, double position)
        {
            if (bar.IsNull())
                return null;

            if (bar.SectionProperty.IsNull())
                return null;

            return SectionPropertyAtParameter(bar.SectionProperty, position);
        }

        /***************************************************/

        [Description("Gets the section property at a normalised position. For section properties with TaperedProfiles, this interpolates the profile at the given position and returns a new section property with the interpolated cross-section constants. For non-tapered sections, the original section property is returned.")]
        [Input("sectionProperty", "The section property to evaluate.")]
        [Input("position", "Normalised position between 0 (start) and 1 (end).")]
        [Output("sectionProperty", "The section property at the given position.")]
        public static ISectionProperty SectionPropertyAtParameter(this ISectionProperty sectionProperty, double position)
        {
            if (!(sectionProperty is IGeometricalSection geoSection))
                return sectionProperty;

            if (!(geoSection.SectionProfile is TaperedProfile taperedProfile))
                return sectionProperty;

            IProfile interpolatedProfile = Spatial.Compute.InterpolateProfileAtPosition(taperedProfile, position);
            if (interpolatedProfile.IsNull())
                return null;

            return CreateSectionAtParameter(sectionProperty as dynamic, interpolatedProfile);
        }

        /***************************************************/
        /**** Private Methods - Section Creation        ****/
        /***************************************************/

        private static SteelSection CreateSectionAtParameter(SteelSection section, IProfile interpolatedProfile)
        {
            SteelSection result = Create.SteelSectionFromProfile(interpolatedProfile, section.Material as Steel, section.Name);
            result.Fabrication = section.Fabrication;
            result.PlateRestraint = section.PlateRestraint;
            return result;
        }

        /***************************************************/

        private static ConcreteSection CreateSectionAtParameter(ConcreteSection section, IProfile interpolatedProfile)
        {
            return Create.ConcreteSectionFromProfile(interpolatedProfile, section.Material as Concrete, section.Name, section.RebarIntent);
        }

        /***************************************************/

        private static AluminiumSection CreateSectionAtParameter(AluminiumSection section, IProfile interpolatedProfile)
        {
            return Create.AluminiumSectionFromProfile(interpolatedProfile, section.Material as Aluminium, section.Name);
        }

        /***************************************************/

        private static TimberSection CreateSectionAtParameter(TimberSection section, IProfile interpolatedProfile)
        {
            return Create.TimberSectionFromProfile(interpolatedProfile, section.Material as ITimber, section.Name);
        }

        /***************************************************/

        private static GenericSection CreateSectionAtParameter(GenericSection section, IProfile interpolatedProfile)
        {
            return Create.GenericSectionFromProfile(interpolatedProfile, section.Material, section.Name);
        }

        /***************************************************/

        private static ISectionProperty CreateSectionAtParameter(ISectionProperty section, IProfile interpolatedProfile)
        {
            Base.Compute.RecordWarning($"SectionPropertyAtParameter is not supported for section properties of type {section.GetType().Name}. The original section property has been returned.");
            return section;
        }

        /***************************************************/
        /**** Private Methods - Profile Interpolation   ****/
        /***************************************************/



        /***************************************************/
    }
}
