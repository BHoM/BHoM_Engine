/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.MEP.Equipment;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the height and width of the equipment based on the inputs of AirVelocityAcrossCoil and TotalAirFlow.")]
        [Input("mepEquipmentObject", "MEP object that contains properties for AirVelocityAcrossCoil and TotalAirFlow.")]
        [Output("widthlength", "This is the width OR the length (they are the same value), since the method is taking the square root of the airflow divided by the velocity.")]
        public static double FaceAreaByVelocity(this AirHandlingUnit mepEquipmentObject)
        {
            if(mepEquipmentObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the face area by velocity of a null air handling unit object.");
                return -1;
            }

            return Math.Sqrt(mepEquipmentObject.TotalAirFlow / mepEquipmentObject.AirVelocityAcrossCoil);
        }
    }
}




