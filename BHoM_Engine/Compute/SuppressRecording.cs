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

        [Description("Suppress the logging system used within BHoM. Any part of the code base which tries to log notes, warnings, or errors into the log system will be housed in the suppressed log and not displayed to users depending on which systems you've chosen to suppress. By default, all recording systems are turned on when BHoM is initialised.")]
        [Input("suppressErrors", "Determine whether to suppress BHoM Events of type ERROR from the log. Set to true to suppress these events.")]
        [Input("suppressWarnings", "Determine whether to suppress BHoM Events of type WARNING from the log. Set to true to suppress these events.")]
        [Input("suppressNotes", "Determine whether to suppress BHoM Events of type NOTE from the log. Set to true to suppress these events.")]
        public static void SuppressRecordingEvents(bool suppressErrors = false, bool suppressWarnings = false, bool suppressNotes = false)
        {
            m_RecordError = suppressErrors;
            m_RecordWarning = suppressWarnings;
            m_RecordNote = suppressNotes;
        }
    }
}
