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

using System.Collections.Generic;
using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static Rasti Rasti(this Receiver receiver, List<Speaker> speakers, Room room, double sabineTime, double envNoise)
        {
            SnRatio apparentSn500 = receiver.SignalToNoise(speakers, room, sabineTime, envNoise, Frequency.Hz500);
            SnRatio apparentSn2000 = receiver.SignalToNoise(speakers, room, sabineTime, envNoise, Frequency.Hz2000);
            double rasti500 = (apparentSn500.Value + 15) / 30;
            double rasti2000 = (apparentSn2000.Value + 15) / 30;
            return Create.Rasti((4d / 9d) * rasti500 + (5d / 9d) * rasti2000, receiver.ReceiverID);
        }

        /***************************************************/
    }
}




