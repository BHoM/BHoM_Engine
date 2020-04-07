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

using BH.oM.Geometry.ShapeProfiles;
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
using BH.oM.Reflection.Attributes;
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

        [Description("Generates a default description for the profile as 'I Hegiht x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this ISectionProfile profile)
        {
            return "I " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Box Hegiht x Width x Thickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this BoxProfile profile)
        {
            return "Box " + profile.Height + "x" + profile.Width + "x" + profile.Thickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Angle Hegiht x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this AngleProfile profile)
        {
            return "Angle " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Channel Hegiht x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this ChannelProfile profile)
        {
            return "Channel " + profile.Height + "x" + profile.FlangeWidth + "x" + profile.WebThickness + "x" + profile.FlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Circle Diameter'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this CircleProfile profile)
        {
            return "Circle " + profile.Diameter;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'FabBox Hegiht x Width x WebThickness x TopFlangeThickness x BotFlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this FabricatedBoxProfile profile)
        {
            return "FabBox " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.TopFlangeThickness + "x" + profile.BotFlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'GenFabBox Hegiht x TotalWidth x WebThickness x TopFlangeThickness x BotFlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this GeneralisedFabricatedBoxProfile profile)
        {
            double width = Math.Max(profile.Width + profile.BotLeftCorbelWidth + profile.BotRightCorbelWidth, profile.Width + profile.TopLeftCorbelWidth + profile.TopRightCorbelWidth);
            return "GenFabBox " + profile.Height + "x" + width + "x" + profile.WebThickness + "x" + profile.TopFlangeThickness + "x" + profile.BotFlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Kite Angle x Width x Thickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated description for the profile depending on its dimensions.")]
        public static string Description(this KiteProfile profile)
        {
            return "Kite " + Math.Round(profile.Angle1, 2) + "x" + profile.Width1 + "x" + profile.Thickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'FabI Hegiht x TopFlangeWidth x BotFLangeWidth x WebThickness x TopFlangeThickness x BotFlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this FabricatedISectionProfile profile)
        {
            return "FabI " + profile.Height + "x" + profile.TopFlangeWidth + "x" + profile.BotFlangeWidth + "x" + profile.WebThickness + "x" + profile.TopFlangeThickness + "x" + profile.BotFlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'FreeForm'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this FreeFormProfile profile)
        {
            return "FreeForm";
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Rectangle Hegiht x Width x CornerRadius'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this RectangleProfile profile)
        {
            return "Rectangle " + profile.Height + "x" + profile.Width + "x" + profile.CornerRadius;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'T Hegiht x Width x WebThickness x FlangeThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this TSectionProfile profile)
        {
            return "T " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'GenT  Height x WebThickness x LeftOutstandWidth x LeftOutstandThickness x RightOutstandWidth x RightOutstandThickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this GeneralisedTSectionProfile profile)
        {
            return "GenT " + profile.Height + "x" + profile.WebThickness + "x" + profile.LeftOutstandWidth + "x" + profile.LeftOutstandThickness + "x" + profile.RightOutstandWidth + "x" + profile.RightOutstandThickness;
        }

        /***************************************************/

        [Description("Generates a default description for the profile as 'Tube Diameter x Thickness'.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string Description(this TubeProfile profile)
        {
            return "Tube " + profile.Diameter + "x" + profile.Thickness;
        }

        /***************************************************/
        /**** Public Methods - Sections                 ****/
        /***************************************************/

        [Description("Generates a default description for the Section as 'Steel ProfileDescription - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this SteelSection section)
        {
            return "Steel " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the Section as 'Concrete ProfileDescription - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this ConcreteSection section)
        {
            return "Concrete " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the Section as 'Timber ProfileDescription - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this TimberSection section)
        {
            return "Timber  " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the Section as 'Aluminium ProfileDescription - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this AluminiumSection section)
        {
            return "Aluminium " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the Section as 'Generic ProfileDescription - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this GenericSection section)
        {
            return "Generic " + section.SectionProfile.IDescription() + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the Section as 'Cable NumberOfCables x Diameter - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this CableSection section)
        {
            return "Cable " + section.NumberOfCables + " x dia " + section.CableDiameter + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the Section as 'Explicit Area Iy Iz J  - MaterialName'.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated descritpion for the section depending on its dimensions, material and type.")]
        public static string Description(this ExplicitSection section)
        {
            return "Explicit A: " + section.Area + " Iy: " + section.Iy + " Iz: " + section.Iz + " J: " + section.J + " - " + CheckGetMaterialName(section.Material);
        }

        /***************************************************/
        /**** Public Methods - Surface Properties       ****/
        /***************************************************/

        [Description("Generates a default description for the SurfaceProperty as 'THK Thickness - MaterialName'.")]
        [Input("property", "The SurfaceProperty to get a default description for.")]
        [Output("desc", "The generated description for the property depending on its dimensions, material and type.")]
        public static string Description(this ConstantThickness property)
        {
            return "THK " + property.Thickness + " - " + CheckGetMaterialName(property.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the SurfaceProperty as 'Ribbed Depth Spacing StemWidth - MaterialName'.")]
        [Input("property", "The SurfaceProperty to get a default description for.")]
        [Output("desc", "The generated descritpion for the property depending on its dimensions, material and type.")]
        public static string Description(this Ribbed property)
        {
            return "Ribbed Depth:" + property.TotalDepth + " Spacing: " + property.Spacing + " sWidth: " + property.StemWidth + " - " + CheckGetMaterialName(property.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the SurfaceProperty as 'Ribbed DepthX DepthY SpacingX SpacingY StemWidthX StemWidthY - MaterialName'.")]
        [Input("property", "The SurfaceProperty to get a default description for.")]
        [Output("desc", "The generated descritpion for the property depending on its dimensions, material and type.")]
        public static string Description(this Waffle property)
        {
            return "Waffle DepthX: " + property.TotalDepthX + " DepthY: " + property.TotalDepthY + " SpacingX: " + property.SpacingX + " SpacingY: " + property.SpacingY + " sWidthX: " + property.StemWidthX + " sWidthY: " + property.StemWidthY + " - " + CheckGetMaterialName(property.Material);
        }

        /***************************************************/

        [Description("Generates a default description for the SurfaceProperty as 'LoadingPanel LoadApplication ReferenceEdge'.")]
        [Input("property", "The SurfaceProperty to get a default description for.")]
        [Output("desc", "The generated descritpion for the property.")]
        public static string Description(this LoadingPanelProperty property)
        {
            return "LoadingPanel Application: " + property.LoadApplication + " RefEdge: " + property.ReferenceEdge;
        }

        /***************************************************/
        /**** Public Methods - Constraints              ****/
        /***************************************************/

        [Description("Generates a default description for the Constraint3DOF as 'DOftypes-Values'.")]
        [Input("constraint", "The Constraint3DOF to get a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Constraint3DOF constraint)
        {
            string desc = constraint.UX.DofSign() + constraint.UY.DofSign() + constraint.Normal.DofSign();
            if (constraint.ElasticValuesWritten())
            {
                desc += "-" + constraint.KX + "," + constraint.KY + "," + constraint.KNorm; 
            }
            return desc;
        }

        /***************************************************/

        [Description("Generates a default description for the Constraint4DOF as 'DOftypes-Values'.")]
        [Input("constraint", "The Constraint4DOF to get a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Constraint4DOF constraint)
        {
            string desc = constraint.TranslationX.DofSign() + constraint.TranslationY.DofSign() + constraint.TranslationZ.DofSign() +
                          constraint.RotationX.DofSign();

            if (constraint.ElasticValuesWritten())
            {
                desc += "-" + constraint.TranslationalStiffnessX + "," + constraint.TranslationalStiffnessY + "," + constraint.TranslationalStiffnessZ +
                        "," + constraint.RotationalStiffnessX;
            }
            return desc;
        }

        /***************************************************/

        [Description("Generates a default description for the Constraint6DOF as 'DOftypes-Values'.")]
        [Input("constraint", "The Constraint6DOF to get a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Constraint6DOF constraint)
        {
            string desc = constraint.TranslationX.DofSign() + constraint.TranslationY.DofSign() + constraint.TranslationZ.DofSign() +
                          constraint.RotationX.DofSign() + constraint.RotationY.DofSign() + constraint.RotationZ.DofSign();

            if (constraint.ElasticValuesWritten())
            {
                desc += "-" + constraint.TranslationalStiffnessX + "," + constraint.TranslationalStiffnessY + "," + constraint.TranslationalStiffnessZ +
                        "," + constraint.RotationalStiffnessX +  "," + constraint.RotationalStiffnessY +  "," + constraint.RotationalStiffnessZ;
            }
            return desc;
        }

        /***************************************************/

        [Description("Generates a default description for the BarRelease as 'StartReleaseNameOrDesc - EndReleaseNameOrDesc'.")]
        [Input("release", "The Constraint6DOF to get a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this BarRelease release)
        {
            return release.StartRelease.DescriptionOrName() + " - " + release.EndRelease.DescriptionOrName();
        }

        /***************************************************/
        /**** Public Methods - Offset                   ****/
        /***************************************************/

        [Description("Generates a default description for the Offset as 'Start - End'.")]
        [Input("offset", "The Constraint6DOF to get a default description for.")]
        [Output("desc", "The generated description for the constraint.")]
        public static string Description(this Offset offset)
        {
            return offset.Start.VectorComponents() + " - " + offset.End.VectorComponents();
        }

        /***************************************************/
        /**** Public Methods - Material                 ****/
        /***************************************************/

        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to get a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Steel material)
        {
            return "Steel " + material.MaterialAnalyticalValues() + ", fy:" + material.YieldStress + ", fu:" + material.UltimateStress;
        }

        /***************************************************/

        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to get a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Concrete material)
        {
            return "Concrete " + material.MaterialAnalyticalValues() + ", cyl:" + material.CylinderStrength + ", cube:" + material.CubeStrength;
        }

        /***************************************************/

        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to get a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Aluminium material)
        {
            return "Alum " + material.MaterialAnalyticalValues();
        }

        /***************************************************/

        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to get a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this GenericIsotropicMaterial material)
        {
            return "Gen " + material.MaterialAnalyticalValues();
        }

        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to get a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this Timber material)
        {
            return "Timber " + material.MaterialAnalyticalValues();
        }

        [Description("Generates a default description for the material based on its properties.")]
        [Input("material", "The material to get a default description for.")]
        [Output("desc", "The generated description for the material.")]
        public static string Description(this GenericOrthotropicMaterial material)
        {
            return "Gen " + material.MaterialAnalyticalValues();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Generates a default description for the IProperty, based on its properties.")]
        [Input("property", "The property to get a default description for.")]
        [Output("desc", "The generated description for the property, depending on its property values.")]
        public static string IDescription(this IProperty property)
        {
            return Description(property as dynamic);
        }

        /***************************************************/

        [Description("Generates a default description for the Section, depending on type, profile and material.")]
        [Input("section", "The section to get a default description for.")]
        [Output("desc", "The generated description for the section depending on its dimensions, material and type.")]
        public static string IDescription(this ISectionProperty section)
        {
            return Description(section as dynamic);
        }

        /***************************************************/

        [Description("Generates a default description for the Profile, depending on dimensions.")]
        [Input("profile", "The profile to get a default description for.")]
        [Output("desc", "The generated descritpion for the profile depending on its dimensions.")]
        public static string IDescription(this IProfile profile)
        {
            return Description(profile as dynamic);
        }

        /***************************************************/

        [Description("Generates a default description for the Section, depending on type, dimensions and material.")]
        [Input("property", "The SurfaceProperty to get a default description for.")]
        [Output("desc", "The generated descritpion for the property depending on its dimensions, material and type.")]
        public static string IDescription(this ISurfaceProperty property)
        {
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
            return "E:" + isotropicMaterial.YoungsModulus +
                   ", ρ:" + isotropicMaterial.Density +
                   ", ν:" + isotropicMaterial.PoissonsRatio +
                   ", α:" + isotropicMaterial.ThermalExpansionCoeff +
                   ", ζ:" + isotropicMaterial.DampingRatio;
        }

        /***************************************************/

        private static string MaterialAnalyticalValues(this IOrthotropic orthoTropicMaterial)
        {
            return "E:" + orthoTropicMaterial.YoungsModulus.VectorComponents() +
                   ", ρ:" + orthoTropicMaterial.Density +
                   ", ν:" + orthoTropicMaterial.PoissonsRatio.VectorComponents() +
                   ", α:" + orthoTropicMaterial.ThermalExpansionCoeff.VectorComponents() +
                   ", ζ:" + orthoTropicMaterial.DampingRatio;
        }

        /***************************************************/

        private static string VectorComponents(this Vector vector)
        {
            return "[" + vector.X + "," + vector.Y + "," + vector.Z + "]";
        }

        /***************************************************/

        [Description("Returns a text sign for the DOFType.")]
        private static string DofSign(this DOFType dof)
        {
            switch (dof)
            {
                case DOFType.Free:
                    return "f";
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

        [Description("Returns true if any of the dof elements is of a value based type or if any of the values are non-zero.")]
        private static bool ElasticValuesWritten(this Constraint3DOF constraint)
        {
            if (constraint.UX.ValueBasedDOF())
                return true;

            if (constraint.UY.ValueBasedDOF())
                return true;

            if (constraint.Normal.ValueBasedDOF())
                return true;

            if (constraint.KX != 0)
                return true;

            if (constraint.KY != 0)
                return true;

            if (constraint.KNorm != 0)
                return true;

            return false;
        }

        /***************************************************/

        [Description("Returns true if any of the dof elements is of a value based type or if any of the values are non-zero.")]
        private static bool ElasticValuesWritten(this Constraint4DOF constraint)
        {
            if (constraint.TranslationX.ValueBasedDOF())
                return true;

            if (constraint.TranslationY.ValueBasedDOF())
                return true;

            if (constraint.TranslationZ.ValueBasedDOF())
                return true;

            if (constraint.RotationX.ValueBasedDOF())
                return true;

            if (constraint.TranslationalStiffnessX != 0)
                return true;

            if (constraint.TranslationalStiffnessY != 0)
                return true;

            if (constraint.TranslationalStiffnessZ != 0)
                return true;

            if (constraint.RotationalStiffnessX != 0)
                return true;

            return false;
        }

        /***************************************************/

        [Description("Returns true if any of the dof elements is of a value based type or if any of the values are non-zero.")]
        private static bool ElasticValuesWritten(this Constraint6DOF constraint)
        {
            if (constraint.TranslationX.ValueBasedDOF())
                return true;

            if (constraint.TranslationY.ValueBasedDOF())
                return true;

            if (constraint.TranslationZ.ValueBasedDOF())
                return true;

            if (constraint.RotationX.ValueBasedDOF())
                return true;

            if (constraint.RotationY.ValueBasedDOF())
                return true;

            if (constraint.RotationZ.ValueBasedDOF())
                return true;

            if (constraint.TranslationalStiffnessX != 0)
                return true;

            if (constraint.TranslationalStiffnessY != 0)
                return true;

            if (constraint.TranslationalStiffnessZ != 0)
                return true;

            if (constraint.RotationalStiffnessX != 0)
                return true;

            if (constraint.RotationalStiffnessY != 0)
                return true;

            if (constraint.RotationalStiffnessZ != 0)
                return true;

            return false;
        }

        /***************************************************/

        [Description("Returns true if the DOF is of a type that depends on a stiffness value.")]
        private static bool ValueBasedDOF(this DOFType dof)
        {
            switch (dof)
            {
                case DOFType.Free:
                case DOFType.Fixed:
                case DOFType.FixedNegative:
                case DOFType.FixedPositive:
                    return false;
                case DOFType.SpringNegative:
                case DOFType.SpringPositive:
                case DOFType.SpringRelative:
                case DOFType.SpringRelativeNegative:
                case DOFType.SpringRelativePositive:
                case DOFType.NonLinear:
                case DOFType.Friction:
                case DOFType.Damped:
                case DOFType.Gap:
                default:
                    return true;
            }
        }

        /***************************************************/
    }
}
