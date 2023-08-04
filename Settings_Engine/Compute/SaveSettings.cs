using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BH.Engine.Settings
{
    public static partial class Compute
    {
        [Description("Save a single instance of settings to the specified file path. If no file path is provided, it will default to %ProgramData%/BHoM/Settings/{settingsType}.json - if a file with that name already exists, it will be overwritten.")]
        [Input("settings", "The settings object to save as JSON in the specified file path.")]
        [Input("run", "Boolean toggle to determine whether to run the method. Useful for Visual Programming to prevent settings being saved/overwritten before providing a file path if desired.")]
        [Input("filePath", "Optional file path of where to save the file. If the file already exists, it will be overwritten.")]
        [Output("success", "True if the settings have been successfully saved, false otherwise.")]
        public static bool SaveSettings(ISettings settings, bool run = false, string filePath = null)
        {
            if (!run)
                return false;

            if(settings == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot save null settings.");
                return false;
            }

            if(string.IsNullOrEmpty(filePath))
                filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Settings", $"{settings.GetType()}.json");

            var json = BH.Engine.Serialiser.Convert.ToJson(settings);
            try
            {
                System.IO.File.WriteAllText(filePath, json);
            }
            catch(Exception ex)
            {
                BH.Engine.Base.Compute.RecordError(ex, $"Error occurred when trying to save settings of type {settings.GetType()} to file path {filePath}.");
                return false;
            }

            return true;
        }

        [Description("Saves all settings in memory to their respective files. Settings are saved back to the same file they were loaded from, overwriting them if they've changed. If settings were added during runtime and do not have an associated file, then a new file will be created with the name {settingsType}.json. If no folder is specified, the default of %ProgramData%/BHoM/Settings/{settingsType}.json will be used.")]
        [Input("outputDirectory", "Optional input to specify where to save settings file to. If the provided output directory differs from the saved load directory, settings will be saved in the provided output directory and not where they were originally loaded from.")]
        [Input("run", "Boolean toggle to determine whether to run the method. Useful for Visual Programming to prevent settings being saved/overwritten before providing a file path if desired.")]
        [Output("success", "True if the settings have been successfully saved, false otherwise.")]
        public static bool SaveSettings(string outputDirectory = null, bool run = false)
        {
            if (!run)
                return false;

            if (string.IsNullOrEmpty(outputDirectory))
                outputDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Settings");

            bool allSuccess = true;
            foreach(var kvp in Global.BHoMSettings)
            {
                string loadedFrom = "";
                if(Global.BHoMSettingsFilePaths.ContainsKey(kvp.Key) && Global.BHoMSettingsFilePaths[kvp.Key].Contains(outputDirectory))
                    loadedFrom = Global.BHoMSettingsFilePaths[kvp.Key]; //Save to the loaded file path if one is stored, and the output directory matches the load path. If the output directory does not match the load path, then this is skipped and the default below is used.

                if (string.IsNullOrEmpty(loadedFrom))
                    loadedFrom = Path.Combine(outputDirectory, $"{kvp.Value.GetType()}.json");

                if (!SaveSettings(kvp.Value, true, loadedFrom))
                {
                    BH.Engine.Base.Compute.RecordError($"Settings of type {kvp.Key} could not be saved to {loadedFrom}.");
                    allSuccess = false;
                }
            }

            return allSuccess;
        }
    }
}
