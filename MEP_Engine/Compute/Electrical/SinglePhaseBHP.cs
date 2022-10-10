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
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP.Electrical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates BHP (Brake Horsepower) for single phase equipment from volts, amps, power factor, and device efficiency.")]
        [Input("voltage", "The voltage of the electrical equipment.")]
        [Input("amperage", "The electric current of the electrical equipment.", typeof(ElectricCurrent))]
        [Input("powerFactor", "The power factor of the electrical equipment.")]
        [Input("deviceEfficiency", "Device efficiency in decimal form (ex. 0.8)")]
        [Output("BHP", "The brake horsepower of the electrical equipment.")]
        public static double SinglePhaseBrakeHorsePower(double voltage, double amperage, double powerFactor, double deviceEfficiency)
        {
            if(voltage == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null voltage value.");
                return -1;
            }

            if (amperage == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null amperage value.");
                return -1;
            }

            if (powerFactor == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null powerFactor value.");
                return -1;
            }

            if (deviceEfficiency == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null device efficiency value.");
                return -1;
            }

            double brakeHorsePower = (voltage * amperage * powerFactor * deviceEfficiency)/746;


            return brakeHorsePower;
        }

        /***************************************************/

    }
}
