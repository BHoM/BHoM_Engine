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
using System;
using BH.oM.Base.Attributes;

namespace BH.Engine.MEP.Mechanical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the log mean temperature difference given the inlet and outlet temperatures of both sides of the heat exchanger.")]
        [Input("heatExchangerType", " input an integer value indicating type. counterflow [0] or parallel [1] ")]  //todo: change to enum
        [Input("temperatureHotIn", "temperatureHotIn [F]")]
        [Input("temperatureHotOut", "temperatureHotOut [F]")]
        [Input("temperatureColdIn", "temperatureColdIn [F]")]
        [Input("temperatureColdOut", "temperatureColdOut [F]")]
        [Output("logMeanTemperatureDifference", "LMTD [F]")]
        //todo: change temperature to temperature, change type to heatExchangerType,
        public static double LogMeanTemperatureDifference(int heatExchangerType, double temperatureHotIn, double temperatureHotOut, double temperatureColdIn, double temperatureColdOut)
        {
            if (heatExchangerType != 0 || heatExchangerType!= 1)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the LMTD from a null type value. Ensure value is either [0] for counterfCold or [1] for parallel");
                return -1;
            }

            //if (temperatureHotIn == double.NaN)
            //{
            //    BH.Engine.Base.Compute.RecordError("Cannot compute the LMTD from a null temperatureHotIn value");
            //    return -1;
            //}
            //if (temperatureHotOut == double.NaN)
            //{
            //    BH.Engine.Base.Compute.RecordError("Cannot compute the LMTD from a null temperatureHotOut value");
            //    return -1;
            //}
            //if (temperatureColdIn == double.NaN)
            //{
            //    BH.Engine.Base.Compute.RecordError("Cannot compute the LMTD from a null temperatureColdIn value");
            //    return -1;
            //}
            //if (temperatureColdOut == double.NaN)
            //{
            //    BH.Engine.Base.Compute.RecordError("Cannot compute the LMTD from a null temperatureColdOut value");
            //    return -1;
            //}

            double logMeanTemperatureDifference = 0;

            if (heatExchangerType == 0)
            {
                logMeanTemperatureDifference = ((temperatureHotOut-temperatureColdIn)-(temperatureHotIn-temperatureColdOut))/ Math.Log((temperatureHotOut-temperatureColdIn)/(temperatureHotIn-temperatureColdOut));

            }
            if (heatExchangerType == 1)
            {
                logMeanTemperatureDifference = ((temperatureHotOut - temperatureColdOut) - (temperatureHotIn - temperatureColdIn)) / Math.Log((temperatureHotOut - temperatureColdOut) / (temperatureHotIn - temperatureColdIn));

            }


            return logMeanTemperatureDifference;
        }

        /***************************************************/

    }
}
