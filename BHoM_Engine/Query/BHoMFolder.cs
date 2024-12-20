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
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the folder path for the top level assemblies directory, for example for the C: drive it will return C:/ProgramData/BHoM/Assemblies")]
        public static string BHoMFolder()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Assemblies");
        }

        /***************************************************/

        [Description("Returns the folder path for the top level datasets directory, for example for the C: drive it will return C:/ProgramData/BHoM/Datasets")]
        public static string BHoMFolderDatasets()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Datasets");
        }

        /***************************************************/

        [Description("Returns the folder path for the top level extensions directory, for example for the C: drive it will return C:/ProgramData/BHoM/Extensions")]
        public static string BHoMFolderExtensions()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Extensions");
        }

        /***************************************************/

        [Description("Returns the folder path for the top level logs directory, for example for the C: drive it will return C:/ProgramData/BHoM/Logs")]
        public static string BHoMFolderLogs()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Logs");
        }

        /***************************************************/

        [Description("Returns the folder path for the top level resources directory, for example for the C: drive it will return C:/ProgramData/BHoM/Resources")]
        public static string BHoMFolderResources()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Resources");
        }

        /***************************************************/

        [Description("Returns the folder path for the top level settings directory, for example for the C: drive it will return C:/ProgramData/BHoM/Settings")]
        public static string BHoMFolderSettings()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Settings");
        }

        /***************************************************/

        [Description("Returns the folder path for the top level upgrades directory, which houses sub-directories for upgraders for different BHoM versions. For example for the C: drive it will return C:/ProgramData/BHoM/Upgrades")]
        public static string BHoMFolderUpgrades()
        {
            return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Upgrades");
        }

        /***************************************************/
    }
}






