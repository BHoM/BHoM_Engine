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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Data.Library;

namespace BH.Engine.Library
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Adds a custom folderpath to the User libraries accessed by the Library_Engine.")]
        [Input("customPath", "Path to folder containing user libraries to be added.", typeof(FolderPathAttribute))]
        [Input("refreshLibraries", "If true, all loaded libraries will be refreshed and reloaded, making use of the provided LibrarySettings object.")]
        [Output("sucess", "Returns true if the settings was successfully updated.")]
        public static bool AddUserPath(string customPath, bool refreshLibraries = true)
        {
            if (!customPath.IsValidUserPath())
                return false;

            LibrarySettings settings = Query.LibrarySettings() ?? new LibrarySettings();
            settings.UserLibraryPaths.Add(customPath);
            return SaveLibrarySettings(settings, true, refreshLibraries);
        }

        /***************************************************/
    }
}





