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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Results;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("The utilisation of the factor of safety, calculated as RequiredFactorOfSafety divided by FactorOfSafety. A value less than or equal to 1.0 indicates a satisfactory design.")]
        [Input("result", "DesignResult containing resistance, action and required factor of safety.")]
        [Output("FactorOfSafetyUtilization", "The utilisation of the factor of safety.", typeof(Ratio))]
        public static double FactorOfSafetyUtilisation(this DesignResult result)
        {
            if (result == null || result.RequiredFactorOfSafety == double.NaN || result.FactorOfSafety() == double.NaN)
            {
                Base.Compute.RecordError("Design result information is null or invalid.");
                return double.NaN;
            }
            if (result.FactorOfSafety() == 0.0)
            {
                Base.Compute.RecordError("Factor of safety is zero, cannot calculate factor of safety utilisation for this design result.");
                return double.NaN;
            }
            return result.RequiredFactorOfSafety / result.FactorOfSafety();
        }
        /***************************************************/
    }
}

