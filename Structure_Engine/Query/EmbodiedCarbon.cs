/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the total amount of embodied carbon of a Bar by taken the mass of the bar (as section area*length*density) times the EmbodiedCarbon value of the material")]
        [Input("bar", "The Bar to get the total embodied carbon from")]
        [Output("emobodiedCarbon","The total embodied carbon of the bar", typeof(Mass))]
        public static double EmbodiedCarbon(this Bar bar)
        {
            return bar.Mass() * bar.SectionProperty.Material.EmbodiedCarbon;
        }

        /***************************************************/

        [Description("Calculates the total amount of embodied carbon of a Panel by taken the mass of the panel (as area*thickness*density) times the EmbodiedCarbon value of the material")]
        [Input("panel", "The Panel to get the total embodied carbon from")]
        [Output("emobodiedCarbon", "The total embodied carbon of the panel", typeof(Mass))]
        public static double EmbodiedCarbon(this Panel panel)
        {
            return panel.Mass() * panel.Property.Material.EmbodiedCarbon;
        }

        /***************************************************/

    }
}

