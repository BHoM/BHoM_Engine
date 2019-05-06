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
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Reflection;
using BH.oM.Common.Materials;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConcreteSection ConcreteRectangleSection(double height, double width, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.RectangleProfile(height, width, 0), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteTSection(double height, double webThickness, double flangeWidth, double flangeThickness, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.TSectionProfile(height, flangeWidth, webThickness, flangeThickness, 0, 0), material, name, reinforcement);
        }


        /***************************************************/

        public static ConcreteSection ConcreteCircularSection(double diameter, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.CircleProfile(diameter), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteFreeFormSection(List<ICurve> edges, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.FreeFormProfile(edges), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteSectionFromProfile(IProfile profile, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            Output<IProfile, Dictionary<string, object>> result = Compute.Integrate(profile, Tolerance.MicroDistance);

            profile = result.Item1;
            Dictionary<string, object> constants = result.Item2;

            constants["J"] = profile.ITorsionalConstant();
            constants["Iw"] = profile.IWarpingConstant();

            ConcreteSection section = new ConcreteSection(profile,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Wely"], (double)constants["Welz"], (double)constants["Wply"], (double)constants["Wplz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            //section.Material = material == null ? Query.Default(MaterialType.Concrete) : material;

            if (!string.IsNullOrWhiteSpace(name))
                section.Name = name;
            else if (!string.IsNullOrWhiteSpace(profile.Name))
                section.Name = profile.Name;

            if (reinforcement != null)
                section.Reinforcement = reinforcement;

            return section;
        }

        /***************************************************/
    }
}
