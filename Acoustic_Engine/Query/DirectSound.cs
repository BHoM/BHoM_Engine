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
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Acoustic;
using BH.Engine.Geometry;


namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static SoundLevel DirectSound(this Receiver receiver, List<Speaker> speakers, Room room, double revTime, Frequency frequency)
        {
            SoundLevel directSound = new SoundLevel();
            foreach (Speaker speaker in speakers)
            {
                Vector deltaPos = receiver.Location - speaker.Location;
                double distance = deltaPos.Length();
                double roomConstant = room.RoomConstant(revTime);

                double recieverAngle = deltaPos.Angle(speaker.Direction) * (180 / Math.PI);
                double orientationFactor = speaker.GainFactor(recieverAngle, frequency);
                double gain = speaker.Gains[frequency] * Math.Pow(10, orientationFactor / 10);
                directSound += Create.SoundLevel((speaker.EmissiveLevel / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant),
                                               receiver.ReceiverID, -1, frequency);
            }
            return Create.SoundLevel(directSound.Value + speakers.First().EmissiveLevel, receiver.ReceiverID, -1, frequency);
        }

        /***************************************************/
    }
}





