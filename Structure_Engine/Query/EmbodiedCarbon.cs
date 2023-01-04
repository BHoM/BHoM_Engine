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

using BH.oM.Structure.Elements;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [ToBeRemoved("4.0","Method ToBeRemoved as part of removal of EmbodiedCarbon from MaterialFragments to avoid conflicts and overlap with LCA_Toolkit.")]
        [Description("Calculates the total amount of embodied carbon of a Bar by getting the mass of the Bar (generally as section area*length*density) multiplied by the EmbodiedCarbon value of the material.")]
        [Input("bar", "The Bar to get the total embodied carbon from.")]
        [Output("embodiedCarbon", "The total embodied carbon of the Bar.", typeof(Mass))]
        public static double EmbodiedCarbon(this Bar bar)
        {
            Engine.Base.Compute.RecordWarning("Method ToBeRemoved as part of removal of EmbodiedCarbon from MaterialFragments to avoid conflicts and overlap with LCA_Toolkit.");
            return 0;
        }

        /***************************************************/

        [ToBeRemoved("4.0", "Method ToBeRemoved as part of removal of EmbodiedCarbon from MaterialFragments to avoid conflicts and overlap with LCA_Toolkit.")]
        [Description("Calculates the total amount of embodied carbon of a Panel by getting the mass of the Panel (generally as area*thickness*density) multiplied by the EmbodiedCarbon value of the material.")]
        [Input("panel", "The Panel to get the total embodied carbon from.")]
        [Output("embodiedCarbon", "The total embodied carbon of the Panel.", typeof(Mass))]
        public static double EmbodiedCarbon(this Panel panel)
        {
            Engine.Base.Compute.RecordWarning("Method ToBeRemoved as part of removal of EmbodiedCarbon from MaterialFragments to avoid conflicts and overlap with LCA_Toolkit.");
            return 0;
        }

        /***************************************************/

    }
}




