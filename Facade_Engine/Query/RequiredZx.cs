/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Facade.Enums;
using System;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double RequiredZx(SupportType supportType, double windLoad, double tributaryWidth, double length, double yieldStress, BuildingCode buildingCode)
        {
            if (tributaryWidth <= 0)
            {
                BH.Engine.Base.Compute.RecordError("Tributary width needs to be a positive number.");
                return double.NaN;
            }

            if (length <= 0)
            {
                BH.Engine.Base.Compute.RecordError("Length needs to be a positive number.");
                return double.NaN;
            }

            if (yieldStress <= 0)
            {
                BH.Engine.Base.Compute.RecordError("Yield stress needs to be a positive number.");
                return double.NaN;
            }

            double linearLoad = tributaryWidth * windLoad;


            double linearLoadWithFactor;
            if (buildingCode == BuildingCode.BSEN19902002)
                linearLoadWithFactor = linearLoad * 1.5;
            else if (buildingCode == BuildingCode.ASCE705)
                linearLoadWithFactor = linearLoad * 1.65;
            else if (buildingCode == BuildingCode.ASCE710 || buildingCode == BuildingCode.ASCE716)
                linearLoadWithFactor = 0.6 * linearLoad * 1.65;
            else
            {
                BH.Engine.Base.Compute.RecordError("Building code not supported in plastic modulus check calculations.");
                return double.NaN;
            }

            if (supportType == SupportType.PinPin || supportType == SupportType.PinSlide)
            {
                double mMax = linearLoadWithFactor * Math.Pow(length, 2) / 8;
                return mMax / yieldStress;
            }
            else if (supportType == SupportType.FixFix)
            {
                double mMax = linearLoadWithFactor * Math.Pow(length, 2) / 12;
                return mMax / yieldStress;
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Support type not supported in plastic modulus check calculations.");
                return double.NaN;
            }
        }

        /***************************************************/
    }
}


