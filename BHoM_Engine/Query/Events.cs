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
    }
}





