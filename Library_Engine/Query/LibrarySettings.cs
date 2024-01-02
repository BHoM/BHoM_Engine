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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data.Library;
using System.IO;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the Library settings object currently used by the Library_Engine.")]
        [Output("settings", "The settings used by the Library_Engine. If settings don't exist, null will be returned.")]
        public static LibrarySettings LibrarySettings()
        {
            if (File.Exists(m_settingsPath))
            {
                string librarySettingsText = File.ReadAllText(m_settingsPath);

                if (!string.IsNullOrWhiteSpace(librarySettingsText))
                    return Engine.Serialiser.Convert.FromJson(librarySettingsText) as LibrarySettings;
            }

            return null;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        public static readonly string m_settingsPath = Path.Combine(BH.Engine.Base.Query.BHoMFolderSettings(), "LibrarySettings.cfg");

        /***************************************************/
    }
}





