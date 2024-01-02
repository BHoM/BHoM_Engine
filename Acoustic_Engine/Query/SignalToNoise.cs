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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static SnRatio SignalToNoise(this Receiver receiver, List<Speaker> speakers, Room room, double sabineTime, double envNoise, Frequency f)
        {
            Dictionary<Frequency, double> gains = speakers.Select(x => x.Gains).First();
            Dictionary<Frequency, double> modFactors = new Dictionary<Frequency, double> { { Frequency.Hz500, 8.0 }, { Frequency.Hz2000, 1.4 } };
            double receiverDirectivity = 1.5;

            double revDistance = Query.ReverbDistance(room, sabineTime);
            double timeConstant = Query.TimeConstant(sabineTime);
            double closestDist = Engine.Geometry.Query.ClosestDistance(speakers.Select(x => x.Location), room.Samples.Select(x => x.Location));

            double soundLevel = receiver.DirectSound(speakers, room, sabineTime, f).Value;
            double FT = 2.0 * Math.PI * timeConstant * modFactors[f];
            double sqrdRevDist = revDistance * revDistance;
            double sqrdClosDist = closestDist * closestDist;
            double totalNoise = Math.Pow(10, (envNoise - soundLevel) / 10);

            double cap_a = ((gains[f] * receiverDirectivity) / sqrdClosDist) + ((1.0 / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0)));
            double cap_b = (FT / sqrdRevDist) / (1.0 + Math.Pow(FT, 2.0));
            double cap_c = ((gains[f] * receiverDirectivity) / sqrdClosDist) + (1.0 / sqrdRevDist) + (gains[f] * totalNoise);
            double modulationF = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;

            double appSoundNoise = 10.0 * Math.Log10(modulationF / (1.0 - modulationF));
            appSoundNoise = appSoundNoise > 15 ? 15 : (appSoundNoise < -15 ? -15 : appSoundNoise);
            return Create.SnRatio(appSoundNoise, receiver.ReceiverID, -1, f);
        }

        /***************************************************/
    }
}





