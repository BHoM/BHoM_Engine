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

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the coefficient of performance given the EER and watts input.")]
        [Input("EER", "Equipment EER value")]
        [Input("wattsInput", "Equipment watts input value")]
        [Output("coefficientOfPerformanceEER", "The coefficient of performance (COP)")]
        public static double CoefficientOfPerformanceEER(double EER, double wattsInput)
        {
            if(EER == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the COP from a null btuOutput value");
                return -1;
            }

            if(wattsInput == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the COP from a null btuInput value");
                return -1;
            }

            double coefficientOfPerformanceEER = EER/wattsInput;


            return coefficientOfPerformanceEER;
        }

        /***************************************************/

    }
}
