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
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Offsets;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Constraints;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Profiles                 ****/
        /***************************************************/
        [Description("Generates a default description for the profile as 'I Height x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this ISectionProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"I {profile.Height:G3}x{profile.Width:G3}x{profile.WebThickness:G3}x{profile.FlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Box Height x Width x Thickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this BoxProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Box {profile.Height:G3}x{profile.Width:G3}x{profile.Thickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Angle Height x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this AngleProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Angle {profile.Height:G3}x{profile.Width:G3}x{profile.WebThickness:G3}x{profile.FlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Channel Height x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this ChannelProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Channel {profile.Height:G3}x{profile.FlangeWidth:G3}x{profile.WebThickness:G3}x{profile.FlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Circle Diameter'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this CircleProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Circle {profile.Diameter:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'FabBox Height x Width x WebThickness x TopFlangeThickness x BotFlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this FabricatedBoxProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"FabBox {profile.Height:G3}x{profile.Width:G3}x" + $"{profile.WebThickness:G3}x{profile.TopFlangeThickness:G3}x{profile.BotFlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'GenFabBox Height x TotalWidth x WebThickness x TopFlangeThickness x BotFlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this GeneralisedFabricatedBoxProfile profile)
        {
            if (profile == null)
                return "null profile";
            double width = Math.Max(profile.Width + profile.BotLeftCorbelWidth + profile.BotRightCorbelWidth, profile.Width + profile.TopLeftCorbelWidth + profile.TopRightCorbelWidth);
            return $"GenFabBox {profile.Height:G3}x{width:G3}x{profile.WebThickness:G3}x" + $"{profile.TopFlangeThickness:G3}x{profile.BotFlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Kite Angle x Width x Thickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this KiteProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Kite {profile.Angle1:G2}x{profile.Width1:G3}x{profile.Thickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'FabI Height x TopFlangeWidth x BotFlangeWidth x WebThickness x TopFlangeThickness x BotFlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this FabricatedISectionProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"FabI {profile.Height:G3}x{profile.TopFlangeWidth:G3}x{profile.BotFlangeWidth:G3}x" + $"{profile.WebThickness:G3}x{profile.TopFlangeThickness:G3}x{profile.BotFlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'FreeForm'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this FreeFormProfile profile)
        {
            if (profile == null)
                return "null profile";
            return "FreeForm";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Rectangle Height x Width x CornerRadius'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this RectangleProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Rectangle {profile.Height:G3}x{profile.Width:G3}x{profile.CornerRadius:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'T Height x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this TSectionProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"T {profile.Height:G3}x{profile.Width:G3}x{profile.WebThickness:G3}x{profile.FlangeThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'GenT Height x WebThickness x LeftOutstandWidth x LeftOutstandThickness x RightOutstandWidth x RightOutstandThickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this GeneralisedTSectionProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"GenT {profile.Height:G3}x{profile.WebThickness}x" + $"{profile.LeftOutstandWidth:G3}x{profile.LeftOutstandThickness:G3}x" + $"{profile.RightOutstandWidth:G3}x{profile.RightOutstandThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the profile as 'Tube Diameter x Thickness'.")]
        [Input("profile", "The profile to generate a default description for.")]
        [Output("desc", "The generated description for the profile based on its dimensions.")]
        public static string Description(this TubeProfile profile)
        {
            if (profile == null)
                return "null profile";
            return $"Tube {profile.Diameter:G3} x {profile.Thickness:G3}";
        }

        /***************************************************/
        /**** Public Methods - Sections                 ****/
        /***************************************************/
        [Description("Generates a default description for the Section as 'Steel ProfileDescription - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this SteelSection section)
        {
            if (section == null)
                return "null section";
            return "Steel " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/
        [Description("Generates a default description for the Section as 'Concrete ProfileDescription - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this ConcreteSection section)
        {
            if (section == null)
                return "null section";
            return "Concrete " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/
        [Description("Generates a default description for the Section as 'Timber ProfileDescription - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this TimberSection section)
        {
            if (section == null)
                return "null section";
            return "Timber  " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/
        [Description("Generates a default description for the Section as 'Aluminium ProfileDescription - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this AluminiumSection section)
        {
            if (section == null)
                return "null section";
            return "Aluminium " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/
        [Description("Generates a default description for the Section as 'Generic ProfileDescription - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this GenericSection section)
        {
            if (section == null)
                return "null section";
            return "Generic " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/
        [Description("Generates a default description for the Section as 'Cable NumberOfCables x Diameter - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this CableSection section)
        {
            if (section == null)
                return "null section";
            return $"Cable {section.NumberOfCables:G3} x dia {section.CableDiameter:G3} - {CheckGetMaterialName(section.Material)}";
        }

        /***************************************************/
        [Description("Generates a default description for the Section as 'Explicit Area Iy Iz J - MaterialName'.")]
        [Input("section", "The section to generate a default description for.")]
        [Output("desc", "The generated description for the section based on its dimensions, material and type.")]
        public static string Description(this ExplicitSection section)
        {
            if (section == null)
                return "null section";
            return $"Explicit A: {section.Area:G3} Iy: {section.Iy:G3} Iz: {section.Iz:G3} J: {section.J:G3} - {CheckGetMaterialName(section.Material)}";
        }

        /***************************************************/
        /**** Public Methods - Surface Properties       ****/
        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'THK Thickness - MaterialName'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this ConstantThickness property)
        {
            if (property == null)
                return "null property";
            return $"THK {property.Thickness:G3} - {CheckGetMaterialName(property.Material)}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Ribbed Depth Spacing StemWidth - MaterialName'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this Ribbed property)
        {
            if (property == null)
                return "null property";
            return $"Ribbed Depth: {property.TotalDepth:G3} Spacing: {property.Spacing:G3} sWidth: {property.StemWidth:G3}" + $" - {CheckGetMaterialName(property.Material)}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Ribbed: TopThickness THK slab MaterialName on RibHeight x RibThickness ribs RibMaterialName spaced RibSpacing'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this BuiltUpRibbed property)
        {
            if (property == null)
                return "null property";
            return $"Ribbed: {property.TopThickness:G3} THK slab {CheckGetMaterialName(property.Material)} on {property.RibHeight:G3}x{property.RibThickness:G3} ribs {CheckGetMaterialName(property.RibMaterial ?? property.Material)} spaced {property.RibSpacing:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Ribbed: TopThickness THK slab MaterialName on double RibHeight x RibThickness ribs RibMaterialName spaced RibSpacing'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this BuiltUpDoubleRibbed property)
        {
            if (property == null)
                return "null property";
            return $"Ribbed: {property.TopThickness:G3} THK slab {CheckGetMaterialName(property.Material)} on double {property.RibHeight:G3}x{property.RibThickness:G3} ribs {CheckGetMaterialName(property.RibMaterial ?? property.Material)} spaced {property.RibSpacing:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Cassette Top: TopThickness THK TopMaterialName Bot: BottomThickness THK BottomMaterialName Ribs: RibHeight x RibThickness RibMaterialName spaced RibSpacing'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this Cassette property)
        {
            if (property == null)
                return "null property";
            return $"Cassette Top: {property.TopThickness:G3} THK {CheckGetMaterialName(property.Material)} Bot: {property.BottomThickness:G3} THK {CheckGetMaterialName(property.BottomMaterial ?? property.Material)} Ribs: {property.RibHeight:G3}x{property.RibThickness:G3} {CheckGetMaterialName(property.RibMaterial ?? property.Material)} spaced {property.RibSpacing:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Waffle DepthX DepthY SpacingX SpacingY StemWidthX StemWidthY - MaterialName'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this Waffle property)
        {
            if (property == null)
                return "null property";
            return $"Waffle DepthX: {property.TotalDepthX:G3} DepthY: {property.TotalDepthY:G3} " + $"SpacingX: {property.SpacingX:G3} SpacingY: {property.SpacingY:G3} " + $"sWidthX: {property.StemWidthX:G3} sWidthY: {property.StemWidthY:G3} - {CheckGetMaterialName(property.Material)}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'LoadingPanel LoadApplication ReferenceEdge'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property.")]
        public static string Description(this LoadingPanelProperty property)
        {
            if (property == null)
                return "null property";
            return "LoadingPanel Application: " + property.LoadApplication + " RefEdge: " + property.ReferenceEdge;
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Shell with Layers: <Thickness - Material> - Base Material: <Material>'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property.")]
        public static string Description(this Layered property)
        {
            if (property == null)
                return "null property";
            List<string> layers = new List<string>()
            {};
            foreach (Layer layer in property.Layers)
            {
                if (property == null)
                    layers.Add("null layer");
                else
                    layers.Add($"THK {layer.Thickness:G3} - {CheckGetMaterialName(layer.Material)}");
            }

            return $"Shell with layers: {String.Join(", ", layers)} - Base Material: {CheckGetMaterialName(property.Material)}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'Thickness of MaterialName on DeckName DeckMaterialName corrugated deck with height and thickness'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this SlabOnDeck property)
        {
            if (property == null)
                return "null property";
            return $"{property.SlabThickness:G3} thick of {property.Material.DescriptionOrName()} on {property.DeckName} {property.DeckMaterial.DescriptionOrName()} corrugated deck with height: {property.DeckHeight:G3} and thickness: {property.DeckThickness:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty as 'MaterialName corrugated deck with thickness, height, spacing, top flute width, and bottom flute width'.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the property based on its dimensions, material and type.")]
        public static string Description(this CorrugatedDeck property)
        {
            if (property == null)
                return "null property";
            return $"{property.Material.DescriptionOrName()} corrugated deck with thickness: {property.Thickness:G3}, height: {property.Height:G3}, spacing: {property.Spacing}, top flute width: {property.TopWidth}, and bottom flute width: {property.BottomWidth}.";
        }

        /***************************************************/
        /**** Public Methods - Constraints              ****/
        /***************************************************/
        [Description("Generates a default description for the Constraint3DOF based on the constraints at each degree of freedom.")]
        [Input("constraint", "The Constraint3DOF to generate a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Constraint3DOF constraint)
        {
            if (constraint == null)
                return "null constraint";
            string desc = constraint.UX.DofSign() + constraint.UY.DofSign() + constraint.Normal.DofSign();
            if (constraint.IsNumericallyDependent())
            {
                desc += $"- {constraint.KX:G3}, {constraint.KY:G3}, {constraint.KNorm:G3}";
            }

            return desc;
        }

        /***************************************************/
        [Description("Generates a default description for the Constraint4DOF based on the constraints at each degree of freedom.")]
        [Input("constraint", "The Constraint4DOF to generate a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Constraint4DOF constraint)
        {
            if (constraint == null)
                return "null constraint";
            string desc = constraint.TranslationX.DofSign() + constraint.TranslationY.DofSign() + constraint.TranslationZ.DofSign() + constraint.RotationX.DofSign();
            if (constraint.IsNumericallyDependent())
            {
                desc += $"- {constraint.TranslationalStiffnessX:G3}, {constraint.TranslationalStiffnessY:G3}, {constraint.TranslationalStiffnessZ:G3}, " + $"{constraint.RotationalStiffnessX:G3}";
            }

            return desc;
        }

        /***************************************************/
        [Description("Generates a default description for the Constraint6DOF based on the constraints at each degree of freedom.")]
        [Input("constraint", "The Constraint6DOF to generate a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Constraint6DOF constraint)
        {
            if (constraint == null)
                return "null constraint";
            string desc = constraint.TranslationX.DofSign() + constraint.TranslationY.DofSign() + constraint.TranslationZ.DofSign() + constraint.RotationX.DofSign() + constraint.RotationY.DofSign() + constraint.RotationZ.DofSign();
            if (constraint.IsNumericallyDependent())
            {
                desc += $"- {constraint.TranslationalStiffnessX:G3}, {constraint.TranslationalStiffnessY:G3}, {constraint.TranslationalStiffnessZ:G3}, " + $"{constraint.RotationalStiffnessX:G3}, {constraint.RotationalStiffnessY:G3}, {constraint.RotationalStiffnessZ:G3}";
            }

            return desc;
        }

        /***************************************************/
        [Description("Generates a default description for the BarRelease as 'StartReleaseNameOrDesc - EndReleaseNameOrDesc'.")]
        [Input("release", "The BarRelease to generate a default description for.")]
        [Output("desc", "The generated description for the BarRelease.")]
        public static string Description(this BarRelease release)
        {
            if (release == null)
                return "null release";
            return release.StartRelease.DescriptionOrName() + " - " + release.EndRelease.DescriptionOrName();
        }

        /***************************************************/
        [Description("Generates a default description for the LinkConstraint based on its fixities where 'x' denotes fixed and 'f' denotes free.")]
        [Input("constraint", "The LinkConstraint to generate a default description for.")]
        [Output("desc", "The generated description for the LinkConstraint.")]
        public static string Description(this LinkConstraint constraint)
        {
            if (constraint == null)
                return "null constraint";
            string desc = constraint.XtoX ? "x" : "f";
            desc += constraint.YtoY ? "x" : "f";
            desc += constraint.ZtoZ ? "x" : "f";
            desc += constraint.XXtoXX ? "x" : "f";
            desc += constraint.YYtoYY ? "x" : "f";
            desc += constraint.ZZtoZZ ? "x" : "f";
            desc += constraint.XtoYY ? "x" : "f";
            desc += constraint.XtoZZ ? "x" : "f";
            desc += constraint.YtoXX ? "x" : "f";
            desc += constraint.YtoZZ ? "x" : "f";
            desc += constraint.ZtoXX ? "x" : "f";
            desc += constraint.ZtoYY ? "x" : "f";
            return desc;
        }

        /***************************************************/
        /**** Public Methods - Offset                   ****/
        /***************************************************/
        [Description("Generates a default description for the Offset as 'Start - End'.")]
        [Input("offset", "The Offset to generate a default description for.")]
        [Output("desc", "The generated description for the offset.")]
        public static string Description(this Offset offset)
        {
            if (offset == null)
                return "null offset";
            return $"{offset.Start.VectorComponents()} - {offset.End.VectorComponents()}";
        }

        /***************************************************/
        /**** Public Methods - Material                 ****/
        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Steel material)
        {
            if (material == null)
                return "null material";
            return $"Steel {material.MaterialAnalyticalValues():G3}, fy: {material.YieldStress:G3}, fu: {material.UltimateStress:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Concrete material)
        {
            if (material == null)
                return "null material";
            return $"Concrete {material.MaterialAnalyticalValues()}, cyl: {material.CylinderStrength:G3}, cube: {material.CubeStrength:G3}";
        }

        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Aluminium material)
        {
            if (material == null)
                return "null material";
            return "Alum " + material.MaterialAnalyticalValues();
        }

        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this GenericIsotropicMaterial material)
        {
            if (material == null)
                return "null material";
            return "Gen " + material.MaterialAnalyticalValues();
        }

        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Timber material)
        {
            if (material == null)
                return "null material";
            return "Timber " + material.MaterialAnalyticalValues();
        }

        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this GenericOrthotropicMaterial material)
        {
            if (material == null)
                return "null material";
            return "Gen " + material.MaterialAnalyticalValues();
        }

        /***************************************************/
        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to generate a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this ITimber material)
        {
            if (material == null)
                return "null material";
            string timberTypeName = material.IAcronymTypeName();
            if (material.YoungsModulus == null && material.ShearModulus == null && material.PoissonsRatio == null)
                return $"{timberTypeName} - Analysis values unset";
            else
                return $"{timberTypeName} - {material.MaterialAnalyticalValues()}";
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/
        [Description("Generates a default description for the IProperty, based on its properties.")]
        [Input("property", "The property to generate a default description for.")]
        [Output("desc", "The generated description for the property, based on its property values.")]
        public static string IDescription(this IProperty property)
        {
            if (property == null)
                return "null property";
            return Description(property as dynamic);
        }

        /***************************************************/
        [Description("Generates a default description for the SectionProperty, based on type, profile and material.")]
        [Input("section", "The SectionProperty to generate a default description for.")]
        [Output("desc", "The generated description for the SectionProperty based on its dimensions, material and type.")]
        public static string IDescription(this ISectionProperty section)
        {
            if (section == null)
                return "null section";
            return Description(section as dynamic);
        }

        /***************************************************/
        [Description("Generates a default description for the Profile, based on dimensions.")]
        [Input("profile", "The Profile to generate a default description for.")]
        [Output("desc", "The generated description for the Profile based on its dimensions.")]
        public static string IDescription(this IProfile profile)
        {
            if (profile == null)
                return "null profile";
            return Description(profile as dynamic);
        }

        /***************************************************/
        [Description("Generates a default description for the SurfaceProperty, based on type, dimensions and material.")]
        [Input("property", "The SurfaceProperty to generate a default description for.")]
        [Output("desc", "The generated description for the SurfaceProperty based on its dimensions, material and type.")]
        public static string IDescription(this ISurfaceProperty property)
        {
            if (property == null)
                return "null property";
            return Description(property as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static string Description(IObject obj)
        {
            return "";
        }

        /***************************************************/
        private static string CheckGetMaterialName(IMaterialFragment material)
        {
            if (material == null)
                return "No material";
            if (string.IsNullOrEmpty(material.Name))
                return "Unnamed Material";
            return material.Name;
        }

        /***************************************************/
        private static string MaterialAnalyticalValues(this IIsotropic isotropicMaterial)
        {
            return $"E: {isotropicMaterial.YoungsModulus:G3}, " + $"ρ: {isotropicMaterial.Density:G3}, " + $"ν: {isotropicMaterial.PoissonsRatio:G3}, " + $"α: {isotropicMaterial.ThermalExpansionCoeff:G3} , " + $"ζ: {isotropicMaterial.DampingRatio:G3}";
        }

        /***************************************************/
        private static string MaterialAnalyticalValues(this IOrthotropic orthoTropicMaterial)
        {
            return $"E: {orthoTropicMaterial.YoungsModulus.VectorComponents()}, " + $"ρ: {orthoTropicMaterial.Density:G3}, " + $"ν: {orthoTropicMaterial.PoissonsRatio.VectorComponents()}," + $"α: {orthoTropicMaterial.ThermalExpansionCoeff.VectorComponents()}," + $"ζ: {orthoTropicMaterial.DampingRatio:G3}";
        }

        /***************************************************/
        private static string VectorComponents(this Vector vector)
        {
            if (vector == null)
                return null;
            return (vector != null) ? $"[{vector.X:G3}, {vector.Y:G3}, {vector.Z:G3}]" : "null";
        }

        /***************************************************/
        [Description("Returns a text sign for the DOFType.")]
        private static string DofSign(this DOFType dof)
        {
            switch (dof)
            {
                case DOFType.Free:
                    return "o";
                case DOFType.Fixed:
                    return "x";
                case DOFType.FixedNegative:
                    return "x⁻";
                case DOFType.FixedPositive:
                    return "x⁺";
                case DOFType.Spring:
                    return "e";
                case DOFType.SpringNegative:
                    return "e⁻";
                case DOFType.SpringPositive:
                    return "e⁺";
                case DOFType.SpringRelative:
                    return "r";
                case DOFType.SpringRelativeNegative:
                    return "r⁻";
                case DOFType.SpringRelativePositive:
                    return "r⁺";
                case DOFType.NonLinear:
                    return "n";
                case DOFType.Friction:
                    return "fᵣ";
                case DOFType.Damped:
                    return "d";
                case DOFType.Gap:
                    return "g";
                default:
                    return "-";
            }
        }

        /***************************************************/
        [Description("Gets a shortened version of the timber type name.")]
        private static string IAcronymTypeName(this ITimber material)
        {
            if (material == null)
                return "null material";
            return AcronymTypeName(material as dynamic);
        }

        /***************************************************/
        [Description("Gets a shortened version of the timber type name.")]
        private static string AcronymTypeName(this SawnTimber material)
        {
            if (material == null)
                return "null material";
            return "Timber";
        }

        /***************************************************/
        [Description("Gets a shortened version of the timber type name.")]
        private static string AcronymTypeName(this Glulam material)
        {
            if (material == null)
                return "null material";
            return "GL";
        }

        /***************************************************/
        [Description("Gets a shortened version of the timber type name.")]
        private static string AcronymTypeName(this LaminatedVeneerLumberCrossbands material)
        {
            if (material == null)
                return "null material";
            return "LVL Crossband";
        }

        /***************************************************/
        [Description("Gets a shortened version of the timber type name.")]
        private static string AcronymTypeName(this LaminatedVeneerLumberParallel material)
        {
            if (material == null)
                return "null material";
            return "LVL Parallel";
        }

        /***************************************************/
        [Description("Gets a shortened version of the timber type name.")]
        private static string AcronymTypeName(this ITimber material)
        {
            if (material == null)
                return "null material";
            //Fallback to type name
            return material.GetType().Name;
        }
    /***************************************************/
    }
}