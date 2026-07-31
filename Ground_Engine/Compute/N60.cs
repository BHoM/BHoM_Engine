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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Ground;

namespace BH.Engine.Ground
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Calculates the number of blows for the main test drive corrected by the energy ratio.")]
        [Input("spt", "The Standard Penetration Test (SPT) to calculate N60.")]
        [Output("n60", "The number of blows for the main test drive corrected by the energy ratio.")]
        public static double N60(SPT standardPenetrationTest)
        {
            return standardPenetrationTest != null ? N60(standardPenetrationTest.NumberofBlows, standardPenetrationTest.EnergyRatio) : double.NaN;

        }

        /***************************************************/

        [Description("Calculates the number of blowsfor the main test drive corrected by the energy ratio.")]
        [Input("numberOfBlows", "The number of blows from the standard penetration test.")]
        [Input("energyRatio", "The energy ratio of the hammer.")]
        [Output("n60", "The number of blows for the main test drive corrected by the energy ratio.")]
        public static double N60(int numberOfBlows, double energyRatio)
        {
            return numberOfBlows > 0 && energyRatio > 0 ? numberOfBlows * energyRatio / 60 : double.NaN;
        }

        /***************************************************/

    }
}




