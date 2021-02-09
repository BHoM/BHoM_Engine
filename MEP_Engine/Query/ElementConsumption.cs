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

using BH.oM.MEP.Enums;
using BH.oM.MEP.System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the total consuption value from the object.")]
        [Input("obj", "The object to get the total consumption from.")]
        [Input("type", "The ConsumptionType to query.")]
        [Output("consumption", "The consumption rate of the object based on a prescribed ConsumptionType.")]
        public static double ElementConsumption(this IElementC obj, ConsumptionType type)
        {
            double consumption = 0;

            switch (type)
            {          
                case ConsumptionType.Undefined:
                    BH.Engine.Reflection.Compute.RecordError("No ConsumptionType could be determined. Returning Double.NaN.");
                    return double.NaN;
                case ConsumptionType.Air:
                    consumption = obj.Consumption.Where(x => x.Type == ConsumptionType.Air).Select(y => y.ConsumptionRate).Sum();
                    BH.Engine.Reflection.Compute.RecordNote("Consumption of Air is returned as VolumetricAirFlow (m3/s).");
                    return consumption;
                case ConsumptionType.Fuel:
                    consumption = obj.Consumption.Where(x => x.Type == ConsumptionType.Fuel).Select(y => y.ConsumptionRate).Sum();
                    BH.Engine.Reflection.Compute.RecordNote("Consumption of Fuel is returned as VolumetricAirFlow (m3/s).");
                    return consumption;
                case ConsumptionType.Power:
                    consumption = obj.Consumption.Where(x => x.Type == ConsumptionType.Power).Select(y => y.ConsumptionRate).Sum();
                    BH.Engine.Reflection.Compute.RecordNote("Consumption of Power is returned as Watts.");
                    return consumption;
                case ConsumptionType.Refrigerant:
                    consumption = obj.Consumption.Where(x => x.Type == ConsumptionType.Refrigerant).Select(y => y.ConsumptionRate).Sum();
                    BH.Engine.Reflection.Compute.RecordNote("Consumption of Refrigerant is returned as VolumetricAirFlow (m3/s).");
                    return consumption;
                case ConsumptionType.Water:
                    consumption = obj.Consumption.Where(x => x.Type == ConsumptionType.Water).Select(y => y.ConsumptionRate).Sum();
                    BH.Engine.Reflection.Compute.RecordNote("Consumption of Water is returned as VolumetricAirFlow (m3/s).");
                    return consumption;
                default:
                    return double.NaN;
            }
        }
        /***************************************************/
    }
}