/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Spatial;
using BH.oM.MEP.Enums;
using BH.oM.MEP.System;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the Materials an object is composed of and in which ratios.")]
        [Input("obj", "The object to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the object is composed of and in which ratios.", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this IFlow obj)
        {
            if (obj.SectionProfile == null)
            {
                Engine.Reflection.Compute.RecordError("The object's MaterialComposition could not be calculated as no SectionProfile has been assigned.");
                return null;
            }

            List<Material> materials = null;
            List<double> ratios = null;
            List<IProfile> profiles = null;

            materials = new List<Material>() {
                obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Material).First(), obj.SectionProfile.Where(x => x.Type == ProfileType.Lining).First().Layer.Select(x => x.Material).First(), obj.SectionProfile.Where(x => x.Type == ProfileType.Insulation).First().Layer.Select(x => x.Material).First() 
            };

            profiles = GetSectionProfiles(obj);
            List<double> areas = null;
            areas = profiles.Select(x => x.Area()).ToList();

            //This will only work with all three areas accounted for
            ratios = new List<double>() {
                areas[0], areas[1], areas[2]
            };

            return Matter.Create.MaterialComposition(materials, ratios);
        }

        /***************************************************/
    }
}