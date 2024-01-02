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

        [Description("Removes a custom folder path from the User libraries accessed by the Library_Engine.")]
        [Input("customPath", "Path to folder to be excluded from the custom libraries.", typeof(FolderPathAttribute))]
        [Input("refreshLibraries", "If true, all loaded libraries will be refreshed and reloaded, making use of the provided LibrarySettings object.")]
        [Output("success", "Returns true of the settings was successfully updated.")]
        public static bool RemoveUserPath(string customPath, bool refreshLibraries = true)
        {
            if (string.IsNullOrEmpty(customPath))
            {
                Engine.Base.Compute.RecordError($"Provided {nameof(customPath)} is null. Can not remove from library settings.");
                return false;
            }

            LibrarySettings settings = Query.LibrarySettings() ?? new LibrarySettings();

            if (settings == null)
            {
                Engine.Base.Compute.RecordError("No library settings available. Unable to remove path.");
                return false;
            }


            if (settings.UserLibraryPaths.Contains(customPath))
                settings.UserLibraryPaths.Remove(customPath);
            else
            {
                Engine.Base.Compute.RecordWarning($"Library settings does not contain provided {nameof(customPath)}.");
                return false;
            }

            return SaveLibrarySettings(settings, true, refreshLibraries);
        }

        /***************************************************/
    }
}




