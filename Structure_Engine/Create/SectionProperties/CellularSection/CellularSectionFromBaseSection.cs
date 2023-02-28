/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates the maximum possible Cellular section from the provided SteelSection and opening.")]
        [Input("baseSEction", "The prismatic steel section being turned into a cellular section. The Steel section needs to have a profile of type ISectionProfile. Material from steel section will be applied to the created cellular section.")]
        [Input("opening", "The openings in the finished cellular section. Opening setting will be used to produce as tight cut as possible on the base section, producing the cellular section with maximum possible height.")]
        [Input("cutThickness", "Additional reduction factor on the final height of the section. Reduction based on thickness of the cutting process when producing the cellular section.", typeof(Length))]
        [InputFromProperty("name")]
        [Output("section", "Created cellular section.")]
        public static CellularSection CellularSectionFromBaseSection(SteelSection baseSection, ICellularOpening opening, double cutThickness = 0, string name = "")
        {
            ISectionProfile baseProfile = baseSection.SectionProfile as ISectionProfile;

            if (baseProfile == null)
            {
                Engine.Base.Compute.RecordError($"Can only create Cellular beams from SteelSections with profiles of type {nameof(ISectionProfile)}");
                return null;
            }
            double openingCutHeight = opening.IHeight();
            double openingAddition = opening.IHeightAddition();


            double totalHeight = openingCutHeight / 2 + openingAddition + baseProfile.Height - opening.ICutReduction() - cutThickness;
           
            VoidedISectionProfile openingProfile = Spatial.Create.VoidedISectionProfile(totalHeight, openingCutHeight + openingAddition, baseProfile.Width, baseProfile.WebThickness, baseProfile.FlangeThickness, baseProfile.RootRadius, baseProfile.ToeRadius);

            if (openingProfile == null)
                return null;
            
            
            ISectionProfile solidProfile = Spatial.Create.ISectionProfile(totalHeight, baseProfile.Width, baseProfile.WebThickness, baseProfile.FlangeThickness, baseProfile.RootRadius, baseProfile.ToeRadius);

            return CellularSection(openingProfile, solidProfile, opening, baseProfile, baseSection.Material as Steel, name);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static CellularSection CellularSection(VoidedISectionProfile openingProfile, ISectionProfile solidProfile, ICellularOpening opening, ISectionProfile baseProfile, Steel material, string name = "")
        {
            Dictionary<string, double> constantsOpen = Compute.IntegrateSection(openingProfile.Edges.ToList(), Tolerance.MicroDistance, computeShearArea : false);
            Dictionary<string, double> constantsClosed = Compute.IntegrateSection(solidProfile.Edges.ToList(), Tolerance.MicroDistance, computeShearArea: false);

            constantsOpen["J"] = openingProfile.ITorsionalConstant();
            constantsOpen["Iw"] = openingProfile.IWarpingConstant();
            constantsOpen["Asz"] = openingProfile.WebThickness * (openingProfile.Height - openingProfile.OpeningHeight - 2 * openingProfile.FlangeThickness);
            constantsOpen["Asy"] = 2 * openingProfile.Width * openingProfile.FlangeThickness;

            Dictionary<string, double> constants = constantsOpen;

            double spacing = opening.Spacing;
            double widthWebPost = opening.WidthWebPost;

            //Major axis constants as arimethric means
            constants["Iy"] = (widthWebPost * constantsClosed["Iy"] + (spacing - widthWebPost) * constantsOpen["Iy"]) / spacing;
            constants["Rgy"] = (widthWebPost * constantsClosed["Rgy"] + (spacing - widthWebPost) * constantsOpen["Rgy"]) / spacing;


            CellularSection section = new CellularSection(openingProfile, solidProfile, opening, baseProfile, constants["Area"], constants["Rgy"], constants["Rgz"], constants["J"], constants["Iy"], constants["Iz"], constants["Iw"], constants["Wely"],
                constants["Welz"], constants["Wply"], constants["Wplz"], constants["CentreZ"], constants["CentreY"], constants["Vz"],
                constants["Vpz"], constants["Vy"], constants["Vpy"], constants["Asy"], constants["Asz"]);

            return PostProcessSectionCreate(section, name, material, MaterialType.Steel);
        }

        /***************************************************/

        private static double IHeight(this ICellularOpening opening)
        {
            return Height(opening as dynamic);
        }

        /***************************************************/

        private static double Height(this HexagonalOpening opening)
        {
            return opening.Height;
        }

        /***************************************************/

        private static double Height(this CircularOpening opening)
        {
            return opening.Diameter;
        }

        /***************************************************/

        private static double Height(this SinusoidalOpening opening)
        {
            return opening.Height;
        }

        /***************************************************/

        private static double ICutReduction(this ICellularOpening opening)
        {
            return CutReduction(opening as dynamic);
        }

        /***************************************************/

        private static double CutReduction(this HexagonalOpening opening)
        {
            return 0;
        }

        /***************************************************/

        private static double CutReduction(this CircularOpening opening)
        {
            double r = opening.Diameter / 2;
            double w = opening.WidthWebPost / 2;
            return r - Math.Sqrt(r * r - w * w);
        }

        /***************************************************/

        private static double CutReduction(this SinusoidalOpening opening)
        {
            return 0;
        }

        /***************************************************/

        private static double IHeightAddition(this ICellularOpening opening)
        { 
            return HeightAddition(opening as dynamic);
        }

        /***************************************************/

        private static double HeightAddition(this HexagonalOpening opening)
        {
            return opening.SpacerHeight;
        }

        /***************************************************/

        private static double HeightAddition(this ICellularOpening opening)
        {
            return 0;
        }

        /***************************************************/
    }
}
