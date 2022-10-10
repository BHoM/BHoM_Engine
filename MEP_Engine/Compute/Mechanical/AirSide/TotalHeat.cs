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

namespace BH.Engine.MEP.Mechanical.RulesOfThumb.AirSide
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the total heat contained within air given CFM and two enthalpy points. Rule of Thumb calc uses coefficient at STP of air.")]
        [Input("airflow", "Airflow [CFM]")]
        [Input("enthalpyIn", "in enthalpy value [Btu/Lb dry air]")]
        [Input("enthalpyOut", "out enthalpy value [Btu/Lb dry air]")]
        [Output("totalHeat", "total heat value [Btu/h]")]
        public static double TotalHeat(double airflow, double enthalpyIn, double enthalpyOut)
        {
            if(airflow == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the total heat from a null airflow value");
                return -1;
            }

            if(enthalpyIn == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the total heat from a null enthalpyIn value");
                return -1;
            }

            if (enthalpyOut == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the total heat from a null enthalpyOut value");
                return -1;
            }

            double totalHeat = 4.5 * airflow * (enthalpyIn-enthalpyOut);


            return totalHeat;
        }

        /***************************************************/

    }
}
