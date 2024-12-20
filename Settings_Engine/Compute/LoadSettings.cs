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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Linq;

namespace BH.Engine.Settings
{
    public static partial class Compute
    {
        [Description("Load all the JSON settings stored within the provided folder into memory. If no folder is provided, the default folder of %ProgramData%/BHoM/Settings is used instead. All JSON files are scraped within the directory (including subdirectories) and deserialised to ISettings objects.")]
        [Input("settingsFolder", "Optional input to determine where to load settings from. Defaults to %ProgramData%/BHoM/Settings if no folder is provided.")]
        [Input("fileFilter", "Optional input to filter which types of files to load settings from. Defaults to all JSON (.json) files within the provided folder.")]
        [Input("forceLoad", "Optional input to determine whether settings should be loaded even if they have already been loaded. If true, and settings have previously been loaded, then settings will be reloaded from the provided folder path.")]
        [Input("searchSubDirectories", "Optional input to determine whether settings should be loaded from the given directory AND all sub-directories (if they exist). Defaults to false, meaning only settings file in the given directory will be loaded.")]
        public static void LoadSettings(string settingsFolder = null, string fileFilter = "*.json", bool forceLoad = false, bool searchSubDirectories = false)
        {
            if (string.IsNullOrEmpty(settingsFolder))
                settingsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Settings"); //Defaults to C:/ProgramData/BHoM/Settings if no folder is provided

            string fullSettingsLoadedKey = $"{settingsFolder}/{fileFilter}"; //Construct a unique key for this folder + filter to determine whether settings have already been loaded from this folder for this file type.

            if (Global.BHoMSettingsLoaded.Contains(fullSettingsLoadedKey) && !forceLoad)
                return; //Settings from this folder have already been loaded, and we're not force loading them, so don't waste time reloading them

            var searchOption = (searchSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var settingsFiles = Directory.EnumerateFiles(settingsFolder, fileFilter, searchOption);

            foreach (var file in settingsFiles)
            {
                string contents = "";
                try
                {
                    contents = File.ReadAllText(file);
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordError(ex, $"Error when trying to read settings file: {file}.");
                }

                if (string.IsNullOrEmpty(contents))
                    continue;

                try
                {
                    ISettings settings = BH.Engine.Serialiser.Convert.FromJson(contents) as ISettings;
                    Type type = settings.GetType();
                    Global.BHoMSettings[type] = settings;
                    Global.BHoMSettingsFilePaths[type] = file;
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordWarning(ex, $"Cannot deserialise the contents of {file} to an ISettings object.");
                }
            }

            Global.BHoMSettingsLoaded.Add(fullSettingsLoadedKey);
        }

        /***************************************************/

        [Description("Load a single settings file into memory for use. Settings file must contain one object in BHoM serialised JSON which is an object implementing the ISettings interface. Any settings object within the file will overwrite any previously loaded settings in memory.")]
        [Input("filePath", "The full file path of the settings JSON file to be loaded.")]
        public static void LoadSettingsFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                BH.Engine.Base.Compute.RecordError("File Path is null or empty.");
                return;
            }

            if(!File.Exists(filePath))
            {
                BH.Engine.Base.Compute.RecordError($"File at {filePath} does not exist to read from.");
                return;
            }

            string contents = "";

            try
            {
                contents = File.ReadAllText(filePath);
            }
            catch(Exception ex)
            {
                BH.Engine.Base.Compute.RecordError(ex, $"Error occurred in reading in file from {filePath}.");
            }

            if(string.IsNullOrEmpty(contents))
            {
                BH.Engine.Base.Compute.RecordError($"Failed to read data from {filePath}.");
                return;
            }

            try
            {
                ISettings settings = BH.Engine.Serialiser.Convert.FromJson(contents) as ISettings;
                Type type = settings.GetType();
                Global.BHoMSettings[type] = settings;
                Global.BHoMSettingsFilePaths[type] = filePath;
            }
            catch (Exception ex)
            {
                BH.Engine.Base.Compute.RecordError(ex, $"Cannot deserialise the contents of {filePath} to an ISettings object.");
            }
        }
    }
}

