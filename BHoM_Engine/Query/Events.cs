/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all events stored in the lifetime of the BHoM event log (equal to lifetime of the process).")]
        [Output("events", "All events stored in the lifetime of the BHoM event log.")]
        public static List<Event> AllEvents()
        {
            lock (Global.DebugLogLock)
            {
                Log log = DebugLog();
                return log.AllEvents.ToList();
            }
        }


        /***************************************************/

        [Description("Gets the events from the BHoM event log that are related to the current action.")]
        [Output("events", "Events from the BHoM event log that are related to the current action.")]
        public static List<Event> CurrentEvents()
        {
            lock (Global.DebugLogLock)
            {
                Log log = DebugLog();
                return log.CurrentEvents.ToList();
            }
        }

        /***************************************************/

        [Description("Gets the events from the BHoM event log that have occurred on or since the given date/time.")]
        [Input("utcDateTime", "A date/time in the UTC timezone. Only events recorded ON (to the second) or AFTER this date/time will be returned.")]
        [Output("events", "Events from the BHoM event log raised since the given date/time.")]
        public static List<Event> EventsSince(DateTime utcDateTime)
        {
            lock (Global.DebugLogLock)
            {
                Log log = Query.DebugLog();
                return log.AllEvents.Where(x => x.UtcTime >= utcDateTime).ToList();
            }
        }

        /***************************************************/

        [Description("Gets the events from the BHoM event log that have occurred since the last time you asked to view the event logs. On the first time of asking, this will be all events which have occurred. Each time you run this component, the bookmark will be updated to reflect the current UTC Date/Time.")]
        [Output("events", "Events from the BHoM event log raised since the last time you queried the event log via the bookmark method.")]
        public static List<Event> EventsSinceBookmark()
        {
            List<Event> events = EventsSince(m_LastBookmark);

            m_LastBookmark = DateTime.UtcNow; //Update the bookmark to now

            return events;
        }

        /***************************************************/
        /*** Private variables                         *****/
        /***************************************************/

        private static DateTime m_LastBookmark = DateTime.UtcNow; //Default to the current UTC time on load
    }
}
