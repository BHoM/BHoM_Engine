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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Reinforcement;
using System.ComponentModel;
using BH.Engine.Base;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Adds Reinforcement to a ConcreteSection. Any previous Reinforcement will be kept.")]
        [Input("section", "The concrete section to add Reinforcement to.")]
        [Input("reinforcement", "The collection of Reinforcement to add to the ConcreteSection.")]
        [Output("concSection", "The ConcreteSection with additional Reinforcement.")]
        public static ConcreteSection AddReinforcement(this ConcreteSection section, IEnumerable<IBarReinforcement> reinforcement)
        {
            if (section.IsNull())
                return null;

            ConcreteSection clone = section.ShallowClone();

            if (clone.RebarIntent == null)
                clone.RebarIntent = new BarRebarIntent() { BarReinforcement = new List<IBarReinforcement>() };
            else if (clone.RebarIntent.BarReinforcement == null)
                clone.RebarIntent.BarReinforcement = new List<IBarReinforcement>();

            clone.RebarIntent.BarReinforcement.AddRange(reinforcement);

            return clone;
        }

        /***************************************************/
    }
}




