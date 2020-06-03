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

        [Description("Creates a physical reinforcement element. Bend radius value is based on Eurocode 1992-1-1. For diameters less than or equal to 0.016m bend diameter will be equal 4 times the stirrups's diameter and 7 times for greater than 0.016m. Note that in the Eurocode there is an inner bend diameter and in Stirrup parameters there is a centerline radius.")]
        [InputFromProperty("centreCurve")]
        [InputFromProperty("diameter")]
        [InputFromProperty("material")]
        [Output("PrimaryReinforcingBar", "The created physical Primary Reinforcing bar.")]
        public static PrimaryReinforcingBar PrimaryReinforcingBar(ICurve centreCurve, double diameter, Material material)
        {
            return new PrimaryReinforcingBar
            {
                CentreCurve = centreCurve,
                Diameter = diameter,
                Material = material,
                BendRadius = diameter <= 0.016 ? (4.5 * diameter) / 2 : (7.5 * diameter) / 2
            };
        }

        /***************************************************/
    }
}
