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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of the normalised locations (0 means start, 1 means end) in the cross section where the reinforcement changes. If section is null or does not contain any reinforcement, an empty list will be returned.")]
        [Input("concreteSection", "The ConcreteSection from which to extract reinforcement transitions.")]
        [Input("tolerance", "Tolerance on how close two transition points should lie to be deemed the same.", typeof(Length))]
        [Output("locations", "The locations of reinforcement transitions in the crossection.")]
        public static List<double> ReinforcementTransitionPoints(this ConcreteSection concreteSection, double tolerance = Tolerance.Distance)
        {
            if (concreteSection == null || concreteSection.RebarIntent == null || concreteSection.RebarIntent.BarReinforcement.IsNullOrEmpty())
            {
                return new List<double>();
            }

            HashSet<int> uniquePositions = new HashSet<int>();

            foreach (IBarReinforcement reinforcement in concreteSection.RebarIntent.BarReinforcement)
            {
                uniquePositions.Add((int)Math.Round((reinforcement.StartLocation / tolerance)));
                uniquePositions.Add((int)Math.Round((reinforcement.EndLocation / tolerance)));
            }

            List<double> positions = uniquePositions.Select(x => x * tolerance).ToList();
            positions.Sort();
            return positions;
        }

        /***************************************************/
    }
}



