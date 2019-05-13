/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Reflection;
using System.Linq;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SteelSection SteelISection(double height, double webThickness, double flangeWidth, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.ISectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelFabricatedISection(double height, double webThickness, double topFlangeWidth, double topFlangeThickness, double botFlangeWidth, double botFlangeThickness,  double weldSize, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize), material, name);
        }

        /***************************************************/

        public static SteelSection SteelBoxSection(double height, double width, double thickness, double innerRadius = 0, double outerRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.BoxProfile(height, width, thickness, outerRadius, innerRadius), material, name);
        }

        /***************************************************/

        public static SteelSection FabricatedSteelBoxSection(double height, double width, double webThickness, double flangeThickness, double weldSize, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.FabricatedBoxProfile(height, width, webThickness, flangeThickness, flangeThickness, weldSize), material, name);


        }

        /***************************************************/

        public static SteelSection SteelTubeSection(double diameter, double thickness, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.TubeProfile(diameter, thickness), material, name);
        }

        /***************************************************/

        public static SteelSection SteelRectangleSection(double height, double width, double cornerRadius=0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.RectangleProfile(height, width, cornerRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelCircularSection(double diameter, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.CircleProfile(diameter), material, name);
        }

        /***************************************************/

        public static SteelSection SteelTSection(double height, double webThickness, double flangeWidth, double flangeThickness,  double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.TSectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);

        }

        /***************************************************/

        public static SteelSection SteelAngleSection(double height, double webThickness, double width, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.AngleProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelFreeFormSection(List<ICurve> edges, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.FreeFormProfile(edges), material, name);
        }

        /***************************************************/

        public static SteelSection SteelSectionFromProfile(IProfile profile, Steel material = null, string name = "")
        {
            Output<IProfile, Dictionary<string, object>> result = Compute.Integrate(profile, Tolerance.MicroDistance);

            profile = result.Item1;
            Dictionary<string, object> constants = result.Item2;

            constants["J"] = profile.ITorsionalConstant();
            constants["Iw"] = profile.IWarpingConstant();

            SteelSection section = new SteelSection(profile,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"], (double)constants["Wely"],
                (double)constants["Welz"], (double)constants["Wply"], (double)constants["Wplz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            if (material == null)
            {
                material = (Steel)Query.Default(oM.Structure.MaterialFragments.MaterialType.Steel);
            }

            section.Material = material;

            if (!string.IsNullOrWhiteSpace(name))
                section.Name = name;
            else if (!string.IsNullOrWhiteSpace(profile.Name))
                section.Name = profile.Name;

            return section;

        }

        /***************************************************/
    }
}
