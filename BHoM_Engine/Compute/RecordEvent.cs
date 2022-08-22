/*
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
using System.ComponentModel;
using System;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Records an event in the BHoM event log.")]
        [Input("message", "Message related to the event to be logged.")]
        [Input("type", "Type of the event to be logged.")]
        [Output("success", "True if the event is logged successfully.")]
        public static bool RecordEvent(string message, EventType type = EventType.Unknown)
        {
            return RecordEvent(new Event { Message = message, Type = type });
        }

        /***************************************************/

        [Description("Records an event in the BHoM event log.")]
        [Input("newEvent", "Event to be logged.")]
        [Output("success", "True if the event is logged successfully.")]
        public static bool RecordEvent(Event newEvent)
        {
            if (newEvent == null)
            {
                Compute.RecordError("Cannot record a null event.");
                return false;
            }

            string trace = System.Environment.StackTrace;
            newEvent.StackTrace = string.Join("\n", trace.Split('\n').Skip(4).ToArray());

            lock (Global.DebugLogLock)
            {
                Log log = Query.DebugLog();
                log.AllEvents.Add(newEvent);
                log.CurrentEvents.Add(newEvent);
                OnEventRecorded(newEvent);
            }

            return true;
        }

        /***************************************************/
    }
}
