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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Find out if the system is currently suppressing error recording in the main log or not.")]
        [Output("errorSuppression", "True if errors are currently being suppressed, false if they are being recorded normally.")]
        public static bool IsSuppressingErrors()
        {
            return Compute.m_SuppressError;
        }

        /***************************************************/

        [Description("Find out if the system is currently suppressing warning recording in the main log or not.")]
        [Output("warningSuppression", "True if warnings are currently being suppressed, false if they are being recorded normally.")]
        public static bool IsSuppressingWarnings()
        {
            return Compute.m_SuppressWarning;
        }

        /***************************************************/

        [Description("Find out if the system is currently suppressing note recording in the main log or not.")]
        [Output("noteSuppression", "True if notes are currently being suppressed, false if they are being recorded normally.")]
        public static bool IsSuppressingNotes()
        {
            return Compute.m_SuppressNote;
        }

        /***************************************************/
    }
}