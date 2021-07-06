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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties.Reinforcement;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a RebarIntent that can be added to a ConreteSection.")]
        [Input("reinforcement", "A list of LongitudinalReinforcement and/or TransverseReinforcement illustrating the rebar intent.")]
        [InputFromProperty("minimumCover")]
        [Output("rebarIntent", "The created circular concrete section.")]
        public static PanelReinforcement PanelReinforcement(IMaterialFragment material, ReinforcementRegion region, 
            double longitudinalDiameter, double longitudinalSpacing, double longitudinalDepth,
            double transverseDiameter, double transverseSpacing, double transverseDepth, double minimumCover)
        {
            if (material.IsNull()|| region.IsNull())
                return null;

            return new PanelReinforcement() { Material = material, Region = region, 
            LongitudinalDiameter = longitudinalDiameter, LongitudinalSpacing = longitudinalSpacing, LongitudinalDepth = longitudinalDepth,
            TransverseDiameter = transverseDiameter, TransverseSpacing = transverseSpacing, TransverseDepth = transverseDepth, MinimumCover = minimumCover};
        }

        /***************************************************/
    }
}


