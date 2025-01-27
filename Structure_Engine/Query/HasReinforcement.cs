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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Reinforcement;
using BH.oM.Structure.SectionProperties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns true if the Bar has a ConcreteSection with BarRebarIntent defined with at least one IBarReinforcement in it. False if the Bar, ConcreteSection or BarRebarIntent is null or the IBarReinforcement count is zero.")]
        [Input("bar", "The Bar to check for reinforcement.")]
        [Output("hasReinforcement", "Returns true if the Bar has reinforcement assigned, false otherwise.")]
        public static bool HasReinforcement(this Bar bar)
        {
            if (bar == null)
                return false;

            ConcreteSection section = bar.SectionProperty as ConcreteSection;
            return HasReinforcement(section);
        }

        /***************************************************/

        [Description("Returns true if the ConcreteSection has BarRebarIntent defined with at least one IBarReinforcement in it.  False if the ConcreteSection or BarRebarIntent is null or the IBarReinforcement count is zero.")]
        [Input("section", "The ConcreteSection to check for reinforcement.")]
        [Output("hasReinforcement", "Returns true if the ConcreteSection has reinforcement assigned, false otherwise.")]
        public static bool HasReinforcement(this ConcreteSection section)
        {
            if (section == null)
                return false;

            if (section.RebarIntent == null)
                return false;

            if (section.RebarIntent.BarReinforcement == null || section.RebarIntent.BarReinforcement.Count == 0)
                return false;

            return true;
        }

        /***************************************************/
    }
}



