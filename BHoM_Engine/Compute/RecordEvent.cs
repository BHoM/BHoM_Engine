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

        [Description("Record an event with details of a C# exception within the BHoM logging system.")]
        [Input("exception", "The C# exception being caught to provide the event and stack information for.")]
        [Input("message", "An optional additional message which will be displayed first in the event log.")]
        [Input("type", "Type of the event to be logged.")]
        [Output("success", "True if the event has been successfully recorded as a BHoM Event.")]
        public static bool RecordEvent(Exception exception, string message = "", EventType type = EventType.Unknown)
        {
            if (exception == null)
                return false;

            string exceptionMessage = "";

            Exception e = exception;
            while(e != null)
            {
                if (!string.IsNullOrEmpty(exceptionMessage))
                    exceptionMessage += $"{Environment.NewLine}{Environment.NewLine}";

                exceptionMessage += $"{e.Message}";

                e = e.InnerException;
            }

            if (!string.IsNullOrEmpty(message))
                message = $"{message}\n\n{exceptionMessage}";
            else
                message = exceptionMessage;

            return RecordEvent(new Event { Message = message, StackTrace = exception.StackTrace, Type = type });
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

            if (string.IsNullOrEmpty(newEvent.StackTrace))
            {
                string trace = System.Environment.StackTrace;
                newEvent.StackTrace = string.Join("\n", trace.Split('\n').Skip(4).ToArray());
            }

            bool suppressEvents = (newEvent.Type == EventType.Error && m_SuppressError) 
                                || (newEvent.Type == EventType.Warning && m_SuppressWarning)
                                || (newEvent.Type == EventType.Note && m_SuppressNote);
            
            Log log = null;
            if (!suppressEvents)
                log = Query.DebugLog();
            else
                log = Query.SuppressedLog();

            lock (Global.DebugLogLock)
            {
                log.AllEvents.Add(newEvent);
                log.CurrentEvents.Add(newEvent);

                if(!suppressEvents)
                    OnEventRecorded(newEvent); //Only raise an event if we're not in switched off mode
            }

            if (newEvent.Type == EventType.Error && m_ThrowError && !m_SuppressError) //Only throw the event as an exception if someone has asked us to throw it, AND we aren't suppressing them
                throw new Exception(newEvent.ToText());

            return true;
        }


        /***************************************************/
        /**** Public Events                             ****/
        /***************************************************/

        [Description("Gets raised every time an event gets recorded in the debug log (see BH.Engine.Compute.RecordEvent method).")]
        public static event EventHandler<Event> EventRecorded;


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void OnEventRecorded(Event ev)
        {
            if (ev != null)
            {
                EventRecorded?.Invoke(null, ev);
            }
        }

        /***************************************************/
        /**** Private Variables                         ****/
        /***************************************************/

        private static bool m_SuppressError = false; //Default to false, do not suppress any events which come through the system
        private static bool m_SuppressWarning = false;
        private static bool m_SuppressNote = false;

        private static bool m_ThrowError = false; //Default to false - do not throw errors. However, if a user (developer user or UI user) has switched this on, then errors will be thrown for try/catch statements to handle.
        //ToDo: Discuss whether we want this to be true by default and have BHoM_UI switch it off on load, or keep as is. FYI @alelom
    }
}


