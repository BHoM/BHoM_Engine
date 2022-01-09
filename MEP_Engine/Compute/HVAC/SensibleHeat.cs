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

namespace BH.Engine.MEP.HVAC.RulesOfThumb.AirSide
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the sensible heat contained within air given CFM and two temperature points. Rule of Thumb calc uses coefficient at STP of air.")]
        [Input("airflow", "Airflow [CFM]")]
        [Input("tempIn", "in temperature value [F]")]
        [Input("tempOut", "out temperature value [F]")]
        [Output("sensibleHeat", "sensible heat value [Btu/h]")]
        public static double SensibleHeat(double airflow, double tempIn, double tempOut)
        {
            if(airflow == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the sensible heat from a null airflow value");
                return -1;
            }

            if(tempIn == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the sensible heat from a null tempIn value");
                return -1;
            }

            if (tempOut == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the sensible heat from a null tempOut value");
                return -1;
            }

            double sensibleHeat = 1.08 * airflow * (tempIn-tempOut);


            return sensibleHeat;
        }

        /***************************************************/

    }
}
