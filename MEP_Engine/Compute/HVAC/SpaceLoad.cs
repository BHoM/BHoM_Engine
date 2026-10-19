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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.MEP.Criteria;
using BH.oM.MEP.Elements;
using BH.oM.MEP.Results;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the internal sensible and latent load for each space. Returns one SpaceLoad per space that can be calculated.")]
        [Input("spaces", "Approved calculation spaces.")]
        [Input("criteria", "Rates for this calculation. The criteria name is stored on each result as ResultCase.")]
        [Output("loads", "Internal load for each calculated space.")]
        public static List<SpaceLoad> SpaceLoad(IEnumerable<Space> spaces, HVACSpaceCriteria criteria)
        {
            if (spaces == null || criteria == null)
            {
                Base.Compute.RecordError("Space load requires spaces and criteria.");
                return new List<SpaceLoad>();
            }

            if (!spaces.Any())
                return new List<SpaceLoad>();

            Base.Compute.RecordWarning("Space load is not implemented.");
            return new List<SpaceLoad>();
        }

        /***************************************************/
    }
}
