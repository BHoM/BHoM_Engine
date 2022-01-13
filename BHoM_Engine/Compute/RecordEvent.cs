﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Base.Debugging;
using BH.oM.Base.Attributes;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("5.1", "BH.Engine.Reflection.Compute.RecordEvent(System.String, BH.oM.Base.Debugging.EventType)")]
        public static bool RecordEvent(string message, EventType type = EventType.Unknown)
        {
            return RecordEvent(new Event { Message = message, Type = type });
        }

        /***************************************************/

        [PreviousVersion("5.1", "BH.Engine.Reflection.Compute.RecordEvent(BH.oM.Base.Debugging.Event)")]
        public static bool RecordEvent(Event newEvent)
        {
            if (newEvent == null)
            {
                Compute.RecordError("Cannot record a null event.");
                return false;
            }

            string trace = System.Environment.StackTrace;
            newEvent.StackTrace = string.Join("\n", trace.Split('\n').Skip(4).ToArray());

            Log log = Query.DebugLog();
            log.AllEvents.Add(newEvent);
            log.CurrentEvents.Add(newEvent);

            return true;
        }
    }
}