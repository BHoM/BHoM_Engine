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

using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double GainFactor(this Speaker speaker, double angle, Frequency frequency)
        {
            if(speaker == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the gain factor from a null speaker.");
                return 0;
            }

            double gains = speaker.Gains[frequency];
            switch (frequency)
            {
                case Frequency.Hz63:
                case Frequency.Hz125:
                case Frequency.Hz250:
                case Frequency.Hz500:
                case Frequency.Hz1000:
                    return (-2 * angle / 90 - 8);
                case Frequency.Hz2000:
                case Frequency.Hz4000:
                case Frequency.Hz8000:
                case Frequency.Hz16000:
                    return (-18 * angle / 150 - 2);
                default:
                    return 0;
            }
        }

        /***************************************************/
    }
}





