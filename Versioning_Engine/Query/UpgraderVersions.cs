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

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Versioning
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Provide the list of BHoM versions covered by an upgrader")]
        [Output("versions", "BHoM versions covered by an upgrader.")]
        public static List<string> UpgraderVersions()
        {
            if (m_UpgraderVersions != null)
                return m_UpgraderVersions;

            string upgraderFolder = Path.Combine(Base.Query.BHoMFolder(), "..", "Upgrades");
            if (!Directory.Exists(upgraderFolder))
                return new List<string>();

            m_UpgraderVersions = Directory.GetDirectories(upgraderFolder, "BHoMUpgrader*", SearchOption.TopDirectoryOnly).Select(folder =>
            {
                string number = Path.GetFileName(folder).Replace("BHoMUpgrader", "");
                return number.Insert(number.Length - 1, ".");
            })
            .OrderBy(x =>
            {
                double n = 0;
                double.TryParse(x, out n);
                return n;
            })
            .ToList();

            return m_UpgraderVersions;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<string> m_UpgraderVersions = null;

        /***************************************************/
    }
}





