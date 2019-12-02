/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Debugging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool RecordEvent(string message, EventType type = EventType.Unknown)
        {
            return RecordEvent(new Event { Message = message, Type = type });
        }

        /***************************************************/

        public static bool RecordError(string message)
        {
            string additionalInfo = "";
            var frame1 = new StackTrace().GetFrame(1).GetMethod();
            var frame2 = new StackTrace().GetFrame(2).GetMethod();

            if (frame1.Name != "RecordError" && frame1.Name != frame2.Name)
            {
                additionalInfo = $"\nHappened in: {frame1.Name}";
                if (frame2.DeclaringType.FullName.Contains("BH."))
                    additionalInfo += $" while executing {frame2.Name}.";
            }

            return RecordEvent(new Event { Message = message + additionalInfo, Type = EventType.Error });
        }

        /***************************************************/

        public static bool RecordWarning(string message)
        {
            return RecordEvent(new Event { Message = message, Type = EventType.Warning });
        }

        /***************************************************/

        public static bool RecordNote(string message)
        {
            return RecordEvent(new Event { Message = message, Type = EventType.Note });
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool RecordEvent(Event newEvent)
        {
            string trace = System.Environment.StackTrace;
            newEvent.StackTrace = string.Join("\n", trace.Split('\n').Skip(4).ToArray());

            Log log = Query.DebugLog();
            log.AllEvents.Add(newEvent);
            log.CurrentEvents.Add(newEvent);

            return true;
        }

        /***************************************************/
    }
}
