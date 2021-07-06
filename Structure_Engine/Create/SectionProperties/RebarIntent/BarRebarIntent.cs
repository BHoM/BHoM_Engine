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
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using System.Linq;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

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
        public static BarRebarIntent BarRebarIntent(List<IBarReinforcement> reinforcement, double minimumCover = 0)
        {
            if (reinforcement.IsNullOrEmpty() || reinforcement.Any(x => x.IsNull()))
                return null;

            return new BarRebarIntent() { BarReinforcement = reinforcement, MinimumCover = minimumCover };
        }

        /***************************************************/
    }
}


