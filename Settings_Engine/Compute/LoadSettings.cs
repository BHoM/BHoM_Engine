using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BH.Engine.Settings
{
    public static partial class Compute
    {
        [Description("Load all the JSON settings stored within the provided folder into memory. If no folder is provided, the default folder of %ProgramData%/BHoM/Settings is used instead. All JSON files are scraped within the directory (including subdirectories) and deserialised to ISettings objects.")]
        [Input("settingsFolder", "Optional input to determine where to load settings from. Defaults to %ProgramData%/BHoM/Settings if no folder is provided.")]
        public static void LoadSettings(string settingsFolder = null)
        {
            if (string.IsNullOrEmpty(settingsFolder))
                settingsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Settings"); //Defaults to C:/ProgramData/BHoM/Settings if no folder is provided

            var settingsFiles = Directory.EnumerateFiles(settingsFolder, "*.json", SearchOption.AllDirectories);

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
                    Global.BHoMSettings.TryAdd(settings.GetType(), settings);
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordWarning(ex, $"Cannot deserialise the contents of {file} to an ISettings object.");
                }
            }
        }
    }
}
