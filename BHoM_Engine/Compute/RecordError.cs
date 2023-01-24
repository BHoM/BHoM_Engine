/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

        [Description("Record an error within the BHoM logging system.")]
        [Input("message", "The message to be recorded for this error.")]
        [Output("success", "True if the error has been successfully recorded as a BHoM Event.")]
        public static bool RecordError(string message)
        {
            return RecordEvent(new Event { Message = message, Type = EventType.Error });
        }

        [Description("Record an error with details of a C# exception within the BHoM logging system.")]
        [Input("exception", "The C# exception being caught to provide the error and stack information for.")]
        [Input("message", "An optional additional message which will be displayed first in the event log.")]
        [Output("success", "True if the error has been successfully recorded as a BHoM Event.")]
        public static bool RecordError(Exception exception, string message = "")
        {
            if (!string.IsNullOrEmpty(message))
                message = $"{message}\n\n{exception.ToString()}";
            else
                message = exception.ToString();

            return RecordEvent(new Event { Message = message, StackTrace = exception.StackTrace, Type = EventType.Error });
        }
    }
}