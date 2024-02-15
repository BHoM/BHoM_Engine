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

        [Description("Switch off the entire logging system used within BHoM. Any part of the code base which tries to log any note, warning, error, or other event into the log system will be housed in the silent recorder and not displayed to users. By default, all recording systems are turned on when BHoM is initialised.")]
        [Input("areYouSure", "Set this to true if you are sure you want to turn off all log recording. This boolean exists so that if this component is placed on a BHoM UI accidently, it does not turn off the system unless then expressly requested.")]
        public static void SwitchOffRecording(bool areYouSure)
        {
            if (!areYouSure)
                return;

            m_RecordError = false;
            m_RecordWarning = false;
            m_RecordNote = false;
        }

        /***************************************************/

        [Description("Switch off the entire logging system for ERRORS only. All other types of events will be recorded.")]
        [Input("areYouSure", "Set this to true if you are sure you want to turn off all ERROR log recording. This boolean exists so that if this component is placed on a BHoM UI accidently, it does not turn off the system unless then expressly requested.")]
        public static void SwitchOffRecordingErrors(bool areYouSure)
        {
            if (!areYouSure)
                return;

            m_RecordError = false;
        }

        /***************************************************/

        [Description("Switch off the entire logging system for WARNINGS only. All other types of events will be recorded.")]
        [Input("areYouSure", "Set this to true if you are sure you want to turn off all WARNING log recording. This boolean exists so that if this component is placed on a BHoM UI accidently, it does not turn off the system unless then expressly requested.")]
        public static void SwitchOffRecordingWarnings(bool areYouSure)
        {
            if (!areYouSure)
                return;

            m_RecordWarning = false;
        }

        /***************************************************/

        [Description("Switch off the entire logging system for NOTES only. All other types of events will be recorded.")]
        [Input("areYouSure", "Set this to true if you are sure you want to turn off all NOTE log recording. This boolean exists so that if this component is placed on a BHoM UI accidently, it does not turn off the system unless then expressly requested.")]
        public static void SwitchOffRecordingNotes(bool areYouSure)
        {
            if (!areYouSure)
                return;

            m_RecordNote = false;
        }
    }
}
