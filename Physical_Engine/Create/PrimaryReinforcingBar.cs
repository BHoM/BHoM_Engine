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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Physical.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a physical reinforcement element. Bend radius will be calculated based on diameter")]
        [InputFromProperty("centreCurve")]
        [InputFromProperty("diameter")]
        [InputFromProperty("material")]
        [Output("PrimaryReinforcingBar", "The created physical Primary Reinforcing bar")]
        public static PrimaryReinforcingBar PrimaryReinforcingBar(ICurve centreCurve, double diameter, Material material)
        {
            return new PrimaryReinforcingBar
            {
                CentreCurve = centreCurve,
                Diameter = diameter,
                Material = material,
                BendRadius = diameter < 0.02 ? 5 * diameter : 7 * diameter //based on PN-EN 1992-1-1 to be discussed how to unify it or select specific standard
            };
        }

        /***************************************************/

        [Description("Creates a physical reinforcement element")]
        [InputFromProperty("centreCurve")]
        [InputFromProperty("diameter")]
        [InputFromProperty("material")]
        [InputFromProperty("bendRadius")]
        [Output("PrimaryReinforcingBar", "The created physical Primary Reinforcing bar")]
        public static PrimaryReinforcingBar PrimaryReinforcingBar(ICurve centreCurve, double diameter, Material material, double bendRadius)
        {
            return new PrimaryReinforcingBar
            {
                CentreCurve = centreCurve,
                Diameter = diameter,
                Material = material,
                BendRadius = bendRadius
            };
        }

        /***************************************************/
    }
}
