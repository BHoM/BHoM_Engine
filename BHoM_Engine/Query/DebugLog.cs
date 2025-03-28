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
using System.Reflection;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        internal static Log DebugLog()
        {
            lock (Global.DebugLogLock)
            {
                if (m_DebugLog == null)
                    m_DebugLog = new Log();

                return m_DebugLog;
            }
        }

        /***************************************************/

        internal static Log SuppressedLog()
        {
            lock(Global.DebugLogLock)
            {
                if (m_SuppressedLog == null)
                    m_SuppressedLog = new Log();

                return m_SuppressedLog;
            }
        }

        /***************************************************/

        internal static void ResetSuppressedLog()
        {
            lock(Global.DebugLogLock)
            {
                m_SuppressedLog = new Log();
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Log m_DebugLog = new Log();
        private static Log m_SuppressedLog = new Log(); //If someone has switched off the log for any reason, keep a record of their events in this log instead - this way they're not completely removed and could be accessed if they switched it off by accident

        /***************************************************/
    }
}






