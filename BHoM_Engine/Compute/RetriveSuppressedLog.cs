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

        [Description("Retrive the events recorded while the logging system was switched off and put them into the main log. When the logging system is switched off, recording of events is done in quiet mode which means UIs are not made aware of the events and the main event log does not have knowledge of them. We still record the events though because they may be useful. This method will move any events stored within the log when it was switched off up to the main log for visibiliy and inspection, and will reset the quiet log to a clean state.")]
        [Output("True if no error occurs in moving up events recorded during a quiet period.")]
        public static bool RetriveSuppressedLog()
        {
            lock(Global.DebugLogLock)
            {
                Log switchedOffLog = Query.SuppressedLog();
                Log debugLog = Query.DebugLog();

                debugLog.CurrentEvents.AddRange(switchedOffLog.CurrentEvents);
                debugLog.AllEvents.AddRange(switchedOffLog.AllEvents);

                Query.ResetSuppressedLog(); //Now we've moved the switched off log events into the main log, we don't need the switched off log to also keep a copy of them
            }

            return true;
        }
    }
}
