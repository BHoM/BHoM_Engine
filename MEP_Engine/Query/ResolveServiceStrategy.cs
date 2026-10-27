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
using BH.oM.MEP.Criteria;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SpaceServiceStrategy ResolvedServiceStrategy(
            SpaceServiceStrategy defaultStrategy,
            SpaceServiceStrategy spaceOverride = null)
        {
            OutdoorAirStrategy outdoorAir =
                defaultStrategy?.OutdoorAirStrategy
                ?? OutdoorAirStrategy.Unspecified;

            CoolingStrategy cooling =
                defaultStrategy?.CoolingStrategy
                ?? CoolingStrategy.Unspecified;

            HeatingStrategy heating =
                defaultStrategy?.HeatingStrategy
                ?? HeatingStrategy.Unspecified;

            ExtractStrategy extract =
                defaultStrategy?.ExtractStrategy
                ?? ExtractStrategy.Unspecified;

            if (spaceOverride != null)
            {
                if (spaceOverride.OutdoorAirStrategy
                    != OutdoorAirStrategy.Unspecified)
                {
                    outdoorAir = spaceOverride.OutdoorAirStrategy;
                }

                if (spaceOverride.CoolingStrategy
                    != CoolingStrategy.Unspecified)
                {
                    cooling = spaceOverride.CoolingStrategy;
                }

                if (spaceOverride.HeatingStrategy
                    != HeatingStrategy.Unspecified)
                {
                    heating = spaceOverride.HeatingStrategy;
                }

                if (spaceOverride.ExtractStrategy
                    != ExtractStrategy.Unspecified)
                {
                    extract = spaceOverride.ExtractStrategy;
                }
            }

            return new SpaceServiceStrategy
            {
                OutdoorAirStrategy = outdoorAir,
                CoolingStrategy = cooling,
                HeatingStrategy = heating,
                ExtractStrategy = extract
            };
        }

        /***************************************************/
    }
}
