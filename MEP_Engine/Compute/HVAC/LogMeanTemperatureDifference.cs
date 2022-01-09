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
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.Fixtures;
using BH.oM.Architecture.Elements;
using BH.Engine.Reflection;

namespace BH.Engine.MEP.HVAC
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the log mean temperature difference given the inlet and outlet temperatures of both sides of the heat exchanger.")]
        [Input("type", " input an integer value indicating type. counterfCold [0] or parallel [1] ")]
        [Input("tempHotIn", "tempHotIn [F]")]
        [Input("tempHotOut", "tempHotOut [F]")]
        [Input("tempColdIn", "tempColdIn [F]")]
        [Input("tempColdOut", "tempColdOut [F]")]
        [Output("logMeanTemperatureDifference", "LMTD [F]")]
        public static double LogMeanTemperatureDifference(int type, double tempHotIn, double tempHotOut, double tempColdIn, double tempColdOut)
        {
            if (type != 0 || type != 1)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the LMTD from a null type value. Ensure value is either [0] for counterfCold or [1] for parallel");
                return -1;
            }

            if (tempHotIn == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the LMTD from a null tempHotIn value");
                return -1;
            }
            if (tempHotOut == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the LMTD from a null tempHotOut value");
                return -1;
            }
            if (tempColdIn == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the LMTD from a null tempColdIn value");
                return -1;
            }
            if (tempColdOut == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the LMTD from a null tempColdOut value");
                return -1;
            }

            double logMeanTemperatureDifference = double.NaN;

            if (type == 0)
            {
                logMeanTemperatureDifference = ((tempHotOut-tempColdIn)-(tempHotIn-tempColdOut))/ Math.Log((tempHotOut-tempColdIn)/(tempHotIn-tempColdOut));

            }
            if (type == 1)
            {
                logMeanTemperatureDifference = ((tempHotOut - tempColdOut) - (tempHotIn - tempColdIn)) / Math.Log((tempHotOut - tempColdOut) / (tempHotIn - tempColdIn));

            }


            return logMeanTemperatureDifference;
        }

        /***************************************************/

    }
}
