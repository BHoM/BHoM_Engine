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

namespace BH.Engine.MEP.Mechanical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the air change rate (ACH) given airflow and volume of space")]
        [Input("volumetricFlowRate", "Volumetric flow rate of air.", typeof(VolumetricFlowRate))]
        [Input("volume", "Volume of the space.", typeof(Volume))]
        [Output("ACH", "Number of full changes of air per hours [Air changes/hour].")]
        public static double AirChangeRate(double airflow, double volume)
        {
            if(airflow == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null airflow value");
                return -1;
            }

            if(volume == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ACH from a null volume value");
                return -1;
            }


            return (airflow * 60 * 60)/volume;
        }

        /***************************************************/

    }
}