/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a path is valid to add to the library settings. WIll return false if the path is the default library folder or a sub or superfolder of it.")]
        [Input("userPath", "The userpath to check.")]
        [Output("isValid", "Returns true if the user path is valid to be added.")]
        public static bool IsValidUserPath(this string userPath)
        {
            if (string.IsNullOrWhiteSpace(userPath))
            {
                Reflection.Compute.RecordWarning("Null or empty userpaths are not valid.");
                return false;
            }

            string path = System.IO.Path.GetFullPath(userPath);

            if (m_sourceFolder.ToLower().Contains(path.ToLower()))
            {
                Reflection.Compute.RecordWarning($"Not allowed to add a parent folder of the default source folder {m_sourceFolder} to the user libraries.");
                return false;
            }

            if (path.ToLower().Contains(m_sourceFolder.ToLower()))
            {
                Reflection.Compute.RecordWarning($"Not allowed to add a sub folder of the default dataset folder {m_sourceFolder} to the user libraries.");
                return false;
            }

            return true;
        }

        /***************************************************/
    }
}
