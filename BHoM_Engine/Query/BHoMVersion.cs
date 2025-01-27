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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string BHoMVersion()
        {
            if (m_BHoMVersion.Length > 0)
                return m_BHoMVersion;

            // First try to get the assembly file version
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
            if (attributes.Length > 0)
            {
                AssemblyFileVersionAttribute attribute = attributes.First() as AssemblyFileVersionAttribute;
                if (attribute != null && attribute.Version != null)
                {
                    string[] split = attribute.Version.Split('.');
                    if (split.Length >= 2)
                        m_BHoMVersion = split[0] + "." + split[1];
                }
            }

            // Get the assembly version as a fallback
            if (m_BHoMVersion.Length == 0)
            {
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                m_BHoMVersion = currentVersion.Major + "." + currentVersion.Minor;
            }

            return m_BHoMVersion;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static string m_BHoMVersion = "";

        /***************************************************/
    }
}






