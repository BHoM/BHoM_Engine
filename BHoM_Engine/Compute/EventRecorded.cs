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

using BH.Engine.Base.Objects;
using BH.oM.Base.Debugging;
using System;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Events                             ****/
        /***************************************************/

        [Description("Gets raised every time an event gets recorded in the debug log (see BH.Engine.Compute.RecordEvent method).")]
        public static event EventHandler<EventRecordedEventArgs> EventRecorded;


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void OnEventRecorded(Event ev)
        { 
            if (ev != null)
            {
                EventRecorded?.Invoke(null, new EventRecordedEventArgs(ev));
            }
        }

        /***************************************************/
    }
}
