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

using BH.oM.Common.Materials;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Deprecated("2.3", "Material class superseded by counterpart in Physical_oM, BH.oM.Phsyical.Materials.Material")]
        public static Material Material(string name, MaterialType type = MaterialType.Steel, double E = 210000000000, double v = 0.3, double tC = 0.000012, double density = 7850)
        {
            return new Material
            {
                Name = name,
                Type = type,
                YoungsModulus = E,
                PoissonsRatio = v,
                CoeffThermalExpansion = tC,
                Density = density
            };
        }

        /***************************************************/
    }
}

