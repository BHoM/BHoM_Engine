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

        [Description("Provide the sequence of BHoM upgrader that need to be called for an object from a given version.")]
        [Input("version", "Version of the object that need to be upgraded.")]
        [Output("upgraders", "BHoM upgrader versions that need to be called.")]
        public static List<string> UpgradersToCall(string version)
        {
            if (m_UpgradersToCall.ContainsKey(version))
                return m_UpgradersToCall[version];

            GetVersionNumbers(version, out int sourceMajor, out int sourceMinor);
            GetVersionNumbers(Base.Query.BHoMVersion(), out int targetMajor, out int targetMinor);

            List<string> result = new List<string>();
            for (int major = sourceMajor; major <= targetMajor; major++)
            {
                int startMinor = (major == sourceMajor) ? sourceMinor : 0;
                int endMinor = (major == targetMajor) ? targetMinor : 3;

                for (int minor = startMinor; minor <= endMinor; minor++)
                    result.Add(major + "." + minor);
            }

            m_UpgradersToCall[version] = result;

            return result;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool GetVersionNumbers(string version, out int major, out int minor)
        {
            string[] versionSplit = version.Split(new char[] { '.' });
            if (versionSplit.Length != 2)
            {
                BH.Engine.Base.Compute.RecordError($"Version provided doesn't fit the format <Major>.<Minor>");
                major = 0;
                minor = 0;
                return false;
            }

            bool success = int.TryParse(versionSplit[0], out major);
            success &= int.TryParse(versionSplit[1], out minor);

            return success;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<string>> m_UpgradersToCall = new Dictionary<string, List<string>>();

        /***************************************************/
    }
}





