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

        [Description("Provide the sequence of BHoM upgrader that need to be called for an object from a given version.")]
        [Input("version", "Version of the object that need to be upgraded.")]
        [Output("upgraders", "BHoM upgrader versions that need to be called.")]
        public static List<string> UpgradersToCall(string version)
        {
            if (m_UpgradersToCall.ContainsKey(version))
                return m_UpgradersToCall[version];

            List<string> upgraders = UpgraderVersions();
            int index = upgraders.IndexOf(version);
            return upgraders.Skip(Math.Min(index + 1, upgraders.Count - 1)).ToList();
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<string>> m_UpgradersToCall = new Dictionary<string, List<string>>();

        /***************************************************/
    }
}




