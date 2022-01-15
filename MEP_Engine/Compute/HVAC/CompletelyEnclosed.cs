/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.Fixtures;
using BH.oM.Architecture.Elements;
using BH.Engine.Reflection;
using System;

namespace BH.Engine.MEP.HVAC.ASHRAE_15
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the airflow required for ventilation of completely enclosed mechanical rooms with refrigeration equipment per ASHRAE 15, Part 8.")]
        [Input("massOfRefrigerant", "mass of refrigerant of largest sysem [lbs]")]
        [Output("ventilationAirFlow", "exhaust air flow rate required [CFM]")]
        public static double CompletelyEnclosed(double massOfRefrigerant)
        {
            if(massOfRefrigerant == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the ACH from a null mass of refrigerant value");
                return -1;
            }


            double ventilationAirFlow = 100 * Math.Pow(massOfRefrigerant, 0.5);


            return ventilationAirFlow;
        }

        /***************************************************/

    }
}
