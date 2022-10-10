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

namespace BH.Engine.MEP.Electrical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates MHP (Motor Horsepower) for single phase equipment from BHP and motor-drive efficiency")]
        [Input("brakeHorsepower", "brakeHorsepower [hp] of single phase equipment")]
        [Input("motor_driveEfficiency", "motor-drive efficiency in decimal form (ex. 0.8)")]
        [Output("MHP", "[hp]")]
        public static double SinglePhaseMHP(double brakeHorsepower, double motor_driveEfficiency)
        {
            if(brakeHorsepower == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null brake horsepower value");
                return -1;
            }

            if (motor_driveEfficiency == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null motor-drive efficiency value");
                return -1;
            }


            double MHP = (brakeHorsepower)/motor_driveEfficiency;


            return MHP;
        }

        /***************************************************/

    }
}
