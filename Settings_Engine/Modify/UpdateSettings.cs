using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.Engine.Settings
{
    public static partial class Modify
    {
        [Description("Update settings in memory with the provided settings object. If the settings already exist in memory, they will be updated to the ones provided. If they do not exist in memory, they will be added.")]
        [Input("settings", "New settings to update or add to memory.")]
        public static void UpdateSettings(ISettings settings)
        {
            if (settings == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot update null settings.");
                return;
            }

            Type type = settings.GetType();
            Global.BHoMSettings[settings.GetType()] = settings;
        }
    }
}
